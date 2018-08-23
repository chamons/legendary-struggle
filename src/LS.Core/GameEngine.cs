using System;
using System.Linq;

namespace LS.Core
{
	public class DelayedActionFired : EventArgs
	{
		public DelayedAction Action;
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

		public event EventHandler<DelayedAction> DelayedActions;

		public bool BlockedOnActive => CurrentState.AllCharacters.Any (x => x.ID == CurrentState.ActivePlayerID && Time.IsReady (x));

		public bool Process ()
		{
			if (BlockedOnActive)
				return false;

			IncrementTime ();

			foreach (Character c in CurrentState.AllCharacters.Where (x => Time.IsReady (x)))
				ApplyNonActiveCharacterTurn (CharacterResolver.Create (c, CurrentState));
			foreach (DelayedAction e in CurrentState.DelayedActions.Where (x => Time.IsReady (x)))
				ApplyDelayedAction (new DelayedActionResolver (e, CurrentState));

			return true;
		}

		public void ProcessActivePlayerAction (TargettedSkill skill)
		{
			ApplySkill (CharacterResolver.Create (CurrentState.ActivePlayerID, CurrentState), skill);
		}

		void IncrementTime ()
		{
			CurrentState = CurrentState.WithTick (CurrentState.Tick + 1);
			foreach (ITimeable c in CurrentState.AllTimables)
				CurrentState = CurrentState.UpdateTimeable (Time.Increment (c));
		}

		void ApplyDelayedAction (DelayedAction e)
		{
			CurrentState = EffectEngine.Apply (e.TargetAction, CurrentState);

			// TODO - This needs to be a UI friendly processed representation of the action
			DelayedActions?.Invoke (this, e);
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

			CurrentState = SkillEngine.ApplyTargettedSkill (skillToUse, CurrentState);
		}
	}
}
