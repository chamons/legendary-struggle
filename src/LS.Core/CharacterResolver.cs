using System;

namespace LS.Core
{
	// Having a stable reference to a immutable game state that can change
	// out from under you (thus giving you stale references) is hard
	// CharacterResolver abstracts that away as much as possible
	// In the happy path, it holds the index into the array and it's pretty fast
	// In the sad path (we reordered) it scans, but most CharacterResolver are
	// short lived anyway
	public class CharacterResolver
	{
		long ID;
		bool IsParty;
		int Index;
		GameState State;
		internal bool Invalid;

		public CharacterResolver (Character c, GameState state)
		{
			ID = c.ID;
			IsParty = state.Party.Contains (c);
			Index = IsParty ? state.Party.IndexOf (c) : state.Enemies.IndexOf (c);
			State = state;
			state.RegisterResolver (this);
		}

		public static implicit operator Character (CharacterResolver c) => c.Item;

		public Character Resolve ()
		{
			if (Invalid)
				throw new StaleReferenceException ();
			try
			{
				if (Index != -1)
					ResolveByIndex ();
				return ResolveBySearch ();
			}
			catch (InvalidOperationException)
			{
				throw new CharacterNotFoundException (); 
			}
			catch (IndexOutOfRangeException)
			{
				throw new CharacterNotFoundException (); 
			}
		}

		public Character Item => Resolve ();

		public void Update (GameState state)
		{
			State = state;
			Invalid = false;
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

	public class CharacterNotFoundException : Exception
	{
		public CharacterNotFoundException() : base ()
		{
		}
	}

	public class StaleReferenceException : Exception
	{
		public StaleReferenceException () : base ()
		{
		}
	}
}
