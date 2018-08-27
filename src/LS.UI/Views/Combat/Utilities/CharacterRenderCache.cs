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
				case "Monster-Imp":
					return CharacterRenderer.CreateExtraLargeAndTall ("data/characters/elemental.png", 3);
				case "Monster-Bear":
					return CharacterRenderer.CreateSquare ("data/animals/animals5.png", 75);
				case "Monster-Dark Knight":
					return CharacterRenderer.CreateFourByThree ("data/characters/$monster_dknight2.png", 6);
				default:
					throw new NotImplementedException ();
			}
		}

		CharacterRenderer CreateRenderer (Character c)
		{
			string key = GetKey (c);
			CharacterRenderer renderer = CreateRendererByClass (key);
			Cache [key] = renderer;
			return renderer;
		}

		public CharacterRenderer this [Character c]
		{
			get
			{
				if (Cache.TryGetValue (GetKey (c), out CharacterRenderer value))
					return value;
				else
					return CreateRenderer (c);
			}
		}

		string GetKey (Character c) => c.CharacterClass == "Monster" ? c.CharacterClass + "-" + c.Name : c.CharacterClass;
	}
}
