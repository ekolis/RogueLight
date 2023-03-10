using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using RogueLight.Magic;
using RogueLight.Mapping;
using RogueLight.Creatures;

namespace RogueLight
{
	/// <summary>
	/// Logs things for the player to read.
	/// </summary>
	public static class Logger
	{
		/// <summary>
		/// Logs a message with a color.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="color"></param>
		public static void Log(string message, Color color)
		{
			Entries.Add(new(message, DateTime.Now, color));
		}

		/// <summary>
		/// Logs an attack.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="target"></param>
		/// <param name="damage"></param>
		/// <param name="crit"></param>
		public static void LogAttack(ICreature attacker, ICreature target, int damage, bool crit)
		{
			var critstr = crit ? " CRITICAL! Triple strength!" : "";
			if (attacker is Hero)
				Log($"{attacker.Capitalize()} attack {target}.{critstr} ({damage} dmg, {target.Hitpoints} HP left)", Color.White);
			else // monster
				Log($"{attacker.Capitalize()} attacks {target}.{critstr} ({damage} dmg, {target.Hitpoints} HP left)", Color.Yellow);
		}

		/// <summary>
		/// Logs a death.
		/// </summary>
		/// <param name="decedent"></param>
		public static void LogDeath(ICreature decedent)
		{
			if (decedent is Hero)
				Log($"{decedent.Capitalize()} die...", Color.Red);
			else
				Log($"{decedent.Capitalize()} dies.", Color.Cyan);
		}

		/*public static void LogSpellCast(ICreature caster, double power, double efficiency, params Element[] elements)
		{
			string powerStr = "", efficencyStr = "";
			var thresholdLow = 1.0 / 3.0;
			var thresholdHigh = 2.0 / 3.0;
			if (power >= thresholdHigh)
				powerStr = "powerful";
			else if (power <= thresholdLow)
				powerStr = "weak";
			if (efficiency >= thresholdHigh)
				efficencyStr = "keen";
			if (efficiency <= thresholdLow)
				efficencyStr = "blunt";
			var desc = string.Join(" ", new string[] { powerStr, efficencyStr }).Trim();
			if (desc == "")
				desc = "standard";
			if (caster is Hero)
				Log($"{caster.Capitalize()} cast a {desc} spell: {string.Join<Element>("/", elements)} ({elements.Last().GetManaCost(efficiency)} MP).", elements.Last().Color);
			else
				Log($"{caster.Capitalize()} casts a {desc} spell: {string.Join<Element>("/", elements)} ({elements.Last().GetManaCost(efficiency)} MP).", elements.Last().Color);
		}

		public static void LogSpellDamage(ICreature target, Element element, int dmg)
		{
			if (element is Fire)
				Log($"{target.Capitalize()} {(target is Hero ? "are" : "is")} burned! ({dmg} dmg, {target.Hitpoints} HP left)", element.Color);
			else
				Log($"{target.Capitalize()} {(target is Hero ? "are" : "is")} hit with an unknown element! ({dmg} dmg, {target.Hitpoints} HP left)", element.Color);
		}

		public static void LogSpellFizzle(ICreature caster)
		{
			Log($"{caster.Capitalize()} {(caster is Hero ? "try" : "tries")} to cast a spell, but it fizzles.", Color.White);
		}

		public static void LogStatusEffectStart(ICreature creature, Element element, StatusEffect fx, double duration)
		{
			var desc = fx switch
			{
				StatusEffect.Slow => "slowed",
				_ => "afflicted with a bizarre, never before seen status effect (did someone forget to write a description?)"
			};
			Log($"{creature.Capitalize()} {(creature is Hero ? "are" : "is")} {desc} for {Math.Round(duration)} turn(s).", element.Color);
		}

		// TODO: LogStatusEffectEnd

		public static void LogKnockback(ICreature creature, Element element, Direction direction, int distance)
		{
			if (direction == Direction.Stationary)
			{
				if (creature is Hero)
					Log($"{creature.Capitalize()} twirl about...", element.Color);
				else // monster
					Log($"{creature.Capitalize()} twirls about...", element.Color);
			}
			else if (distance == 0)
			{
				if (creature is Hero)
					Log($"{creature.Capitalize()} lean slightly to the {direction.Lowercase()}...", element.Color);
				else // monster
					Log($"{creature.Capitalize()} leans sligihtly to the {direction.Lowercase()}...", element.Color);
			}
			else
			{
				if (creature is Hero)
					Log($"{creature.Capitalize()} were knocked back ({distance} to the {direction.Lowercase()}).", element.Color);
				else // monster
					Log($"{creature.Capitalize()} was knocked back ({distance} to the {direction.Lowercase()}).", element.Color);
			}
		}

		public static void LogManaDrain(ICreature caster, ICreature target, Element element, int drain)
		{
			if (caster is Hero)
				Log($"{caster.Capitalize()} drain {drain} mana from {target}.", element.Color);
			else
				Log($"{caster.Capitalize()} drains {drain} mana from {target}.", element.Color);
		}*/

