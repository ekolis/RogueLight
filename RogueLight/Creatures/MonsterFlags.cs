using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLight.Creatures
{
	[Flags]
	public enum MonsterFlags
	{
		/// <summary>
		/// No flags are set.
		/// </summary>
		None,
		/// <summary>
		/// The monster is unique, and will only be spawned once.
		/// </summary>
		Unique = 0x1,
		/// <summary>
		/// The monster is the final boss. When it is defeated, the endgame scenario will start!
		/// </summary>
		FinalBoss = 0x2,
	}
}
