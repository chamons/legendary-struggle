using System;
using LS.Core;

namespace LS.UI.Utilities
{
	static class RecordExtensions
	{
		public static int GetSlot (this Character c, GameState state)
		{
			int slot = state.GetTeammates (c).IndexOf (c);
			if (state.Party.Contains (c))
				return slot;
			else
				return slot + 5;
		}

		public static Character GetCharacterFromSlot (this GameState state, int slot)
		{
			if (slot < 5)
				return state.Party [slot];
			else
				return state.Enemies [slot - 5];
		}
	}
}