		public static void LogHealing(ICreature creature, int healing)
		{
			if (creature is Hero)
				Log($"{creature.Capitalize()} are healed ({healing} HP).", Color.Green);
			else
				Log($"{creature.Capitalize()} is healed ({healing} HP).", Color.Green);
		}

		/*public static void LogStun(ICreature creature, Element element, double stun)
		{
			if (creature is Hero)
				Log($"{creature.Capitalize()} are stunned for {Math.Round(stun)} turns!", element.Color);
			else
				Log($"{creature.Capitalize()} is stunned for {Math.Round(stun)} turns.", element.Color);
		}

		public static void LogHPDrain(ICreature caster, ICreature target, Element element, int drain)
		{
			if (caster is Hero)
				Log($"{caster.Capitalize()} drain {drain} HP from {target}.", element.Color);
			else
				Log($"{caster.Capitalize()} drains {drain} HP from {target}.", element.Color);
		}

		public static void LogEssenceDrain(ICreature caster, ICreature target, Element casterElement, Element targetElement, int drain)
		{
			// TODO: special messages for hero or monster draining itself?
			if (caster is Hero)
			{
				Log($"{caster.Capitalize()} drain {drain} {casterElement.GetType().Name} essences from {target}.", casterElement.Color);
				Log($"Your {casterElement.GetType().Name} attunement increases to {Math.Round(casterElement.Attunement * 100)}%!", casterElement.Color);
			}
			else if (target is Hero)
			{
				Log($"{caster.Capitalize()} drains {drain} {targetElement.GetType().Name} essences from {target}!", targetElement.Color);
				Log($"Your {targetElement.GetType().Name} attunement decreases to {Math.Round(targetElement.Attunement * 100)}%!", targetElement.Color);
			}
			else
			{
				// monster on monster action! don't show attunement
				Log($"{caster.Capitalize()} drains {drain} {targetElement.GetType().Name} essences from {target}!", targetElement.Color);
			}
		}

		public static void LogManaRestoration(ICreature creature, Element element, int mana)
		{
			if (creature is Hero)
				Log($"{creature.Capitalize()} have your mana restored ({mana} MP).", element.Color);
			else
				Log($"{creature.Capitalize()} has its mana restored ({mana} MP).", element.Color);
		}

		public static void LogCriticalSpell(ICreature caster, Element element)
		{
			if (caster is Hero)
				Log($"Your spell goes haywire! CRITICAL! Triple spell power!", element.Color);
			else
				Log($"{caster}'s spell goes haywire! CRITICAL! Triple spell power!", element.Color);
		}*/

		/// <summary>
		/// Gets a list of all unexpired entries.
		/// </summary>
		/// <returns></returns>
		public static IEnumerable<Entry> List()
		{
			var now = DateTime.Now;
			Entries = Entries.Where(q => now - q.Timestamp < Duration).ToList();
			return Entries;
		}

		public static TimeSpan Duration { get; } = new(0, 0, 15);

		private static IList<Entry> Entries { get; set; } = new List<Entry>();

		/// <summary>
		/// A log entry.
		/// </summary>
		public record Entry(string Message, DateTime Timestamp, Color Color);
	}
}
