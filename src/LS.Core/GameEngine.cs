using System;
using System.Linq;

namespace LS.Core
{
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

		public GameEngine (GameState initialState, ICharacterBehavior behavior)
		{
			CurrentState = initialState;
			CharacterBehavior = behavior;
		}

		public bool BlockedOnActive => CurrentState.AllCharacters.Any (x => x.ID == CurrentState.ActivePlayerID && Time.IsReady (x));

		public bool Process ()
		{
			if (BlockedOnActive)
				return false;

			IncrementTime ();

			foreach (Character c in CurrentState.AllCharacters.Where (x => Time.IsReady (x)))
			         TakeAction (new CharacterResolver (c, CurrentState));

			return true;
		}

		public void ProcessAction ()
		{			
		}

		void IncrementTime ()
		{
			CurrentState = CurrentState.WithTick (CurrentState.Tick + 1);
			foreach (Character c in CurrentState.AllCharacters)
				CurrentState = CurrentState.UpdateCharacter (Time.Increment (c));
		}

		void TakeAction (CharacterResolver c)
		{
			CurrentState = CurrentState.UpdateCharacter (Time.SpendAction (c));
			c.Update (CurrentState);

			CurrentState = CharacterBehavior.Act (CurrentState, c);
		}
	}
}
