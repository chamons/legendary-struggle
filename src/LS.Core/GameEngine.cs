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

		public GameEngine (GameState initialState, ICharacterBehavior behavior, IEffectEngine effectEngine)
		{
			CurrentState = initialState;
			CharacterBehavior = behavior;
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
				TakeAction (CharacterResolver.Create (c, CurrentState));
			foreach (DelayedAction e in CurrentState.DelayedActions.Where (x => Time.IsReady (x)))
				TakeAction (new DelayedActionResolver (e, CurrentState));

			return true;
		}

		void IncrementTime ()
		{
			CurrentState = CurrentState.WithTick (CurrentState.Tick + 1);
			foreach (ITimeable c in CurrentState.AllTimables)
				CurrentState = CurrentState.UpdateTimeable (Time.Increment (c));
		}

		void TakeAction (DelayedAction e)
		{
			CurrentState = EffectEngine.Apply (e.Action, CurrentState);

			// TODO - This needs to be a UI friendly processed representation of the action
			DelayedActions (this, e);
			CurrentState = CurrentState.WithDelayedActions (CurrentState.DelayedActions.Remove (e));
		}

		void TakeAction (ItemResolver<Character> c)
		{
			CurrentState = CurrentState.UpdateCharacter (Time.SpendAction (c));
			c.Update (CurrentState);

			CurrentState = CharacterBehavior.Act (CurrentState, c);
		}
	}
}
