using System;

namespace LS.Core
{
	public class GameEngine
	{
		public GameState CurrentState { get; private set; }

		public GameEngine (GameState initialState)
		{
			CurrentState = initialState;
		}

		public void Process ()
		{
			CurrentState = CurrentState.WithTick (CurrentState.Tick + 1);
		}
	}
}
