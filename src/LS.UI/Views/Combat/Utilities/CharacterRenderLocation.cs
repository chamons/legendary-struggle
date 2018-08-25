using LS.Model;
using LS.Utilities;

namespace LS.UI.Views.Combat.Utilities
{
	public static class CharacterRenderLocation
	{
		const int CharacterStartingY = 180;
		const int CharacterYDelta = 80;
		static Point [] PlayerOffsets = new Point [] {
			new Point (525, CharacterStartingY), new Point (525, CharacterStartingY + (1 * CharacterYDelta)),
			new Point (525, CharacterStartingY + (2 * CharacterYDelta)),
			new Point (525, CharacterStartingY + (3 * CharacterYDelta)),
			new Point (525, CharacterStartingY + (4 * CharacterYDelta))
		};

		// MultipleEnemies
		static Point [] EnemyOffsets = new Point [] {
			new Point (300, 275),
		};

		public static Point GetRenderPoint (Character c)
		{
			if (c.Slot < 5)
				return PlayerOffsets [c.Slot];
			else
				return EnemyOffsets [c.Slot - 5];
		}
	}
}
