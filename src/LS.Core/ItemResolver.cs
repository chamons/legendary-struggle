using System;
using System.Collections.Immutable;

namespace LS.Core
{
	// Having a stable reference to a immutable game state that can change
	// out from under you (thus giving you stale references) is hard
	// ItemResolver abstracts that away as much as possible
	// In the happy path, it holds the index into the array and it's pretty fast
	// In the sad path (we reordered) it scans, but most ItemResolvers are
	// short lived anyway

	public interface IItemResolver 
	{
		bool Invalid { get; set; }
	}

	public abstract class ItemResolver<T> : IItemResolver where T : IIdentifiable
	{
		long ID;
		int Index;
		protected GameState State;
		public bool Invalid { get; set; }

		protected abstract ImmutableArray<T> Array { get; }

		public ItemResolver (T item, GameState state)
		{
			ID = item.ID;
			State = state;
			Index = Array.IndexOf (item);
			state.RegisterResolver (this);
		}

		public static implicit operator T (ItemResolver<T> c) => c.Item;

		public T Resolve ()
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

		public T Item => Resolve ();

		public void Update (GameState state)
		{
			State = state;
			Invalid = false;
			Index = -1;
			state.RegisterResolver (this);
		}

		T ResolveByIndex ()
		{
			T potentialHit = Array[Index];
			if (potentialHit.ID == ID)
				return potentialHit;
			else
				return ResolveBySearch ();
		}

		T ResolveBySearch ()
		{
			T item = Array.WithID (ID);
			Index = Array.IndexOf (item);
			return item;
		}
	}

	public class PartyResolver : ItemResolver<Character> 
	{
		public PartyResolver (Character item, GameState state) : base (item, state)
		{
		}

		protected override ImmutableArray<Character> Array => State.Party;
	}

	public class EnemyResolver : ItemResolver<Character> 
	{
		public EnemyResolver (Character item, GameState state) : base (item, state)
		{
		}

		protected override ImmutableArray<Character> Array => State.Enemies;
	}

	public class DelayedActionResolver : ItemResolver<DelayedAction> 
	{
		public DelayedActionResolver (DelayedAction item, GameState state) : base (item, state)
		{
		}

		protected override ImmutableArray<DelayedAction> Array => State.DelayedActions;
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
