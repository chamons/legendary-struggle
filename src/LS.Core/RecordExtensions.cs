using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LS.Core
{
	public partial class Character
	{
		public static Character Create (string name, string characterClass, Health health) => new Character (IDs.Next (), name, characterClass, health, null, null); 

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

		public Skill SkillWithName (string name) => Skills.FirstOrDefault (x => x.SkillName == name);
		public bool HasSkill (string name) => Skills.Any (x => x.SkillName == name);

		public Character WithResetSkills () => WithSkills (Skills.Select (x => x.WithAvailable (true)));

		public override string ToString() => $"{Name} ({ID}) {Health}";
	}

	public partial struct Health
	{
		public Health WithDeltaCurrent (int delta)
		{
			int newHealthValue = (Current + delta).Clamp (0, Max);
			return WithCurrent (newHealthValue);
		}

		public Health WithFull () => WithCurrent (Max);

		public bool IsLow => (Current / (double)Max) < .25;

		public override string ToString() => $"{Current}/{Max}";
	}

	public partial class Skill
	{
		public static Skill Create (string name, string skillName, Action action, int cooldown, int delay) => new Skill (IDs.Next (), name, skillName, action, true, cooldown, delay);
	}

	public partial class TargettedSkill
	{
		public TargettedAction CreateAction () => new TargettedAction (Skill.Action, TargetInfo);
	}

	public partial struct DelayedAction
	{
		public static DelayedAction Create (TargettedAction action, int ct = 0, TargettedSkill sourceSkill = null)
		{
			return new DelayedAction (IDs.Next (), action, ct, sourceSkill);
		}

		public DelayedAction ExtendDuration (int ct) => WithCT (CT - ct);
	}

	public partial struct TargettingInfo : IEquatable<TargettingInfo>
	{
		public static TargettingInfo From (Character source, Character target) => new TargettingInfo (source.ID, target.ID);
		public static TargettingInfo From (long source, long target) => new TargettingInfo (source, target);
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
		public Character ActiveCharacter => Party.WithID (ActivePlayerID);

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

		public ImmutableArray<Character> GetTeammates (ItemResolver<Character> c)
		{
			return Enemies.Contains (c.Item) ? Enemies : Party;
		}

		public ImmutableArray<Character> GetOpponents (ItemResolver<Character> c)
		{
			return Enemies.Contains (c.Item) ? Party : Enemies;
		}

		public ImmutableArray<Character> GetTeammates (Character c)
		{
			return Enemies.Contains (c) ? Enemies : Party;
		}

		public ImmutableArray<Character> GetOpponents (Character c)
		{
			return Enemies.Contains (c) ? Party : Enemies;
		}

	}
}
