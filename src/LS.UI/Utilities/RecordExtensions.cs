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
	}
}
