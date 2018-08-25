using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LS.Core
{
	public interface IIdentifiable
	{
		long ID { get; }
	}

	public interface ITimeable : IIdentifiable
	{
		int CT { get; }
	}

	[Flags]
	public enum ActionType
	{
		None = 0,
		Damage = 1 << 1,
		Heal = 1 << 2,
		Cooldown = 1 << 3,
		Effect = 1 << 4,
		RemoveEffect = 1 << 5
	}

	public enum GameCondition
	{
		None,
		PartyHealthLow,
		EnemyHealthLow
	}

	public partial class Character : ITimeable
	{
		public long ID { get; }
		public string Name { get; }
		public string CharacterClass { get; }
		public Health Health { get; }
		public ImmutableArray<Skill> Skills { get; }
		public ImmutableArray<StatusEffect> StatusEffects { get; }
		public int CT { get; }

		public Character (long id, string name, string characterClass, Health health, IEnumerable<Skill> skills, IEnumerable<StatusEffect> statusEffects, int ct = 0)
		{
			ID = id;
			Name = name;
			CharacterClass = characterClass;
			Health = health;
			Skills = ImmutableArray.CreateRange (skills ?? Array.Empty<Skill> ());
			StatusEffects = ImmutableArray.CreateRange (statusEffects ?? Array.Empty<StatusEffect> ());
			CT = ct;
		}

		public Character WithID (long id)
		{
			return new Character (id, Name, CharacterClass, Health, Skills, StatusEffects, CT);
		}

		public Character WithName (string name)
		{
			return new Character (ID, name, CharacterClass, Health, Skills, StatusEffects, CT);
		}

		public Character WithCharacterClass (string characterClass)
		{
			return new Character (ID, Name, characterClass, Health, Skills, StatusEffects, CT);
		}

		public Character WithHealth (Health health)
		{
			return new Character (ID, Name, CharacterClass, health, Skills, StatusEffects, CT);
		}

		public Character WithSkills (IEnumerable<Skill> skills)
		{
			return new Character (ID, Name, CharacterClass, Health, skills, StatusEffects, CT);
		}

		public Character WithStatusEffects (IEnumerable<StatusEffect> statusEffects)
		{
			return new Character (ID, Name, CharacterClass, Health, Skills, statusEffects, CT);
		}

		public Character WithCT (int ct)
		{
			return new Character (ID, Name, CharacterClass, Health, Skills, StatusEffects, ct);
		}

		public bool IsAlive => Health.Current > 0;
	}

	public partial class Skill : IIdentifiable
	{
		public long ID { get; }
		public Action Action { get; }
		public bool Available { get; }
		public int Cooldown { get; }
		public int Delay { get; }

		public Skill (long id, Action action, bool available, int cooldown, int delay)
		{
			ID = id;
			Action = action;
			Available = available;
			Cooldown = cooldown;
			Delay = delay;
		}

		public Skill WithID (long id)
		{
			return new Skill (id, Action, Available, Cooldown, Delay);
		}

		public Skill WithAction (Action action)
		{
			return new Skill (ID, action, Available, Cooldown, Delay);
		}

		public Skill WithAvailable (bool available)
		{
			return new Skill (ID, Action, available, Cooldown, Delay);
		}

		public Skill WithCooldown (int cooldown)
		{
			return new Skill (ID, Action, Available, cooldown, Delay);
		}

		public Skill WithDelay (int delay)
		{
			return new Skill (ID, Action, Available, Cooldown, delay);
		}
	}

	public partial class TargettedSkill
	{
		public Skill Skill { get; }
		public TargettingInfo TargetInfo { get; }

		public TargettedSkill (Skill skill, TargettingInfo targetInfo)
		{
			Skill = skill;
			TargetInfo = targetInfo;
		}
	}

	public partial class BehaviorSet
	{
		public Behavior Behavior { get; }
		public string Name { get; }

		public BehaviorSet (Behavior behavior, string name)
		{
			Behavior = behavior;
			Name = name;
		}
	}

	public partial class Behavior
	{
		public ImmutableArray<BehaviorSkill> Skills { get; }

		public Behavior (IEnumerable<BehaviorSkill> skills)
		{
			Skills = ImmutableArray.CreateRange (skills ?? Array.Empty<BehaviorSkill> ());
		}
	}

	public partial class BehaviorSkill
	{
		public string SkillName { get; }
		public GameCondition OverrideCondition { get; }

		public BehaviorSkill (string skillName, GameCondition overrideCondition = GameCondition.None)
		{
			SkillName = skillName;
			OverrideCondition = overrideCondition;
		}
	}

	public partial class GameState
	{
		public long Tick { get; }
		public ImmutableArray<Character> Party { get; }
		public ImmutableArray<Character> Enemies { get; }
		public ImmutableArray<DelayedAction> DelayedActions { get; }
		public long ActivePlayerID { get; }
		List<IItemResolver> ActiveResolvers;

		public GameState (long tick, IEnumerable<Character> party, IEnumerable<Character> enemies, IEnumerable<DelayedAction> delayedActions, long activePlayerID)
		{
			Tick = tick;
			Party = ImmutableArray.CreateRange (party ?? Array.Empty<Character> ());
			Enemies = ImmutableArray.CreateRange (enemies ?? Array.Empty<Character> ());
			DelayedActions = ImmutableArray.CreateRange (delayedActions ?? Array.Empty<DelayedAction> ());
			ActivePlayerID = activePlayerID;
		}

		public GameState WithTick (long tick)
		{
			return new GameState (tick, Party, Enemies, DelayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithParty (IEnumerable<Character> party)
		{
			return new GameState (Tick, party, Enemies, DelayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithEnemies (IEnumerable<Character> enemies)
		{
			return new GameState (Tick, Party, enemies, DelayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithDelayedActions (IEnumerable<DelayedAction> delayedActions)
		{
			return new GameState (Tick, Party, Enemies, delayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithActivePlayerID (long activePlayerID)
		{
			return new GameState (Tick, Party, Enemies, DelayedActions, activePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public IEnumerable <Character> AllCharacters
		{
			get
			{
				foreach (Character e in Enemies)
					yield return e;
				foreach (Character p in Party)
					yield return p;
			}
		}
	}

	public partial struct Health
	{
		public int Current { get; }
		public int Max { get; }

		public Health (int current, int max)
		{
			Current = current;
			Max = max;
		}

		public Health WithCurrent (int current)
		{
			return new Health (current, Max);
		}

		public Health WithMax (int max)
		{
			return new Health (Current, max);
		}
	}

	public partial struct DelayedAction : ITimeable
	{
		public long ID { get; }
		public TargettedAction TargetAction { get; }
		public int CT { get; }
		public TargettedSkill SourceSkill { get; }

		public DelayedAction (long id, TargettedAction targetAction, int ct = 0, TargettedSkill sourceSkill = null)
		{
			ID = id;
			TargetAction = targetAction;
			CT = ct;
			SourceSkill = sourceSkill;
		}

		public DelayedAction WithID (long id)
		{
			return new DelayedAction (id, TargetAction, CT, SourceSkill);
		}

		public DelayedAction WithTargetAction (TargettedAction targetAction)
		{
			return new DelayedAction (ID, targetAction, CT, SourceSkill);
		}

		public DelayedAction WithCT (int ct)
		{
			return new DelayedAction (ID, TargetAction, ct, SourceSkill);
		}

		public DelayedAction WithSourceSkill (TargettedSkill sourceSkill)
		{
			return new DelayedAction (ID, TargetAction, CT, sourceSkill);
		}
	}

	public partial struct TargettingInfo
	{
		public long InvokerID { get; }
		public long TargetID { get; }

		public TargettingInfo (long invokerID, long targetID)
		{
			InvokerID = invokerID;
			TargetID = targetID;
		}
	}

	public partial struct Action
	{
		public string Name { get; }
		public ActionType Type { get; }
		public int Power { get; }
		public string EffectName { get; }

		public Action (string name, ActionType type, int power, string effectName = "")
		{
			Name = name;
			Type = type;
			Power = power;
			EffectName = effectName;
		}

		public Action WithName (string name)
		{
			return new Action (name, Type, Power, EffectName);
		}

		public Action WithType (ActionType type)
		{
			return new Action (Name, type, Power, EffectName);
		}

		public Action WithPower (int power)
		{
			return new Action (Name, Type, power, EffectName);
		}

		public Action WithEffectName (string effectName)
		{
			return new Action (Name, Type, Power, effectName);
		}
	}

	public partial struct TargettedAction
	{
		public Action Action { get; }
		public TargettingInfo TargetInfo { get; }

		public TargettedAction (Action action, TargettingInfo targetInfo)
		{
			Action = action;
			TargetInfo = targetInfo;
		}

		public TargettedAction WithAction (Action action)
		{
			return new TargettedAction (action, TargetInfo);
		}

		public TargettedAction WithTargetInfo (TargettingInfo targetInfo)
		{
			return new TargettedAction (Action, targetInfo);
		}
	}

	public partial struct StatusEffect
	{
		public string Name { get; }

		public StatusEffect (string name)
		{
			Name = name;
		}
	}
}
