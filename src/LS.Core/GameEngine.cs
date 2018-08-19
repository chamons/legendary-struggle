using System;
using System.Linq;

namespace LS.Core
{
	public class GameEngine
	{
		public GameState CurrentState { get; private set; }
		ICharacterBehavior CharacterBehavior;

		public GameEngine (GameState initialState, ICharacterBehavior behavior)
		{
			CurrentState = initialState;
			CharacterBehavior = behavior;
		}

		public bool BlockedOnActive => CurrentState.AllCharacters.Any (x => x.IsActivePlayer && Time.IsReady (x));

		public bool Process ()
		{
			if (BlockedOnActive)
				return false;

			CurrentState = CurrentState.WithTick (CurrentState.Tick + 1);

			foreach (Character e in CurrentState.Enemies)
				ProcessCharacter (new CharacterResolver (e, CurrentState));
			foreach (Character p in CurrentState.Party)
				ProcessCharacter (new CharacterResolver (p, CurrentState));

			return true;
		}

		void ProcessCharacter (CharacterResolver c)
		{
			CurrentState = CurrentState.UpdateCharacter (Time.Increment (c));
			c.Update (CurrentState);

			if (Time.IsReady (c))
			{
				CurrentState = CurrentState.UpdateCharacter (Time.SpendAction (c));
				c.Update (CurrentState);

				CurrentState = CharacterBehavior.Act (CurrentState, c);
			}
		}
	}
}
