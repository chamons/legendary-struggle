using System;
using System.Collections.Generic;
using System.Linq;

namespace LS.Core
{
	public partial class Character
	{
		public static Character Create (Health health) => new Character (IDs.Next (), health, null, null); 

		public Character WithDeltaCurrentHealth (int delta) => WithHealth (Health.WithDeltaCurrent (delta));
		public Character WithCurrentHealth (int current) => WithHealth (Health.WithCurrent (current));

		public Character WithUpdatedSkill (Skill s)
		{
			return WithSkills (Skills.ReplaceWithID (s));
		}

		public Character AddStatusEffect (StatusEffect effect)
		{
			return WithStatusEffects (StatusEffects.Add (effect));
		}

		public bool HasEffect (string name) => StatusEffects.Any (x => x.Name == name);
	}

	public partial struct Health
	{
		public Health WithDeltaCurrent (int delta)
		{
			int newHealthValue = (Current + delta).Clamp (0, Max);
			return WithCurrent (newHealthValue);
		}
	}

	public partial class Skill
	{
		public static Skill Create (Action action, int cooldown, int delay) => new Skill (IDs.Next (), action, true, cooldown, delay);
	}

	public partial class TargettedSkill
	{
		public TargettedAction CreateAction () => new TargettedAction (Skill.Action, TargetInfo);
	}

	public partial struct DelayedAction
	{
		public static DelayedAction Create (TargettedAction action, int ct = 0) => new DelayedAction (IDs.Next (), action, ct);

		public DelayedAction ExtendDuration (int ct) => WithCT (CT + ct);
	}

	public partial struct TargettingInfo : IEquatable<TargettingInfo>
	{
		public static TargettingInfo From (Character source, Character target) => new TargettingInfo (source.ID, target.ID);
		public static TargettingInfo Self (Character source) => new TargettingInfo (source.ID, source.ID);
		public static TargettingInfo Self (long id) => new TargettingInfo (id, id);

		public static TargettingInfo Empty = new TargettingInfo (-1, -1);

		public override bool Equals(object obj) => (obj is TargettingInfo info) && Equals(info);
		public bool Equals(TargettingInfo other) => (InvokerID, TargetID) == (other.InvokerID, other.TargetID);
		public override int GetHashCode() => (InvokerID, TargetID).GetHashCode ();
		public static bool operator ==(TargettingInfo left, TargettingInfo right) => Equals (left, right);
		public static bool operator !=(TargettingInfo left, TargettingInfo right) => !Equals (left, right);
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

		public GameState AddDelayedAction (DelayedAction action)
		{
			return WithDelayedActions (DelayedActions.Add (action));
		}

		public GameState ExtendDelayedAction (DelayedAction action, int amount)
		{
			return UpdateDelayedAction (DelayedActions.WithID (action.ID).ExtendDuration (amount));
		}
	}
}
