using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RogueSharp.Random;

namespace RogueLight
{
	public static class Extensions
	{
		/// <summary>
		/// Picks a random element from a sequence.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="random"></param>
		/// <param name="choices"></param>
		/// <returns></returns>
		public static T Pick<T>(this IRandom random, IEnumerable<T> choices)
		{
			int cnt = choices.Count();
			int idx = random.Next(0, cnt - 1);
			T item = choices.ElementAt(idx);
			return item;
		}

		/// <summary>
		/// Picks a random element from a sequence using weights.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="random"></param>
		/// <param name="choices"></param>
		/// <param name="weightSelector"></param>
		/// <returns></returns>
		public static T PickWeighted<T>(this IRandom random, IEnumerable<T> choices, Func<T, int> weightSelector)
		{
			var list = new List<(T Item, int Weight, int CumulativeWeight)>();
			int cumulativeWeight = 0;
			foreach (var item in choices)
			{
				var weight = weightSelector(item);
				cumulativeWeight += weight;
				list.Add((item, weight, cumulativeWeight));
			}
			var num = random.Next(list.Last().CumulativeWeight);
			var match = list.First(q => q.CumulativeWeight >= num);
			return match.Item;
		}

		/// <summary>
		/// Flips a weighted coin.
		/// </summary>
		/// <param name="random">The random number generator.</param>
		/// <param name="chance">Ranges from zero (always fail) to one (always succeed).</param>
		/// <returns>Success or failure?</returns>
		public static bool Chance(this IRandom random, double chance)
			=> random.Next(1_000_000) < chance * 1_000_000;
	}
}
