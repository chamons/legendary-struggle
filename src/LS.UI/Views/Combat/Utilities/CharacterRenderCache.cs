using System;
using System.Collections.Generic;
using LS.Model;
using LS.UI.Views.Combat.Renderers;

namespace LS.UI.Views.Combat.Utilities
{
	class CharacterRenderCache
	{
		Dictionary<string, CharacterRenderer> Cache = new Dictionary<string, CharacterRenderer> ();

		// TestData - Should be based on name not slot
		CharacterRenderer CreateRendererBySlot (int slot)
		{
			switch (slot)
			{
				case 0:
					return CharacterRenderer.CreateNormalSized ("data/characters/chara6.png", 21, true);
				case 1:
					return CharacterRenderer.CreateNormalSized ("data/characters/chara7.png", 15, true);
				case 2:
					return CharacterRenderer.CreateNormalSized ("data/characters/chara2.png", 12, false);
				case 3:
					return CharacterRenderer.CreateNormalSized ("data/characters/chara2.png", 69, false);
				case 4:
					return CharacterRenderer.CreateNormalSized ("data/characters/chara2.png", 21, false);
				case 5:
					return CharacterRenderer.CreateExtraLarge ("data/characters/$monster_bird1.png", 0);
				default:
					throw new NotImplementedException ();
			}
		}

		CharacterRenderer CreateRenderer (Character c)
		{
			CharacterRenderer renderer = CreateRendererBySlot (c.Slot);
			Cache [c.Name] = renderer;
			return renderer;
		}

		public CharacterRenderer this [Character c]
		{
			get
			{
				if (Cache.TryGetValue (c.Name, out CharacterRenderer value))
					return value;
				else
					return CreateRenderer (c);
			}
		}
	}
}
