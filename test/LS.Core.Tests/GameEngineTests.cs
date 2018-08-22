using System;
using Xunit;

using LS.Core;
using System.Collections.Generic;

namespace LS.Core.Tests
{
	public class GameEngineTests
	{
		[Fact]
		public void ProcessingIncrementsTicks ()
		{
			GameEngine engine = Factory.CreateDefaultGameEngine (Factory.EmptyGameState);
			Assert.True (engine.Process ());
			Assert.Equal (1, engine.CurrentState.Tick);
		}
		
		[Fact]
		public void OtherCharactersTakeActions ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Enemies[0].WithCT (50));

			GameEngine engine = Factory.CreateGameEngine (state, new TestCharacterBehavior ((s, c) => {
				acted.Add (c.Item.ID);
				return s;
			}));

			for (int i = 0 ; i < 90 ; ++i)
				Assert.True (engine.Process ());

			Assert.Single (acted);
			Assert.Contains (acted, x => x == engine.CurrentState.Enemies[0].ID);
		}

		[Fact]
		public void DelayedActionTakeActionsAndDisappear ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			Action testAction = new Action ("Test", TargettingInfo.Empty, ActionType.None, 0);
			state = state.WithDelayedActions (DelayedAction.Create (testAction).WithCT (90).Yield ());
			long delayedActionID = state.DelayedActions[0].ID;

			GameEngine engine = Factory.CreateDefaultGameEngine (state);

			engine.DelayedActions += (s, a) => acted.Add (a.ID);

			for (int i = 0 ; i < 10 ; ++i)
				Assert.True (engine.Process ());

			Assert.Single (acted);
			Assert.Contains (acted, x => x == delayedActionID);
			Assert.Empty (engine.CurrentState.DelayedActions);
		}

		[Fact]
		public void BlockedWhenActiveCharacterTurn ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (100));
			state = state.WithActivePlayerID (state.Party[0].ID);

			GameEngine engine = Factory.CreateGameEngine (state, new TestCharacterBehavior ((s, c) => {
				acted.Add (c.Item.ID);
				return s;
			}));

			Assert.False (engine.Process ());

			Assert.Empty (acted);
			Assert.Equal (100, state.Party[0].CT);
			Assert.Equal (0, state.Enemies[0].CT);
		}
	}
}
