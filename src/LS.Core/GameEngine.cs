using System;
using System.Linq;

namespace LS.Core
{
	public class DelayedActionFiredEventArgs : EventArgs
	{
		public DelayedActionFiredEventArgs (DelayedAction action)
		{
			Action = action;
		}

		public DelayedAction Action;
	}

	// TODO needs skill channel event

	public class SkillUsedEventArgs : EventArgs
	{
		public SkillUsedEventArgs (Character character, Skill skill)
		{
			Character = character;
			Skill = skill;
		}

		public Character Character;
		public Skill Skill;
	}

	public class SkillChannelEventArgs : EventArgs
	{
		public SkillChannelEventArgs (Character character, Skill skill)
		{
			Character = character;
			Skill = skill;
		}

		public Character Character;
		public Skill Skill;
	}

	public class GameEngine
	{
		GameState _state;
		public GameState CurrentState 
		{
			get => _state;
			private set
			{
				_state = value;
				_state.InvalidateResolvers ();
			}
		}

		ICharacterBehavior CharacterBehavior;
		IEffectEngine EffectEngine;
		ISkillEngine SkillEngine;

		public GameEngine (GameState initialState, ICharacterBehavior behavior, ISkillEngine skillEngine, IEffectEngine effectEngine)
		{
			CurrentState = initialState;
			CharacterBehavior = behavior;
			SkillEngine = skillEngine;
			EffectEngine = effectEngine;
		}

		// Hack for console 
		public void LoadState (GameState state)
		{
			CurrentState = state;
		}

		public event EventHandler<DelayedActionFiredEventArgs> DelayedActions;
		public event EventHandler<SkillChannelEventArgs> SkillChannelStarted;
		public event EventHandler<SkillChannelEventArgs> SkillChannelEnded;
		public event EventHandler<SkillUsedEventArgs> SkillUsed;

		public bool BlockedOnActive => CurrentState.AllCharacters.Any (x => x.ID == CurrentState.ActivePlayerID && Time.IsReady (x));

		public bool Process ()
		{
			if (BlockedOnActive)
				return false;

			IncrementTime ();

			foreach (Character c in CurrentState.AllCharacters.Where (CanTakeNonActiveTurn))
				ApplyNonActiveCharacterTurn (CharacterResolver.Create (c, CurrentState));
			foreach (DelayedAction e in CurrentState.DelayedActions.Where (x => Time.IsReady (x)))
				ApplyDelayedAction (new DelayedActionResolver (e, CurrentState));

			return true;
		}

		bool CanTakeNonActiveTurn (Character c)
		{
			return c.ID != CurrentState.ActivePlayerID && c.IsAlive && Time.IsReady (c);
		}

		public void ProcessActivePlayerAction (TargettedSkill skill)
		{
			if (!BlockedOnActive)
				throw new InvalidOperationException ($"Attempted to use skill {skill.Skill.ID} when not blocked on active");

			ApplySkill (CharacterResolver.Create (CurrentState.ActivePlayerID, CurrentState), skill);
		}

		void IncrementTime ()
		{
			CurrentState = CurrentState.WithTick (CurrentState.Tick + 1);
			foreach (Character c in CurrentState.AllCharacters.Where (x => x.IsAlive))
				CurrentState = CurrentState.UpdateCharacter (Time.Increment (c));
			foreach (DelayedAction a in CurrentState.DelayedActions)
				CurrentState = CurrentState.UpdateDelayedAction (Time.Increment (a));
		}

		void ApplyDelayedAction (DelayedAction e)
		{
			CurrentState = EffectEngine.Apply (e.TargetAction, CurrentState);

			// TODO - This needs to be a UI friendly processed representation of the action
			if (e.SourceSkill != null)
			{
				Character invoker = CurrentState.AllCharacters.WithID (e.SourceSkill.TargetInfo.InvokerID);
				SkillChannelEnded?.Invoke (this, new SkillChannelEventArgs (invoker, e.SourceSkill.Skill));
			}
			else
			{
				DelayedActions?.Invoke (this, new DelayedActionFiredEventArgs (e));
			}
			CurrentState = CurrentState.WithDelayedActions (CurrentState.DelayedActions.Remove (e));
		}

		void ApplyNonActiveCharacterTurn (ItemResolver<Character> c)
		{
			TargettedSkill skillToUse = CharacterBehavior.Act (CurrentState, c);
			ApplySkill (c, skillToUse);
		}

		void ApplySkill (ItemResolver<Character> c, TargettedSkill skillToUse)
		{
			CurrentState = CurrentState.UpdateCharacter (Time.SpendAction (c));
			c.Update (CurrentState);

			if (skillToUse != null)
			{
				CurrentState = SkillEngine.ApplyTargettedSkill (skillToUse, CurrentState);
				c.Update (CurrentState);

				if (skillToUse.Skill.Delay > 0)
					SkillChannelStarted?.Invoke (this, new SkillChannelEventArgs (c, skillToUse.Skill));
				else
					SkillUsed?.Invoke (this, new SkillUsedEventArgs (c, skillToUse.Skill));
			}
		}

		public static GameState Reset (GameState state)
		{
			state = state.WithEnemies (null);
			state = state.WithDelayedActions (null);
			foreach (Character c in state.Party)
				state = state.UpdateCharacter (c.WithCT (0).WithHealth (c.Health.WithFull ()).WithStatusEffects (null).WithResetSkills ());
			return state;
		}
	}
}
