using System;
namespace LS.Core
{
	public class CharacterResolver
	{
		long ID;
		bool IsParty;
		int Index;
		GameState State;

		public CharacterResolver (Character c, GameState state)
		{
			ID = c.ID;
			IsParty = state.Party.Contains (c);
			Index = IsParty ? state.Party.IndexOf (c) : state.Enemies.IndexOf (c);
			State = state;
		}

		public static implicit operator Character (CharacterResolver c) => c.Item;

		public Character Resolve ()
		{
			if (Index != -1)
				ResolveByIndex ();
			return ResolveBySearch ();
		}

		public Character Item => Resolve ();

		public void Update (GameState state)
		{
			State = state;
		}

		Character ResolveByIndex ()
		{
			Character potentialHit = IsParty ? State.Party [Index] : State.Enemies[Index];
			if (potentialHit.ID == ID)
			{
				return potentialHit;
			}
			else
			{
				Index = -1;
				return ResolveBySearch ();
			}
		}

		Character ResolveBySearch ()
		{
			if (IsParty)
				return State.Party.WithID (ID);
			else
				return State.Enemies.WithID (ID);
		}
	}
}
