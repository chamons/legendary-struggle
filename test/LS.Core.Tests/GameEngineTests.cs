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
			GameEngine engine = new GameEngine (Factory.EmptyGameState, new TestCharacterBehavior ());
			Assert.True (engine.Process ());
			Assert.Equal (1, engine.CurrentState.Tick);
		}
		
		[Fact]
		public void CharactersTakeActions ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Enemies[0].WithCT (50));

			GameEngine engine = new GameEngine (state, new TestCharacterBehavior ((s, c) => {
				acted.Add (c.Item.ID);
				return s;
			}));

			for (int i = 0 ; i < 90 ; ++i)
				Assert.True (engine.Process ());

			Assert.Single (acted);
			Assert.Contains (acted, x => x == engine.CurrentState.Enemies[0].ID);
		}

		[Fact]
		public void BlockedWhenActiveCharacterTurn ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (100));
			state = state.WithActivePlayerID (state.Party[0].ID);

			GameEngine engine = new GameEngine (state, new TestCharacterBehavior ((s, c) => {
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
