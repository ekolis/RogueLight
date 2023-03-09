using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using RogueLight.Magic;
using RogueSharp;
using RogueLight.Mapping;
using RogueLight.UI;

namespace RogueLight.Creatures
{
	/// <summary>
	/// Our intrepid hero, exploring the world... 🧙🏼‍
	/// </summary>
	public class Hero
		: ICreature
	{
		/// <summary>
		/// Hero is a singleton, so the constructor is private.
		/// </summary>
		private Hero()
		{
			Hitpoints = MaxHitpoints;
			Mana = MaxMana;
		}

		/// <summary>
		/// The singleton instance of the hero.
		/// </summary>
		public static Hero Instance { get; } = new Hero();

		public string Name
			=> "you";

		public char Glyph { get; } = '@';

		public Color Color
		{
			get
			{
				var damageColors = new List<Color>();
				var dmgTenths = 10 - (int)(10d * Hitpoints / MaxHitpoints);
				for (var i = 0; i < 10; i++)
				{
					if (i < dmgTenths)
					{
						damageColors.Add(Color.Red);
					}
					else
					{
						damageColors.Add(Color.White);
					}
				}
				return damageColors.Average();
			}
		}

		public FieldOfView? FieldOfView { get; set; }

		public void UpdateFov()
		{
			var floor = Floor.Current;
			if (floor is null)
				return;

			if (FieldOfView is null)
				FieldOfView = new FieldOfView(floor);

			var newtile = floor.Find(this);
			var fovData = FieldOfView.ComputeFov(newtile.X, newtile.Y, 999, true);
			foreach (var tile in floor.Tiles)
				floor.Tiles[tile.X, tile.Y] = floor.Tiles[tile.X, tile.Y].WithInvisible();
			foreach (var fovCell in fovData)
			{
				var tile = floor.Tiles[fovCell.X, fovCell.Y];
				if (tile.Brightness * Vision >= 1d)
				{
					floor.Tiles[fovCell.X, fovCell.Y] = tile.WithVisible();
				}
			}
		}

		public void ResetFov()
			=> FieldOfView = null;

		public double Act()
		{
			if (Floor.Current is null)
				return 0; // nothing to do

			// get globals
			Floor floor = Floor.Current;

			// which way are we moving/casting?
			var dir = Keyboard.ActionDirection;

			// shift is used for climbing stairs
			bool shift = Keyboard.IsKeyPressed(Keys.ShiftKey);

			// HACK: why is this necessary?
			Keyboard.Reset();

			if (Pulse > 0)
			{
				Pulse *= -1;
				Logger.Log("Your light is quenched for a little while.", Color.Gray);
			}
			else if (Pulse <= 0)
			{
				Pulse++;
				if (Pulse > 0)
				{
					Pulse = 0;
				}
				Logger.Log("Your light is gradually restored.", Color.Gray);
			}

			bool success = false;

			/*if (Spell is not null)
			{
				if (Spell.CanCast)
				{
					// we are casting a spell
					Spell.Cast();
					success = true;
				}
				else
				{
					// not enough attunement from essences
					Logger.LogSpellFizzle(this);
					success = false;
				}
			}
			else*/ if (dir is not null)
			{
				if (shift)
				{
					if (dir == Direction.Stationary)
					{
						// holding shift while standing still climbs stairs
						// because > is shift plus .
						success = ClimbStairs();
					}
					else
					{
						// can't use shift with other directions besides stationary
						success = false;
					}
				}
				else
				{
					// move hero
					success = floor.Move(this, dir, true) > 0;
				}
			}

			if (success)
			{
				// update the hero's field of view
				UpdateFov();

				// strong light damages enemies
				var whereami = Floor.Current.Tiles.Cast<Tile>().SingleOrDefault(q => q.Creature == this);
				if (whereami != null)
				{
					var fovData = FieldOfView.ComputeFov(whereami.X, whereami.Y, 999, true);
					foreach (var cell in fovData)
					{
						var tile = Floor.Current.Tiles[cell.X, cell.Y];
						if (tile.Brightness > 0 && tile.Creature is Monster m && m.Brightness < 0)
						{
							var dmg = (int)(tile.Brightness / -tile.Creature.Brightness);
							if (dmg > 0)
							{
								this.InflictDamage(tile.Creature, dmg);
								Logger.LogAttack(this, m, dmg, false);
							}
						}
					}
				}

				// spend time
				return 1.0 / Speed;
			}

			return 0;
		}

		public int Vision { get; } = 3;

		public double Speed
			=> 1d / (this.HasStatusEffect(StatusEffect.Slow) ? 2 : 1);

		public double Timer { get; set; }

		public int Strength { get; } = 1;

		public int MaxHitpoints => 10;

		public int Hitpoints { get; set; }

		public int MaxMana => 10;

		public int Mana { get; set; }

		public override string ToString()
			=> Name;

		/// <summary>
		/// When did the hero die? Or null if he didn't.
		/// </summary>
		public DateTime? DeathTimestamp { get; set; }

		/// <summary>
		/// How long should the screen take to fade when the hero dies?
		/// </summary>
		public TimeSpan DeathFadeTime { get; } = new TimeSpan(0, 0, 5);

		/// <summary>
		/// Is the spellcasting interface open?
		/// </summary>
		public bool IsCasting { get; set; } = false;

		//private Element[]? elements;

		/*public IEnumerable<Element> Elements
		{
			get
			{
				if (elements is null)
				{
					// you get one attack spell
					var fire = new Fire(0);
					var darkness = new Darkness(0);
					World.Instance.Rng.Pick(new Element[] { fire, darkness }).Essences = Element.EssencesForStandardAttunement;

					// and one utility spell
					var air = new Air(0);
					var earth = new Earth(0);
					var water = new Water(0);
					var light = new Light(0);
					World.Instance.Rng.Pick(new Element[] { air, earth, water, light }).Essences = Element.EssencesForStandardAttunement;

					// set up elements list
					elements = new Element[] { fire, earth, air, water, light, darkness };
				}
				return elements;
			}
		}*/

		/// <summary>
		/// The magic word currently being typed/cast.
		/// </summary>
		public string SpellWord { get; set; } = "";

		/// <summary>
		/// The spell that the hero is casting.
		/// </summary>
		//public Spell? Spell { get; set; }

		/// <summary>
		/// How long did it take the player to type the magic words to cast a spell?
		/// </summary>
		public TimeSpan? SpellDuration
			=> DateTime.Now - SpellTimestamp;

		/// <summary>
		/// When did the player start typing the magic words to cast a spell?
		/// </summary>
		public DateTime? SpellTimestamp { get; set; }

		public IDictionary<StatusEffect, double> StatusEffects { get; } = new Dictionary<StatusEffect, double>();

		/// <summary>
		/// Attempts to climb stairs, if present.
		/// </summary>
		/// <returns>true if successful, otherwise false.</returns>
		public bool ClimbStairs()
		{
			if (Floor.Current.Find(this).Terrain == Terrain.StairsDown)
			{
				if (!World.Instance.IsEndgame)
				{
					// there are down stairs here, so let's generate a new floor!
					World.Instance.ClimbDown();
					/*if (EssenceBoostRegeneration > 0)
					{
						var healing = this.Heal(EssenceBoostRegeneration);
						Logger.LogHealing(this, healing);
						//var manaRestoration = this.RestoreMana(EssenceBoostRegeneration);
						//Logger.LogManaRestoration(this, GetElement<Light>(), manaRestoration);
					}*/
					IsClimbing = true;
					return true;
				}
				else
				{
					Logger.Log("It's too dangerous down there! Flee upward!", Color.White);
					return false;
				}
			}
			else if (Floor.Current.Find(this).Terrain == Terrain.StairsUp)
			{
				if (World.Instance.IsEndgame)
				{
					World.Instance.ClimbUp();
					/*if (EssenceBoostRegeneration > 0)
					{
						var healing = this.Heal(EssenceBoostRegeneration);
						Logger.LogHealing(this, healing);
						//var manaRestoration = this.RestoreMana(EssenceBoostRegeneration);
						//Logger.LogManaRestoration(this, GetElement<Light>(), manaRestoration);
					}*/
					IsClimbing = true;
					return true;
				}
				else
				{
					Logger.Log("You cannot return to the surface in shame! Downward, to your destiny!", Color.White);
					return false;
				}
			}
			else
			{
				// no stairs here to climb.
				return false;
			}
		}

		/// <summary>
		/// Are we currently climbing stairs? If so, let's not move any monsters on the old floor, that won't work...
		/// </summary>
		public bool IsClimbing { get; set; }

		/*public T GetElement<T>()
			where T : Element
			=> Elements.OfType<T>().Single();*/

		/// <summary>
		/// Gets the attunement value of an element.
		/// </summary>
		/// <typeparam name="T">The element to check.</typeparam>
		/// <returns>The attunement.</returns>
		/*public double GetAttunement<T>()
			where T : Element
			=> Elements.OfType<T>().Max(q => q.Attunement);*/

		/*/// <summary>
		/// Fire essences boost your spell power.
		/// </summary>
		public double EssenceBoostSpellPower => GetAttunement<Fire>() * 0.20;

		/// <summary>
		/// Earth essences boost your max HP.
		/// </summary>
		public int EssenceBoostMaxHitpoints => (int)Math.Round(GetAttunement<Earth>() * 5);

		/// <summary>
		/// Air essences boost your speed.
		/// </summary>
		public double EssenceBoostSpeed => GetAttunement<Air>() * 0.10;

		/// <summary>
		/// Water essences boost your max mana.
		/// </summary>
		public int EssenceBoostMaxMana => (int)Math.Round(GetAttunement<Water>() * 5);

		/// <summary>
		/// Light essences let you regenerate some of your HP and mana when you climb stairs.
		/// </summary>
		public int EssenceBoostRegeneration => (int)Math.Round(GetAttunement<Light>() * 5);

		/// <summary>
		/// Darkness essences give you a chance for critical hits from melee attacks and spells.
		/// </summary>
		public double EssenceBoostCriticalHits => GetAttunement<Darkness>() * 0.10;*/

		public double Brightness
			=> 10d * (1d + Pulse);

		/// <summary>
		/// Positive means a pulse is being emitted; negative means recovering from pulse.
		/// </summary>
		public double Pulse { get; set; } = 0;
	}
}
