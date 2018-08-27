using LS.Core;
using LS.UI.Utilities;

namespace LS.UI.Views.Combat.Utilities
{
	public static class CharacterRenderLocation
	{
		const int CharacterStartingY = 180;
		const int CharacterYDelta = 80;
		static Point [] PlayerOffsets = {
			new Point (525, CharacterStartingY), new Point (525, CharacterStartingY + (1 * CharacterYDelta)),
			new Point (525, CharacterStartingY + (2 * CharacterYDelta)),
			new Point (525, CharacterStartingY + (3 * CharacterYDelta)),
			new Point (525, CharacterStartingY + (4 * CharacterYDelta))
		};

		// MultipleEnemies
		static Point [] EnemyOffsets = {
			new Point (300, 275),
		};

		public static Point GetRenderPoint (GameState state, Character c)
		{
			int slot = state.GetTeammates (c).IndexOf (c);
			if (state.Party.Contains (c))
				return PlayerOffsets [slot];
			else
				return EnemyOffsets [slot];
		}
	}
}
