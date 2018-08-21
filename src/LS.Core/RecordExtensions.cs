using System;
using System.Collections.Generic;
using System.Linq;

namespace LS.Core
{
	public partial struct Character
	{
		public static Character Create () => new Character (IDs.Next ()); 
	}

	public partial struct DelayedAction
	{
		public static DelayedAction Create (Action action) => new DelayedAction (IDs.Next (), action);
	}

	public partial struct TargettingInfo
	{
		public static TargettingInfo From (Character source, Character target) => new TargettingInfo (source.ID, target.ID);
	}

	public partial class GameState
	{
		internal void RegisterResolver (IItemResolver resolver)
		{
			if (ActiveResolvers == null)
				ActiveResolvers = new List<IItemResolver> ();
			ActiveResolvers.Add (resolver);
		}

		internal void InvalidateResolvers ()
		{
			if (ActiveResolvers != null)
			{
				foreach (var resolver in ActiveResolvers)
					resolver.Invalid = true;
				ActiveResolvers.Clear ();
			}
		}

		public GameState UpdateTimeable (ITimeable t)
		{
			if (t is DelayedAction a)
				return UpdateDelayedAction (a);
			if (t is Character c)
				return UpdateCharacter (c);
			throw new NotImplementedException ();
		}

		public GameState UpdateDelayedAction (DelayedAction delayedEffect)
		{
			return WithDelayedActions (DelayedActions.ReplaceWithID (delayedEffect));
		}

		public GameState UpdateCharacter (Character newCharacter)
		{
			if (Enemies.Any (x => x.ID == newCharacter.ID))
				return WithEnemies (Enemies.ReplaceWithID (newCharacter));
			else 
				return WithParty (Party.ReplaceWithID (newCharacter));
		}
	}
}
