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
		None = 1 << 0,
		Damage = 1 << 1,
		Heal = 1 << 2,
	}

	public partial class Character : ITimeable
	{
		public long ID { get; }
		public Health Health { get; }
		public int CT { get; }

		public Character (long id, Health health, int ct = 0)
		{
			ID = id;
			Health = health;
			CT = ct;
		}

		public Character WithID (long id)
		{
			return new Character (id, Health, CT);
		}

		public Character WithHealth (Health health)
		{
			return new Character (ID, health, CT);
		}

		public Character WithCT (int ct)
		{
			return new Character (ID, Health, ct);
		}

		public bool IsAlive => Health.Current > 0;
	}

	public partial class GameState
	{
		public long Tick { get; }
		public ImmutableArray<Character> Enemies { get; }
		public ImmutableArray<Character> Party { get; }
		public ImmutableArray<DelayedAction> DelayedActions { get; }
		public long ActivePlayerID { get; }
		List<IItemResolver> ActiveResolvers;

		public GameState (long tick, IEnumerable<Character> enemies, IEnumerable<Character> party, IEnumerable<DelayedAction> delayedActions, long activePlayerID)
		{
			Tick = tick;
			Enemies = ImmutableArray.CreateRange (enemies ?? Array.Empty<Character> ());
			Party = ImmutableArray.CreateRange (party ?? Array.Empty<Character> ());
			DelayedActions = ImmutableArray.CreateRange (delayedActions ?? Array.Empty<DelayedAction> ());
			ActivePlayerID = activePlayerID;
		}

		public GameState WithTick (long tick)
		{
			return new GameState (tick, Enemies, Party, DelayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithEnemies (IEnumerable<Character> enemies)
		{
			return new GameState (Tick, enemies, Party, DelayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithParty (IEnumerable<Character> party)
		{
			return new GameState (Tick, Enemies, party, DelayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithDelayedActions (IEnumerable<DelayedAction> delayedActions)
		{
			return new GameState (Tick, Enemies, Party, delayedActions, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithActivePlayerID (long activePlayerID)
		{
			return new GameState (Tick, Enemies, Party, DelayedActions, activePlayerID) { ActiveResolvers = this.ActiveResolvers };
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

		public IEnumerable <ITimeable> AllTimables
		{
			get
			{
				foreach (Character e in Enemies)
					yield return e;
				foreach (Character p in Party)
					yield return p;
				foreach (DelayedAction a in DelayedActions)
					yield return a;
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
		public Action Action { get; }
		public int CT { get; }

		public DelayedAction (long id, Action action, int ct = 0)
		{
			ID = id;
			Action = action;
			CT = ct;
		}

		public DelayedAction WithID (long id)
		{
			return new DelayedAction (id, Action, CT);
		}

		public DelayedAction WithAction (Action action)
		{
			return new DelayedAction (ID, action, CT);
		}

		public DelayedAction WithCT (int ct)
		{
			return new DelayedAction (ID, Action, ct);
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
		public TargettingInfo TargetInfo { get; }
		public ActionType Type { get; }
		public int Power { get; }

		public Action (string name, TargettingInfo targetInfo, ActionType type, int power)
		{
			Name = name;
			TargetInfo = targetInfo;
			Type = type;
			Power = power;
		}

		public Action WithName (string name)
		{
			return new Action (name, TargetInfo, Type, Power);
		}

		public Action WithTargetInfo (TargettingInfo targetInfo)
		{
			return new Action (Name, targetInfo, Type, Power);
		}

		public Action WithType (ActionType type)
		{
			return new Action (Name, TargetInfo, type, Power);
		}

		public Action WithPower (int power)
		{
			return new Action (Name, TargetInfo, Type, power);
		}
	}
}
