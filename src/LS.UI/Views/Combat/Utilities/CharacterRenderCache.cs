using System;
using System.Collections.Generic;
using LS.UI.Views.Combat.Renderers;

using LS.Core;

namespace LS.UI.Views.Combat.Utilities
{
	class CharacterRenderCache
	{
		Dictionary<string, CharacterRenderer> Cache = new Dictionary<string, CharacterRenderer> ();

		CharacterRenderer CreateRendererByClass (string characterClass)
		{
			switch (characterClass)
			{
				case "Mage":
					return CharacterRenderer.CreateNormalSized ("data/characters/chara6.png", 21, true);
				case "Thief":
					return CharacterRenderer.CreateNormalSized ("data/characters/chara2.png", 21, false);
				case "Monster":
					return CharacterRenderer.CreateExtraLarge ("data/characters/$monster_bird1.png", 0);
				default:
					throw new NotImplementedException ();
			}
		}

		CharacterRenderer CreateRenderer (Character c)
		{
			CharacterRenderer renderer = CreateRendererByClass (c.CharacterClass);
			Cache [c.CharacterClass] = renderer;
			return renderer;
		}

		public CharacterRenderer this [Character c]
		{
			get
			{
				if (Cache.TryGetValue (c.CharacterClass, out CharacterRenderer value))
					return value;
				else
					return CreateRenderer (c);
			}
		}
	}
}
