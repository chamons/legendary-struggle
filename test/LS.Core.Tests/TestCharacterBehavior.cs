using System;

namespace LS.Core.Tests
{
	public class TestCharacterBehavior : ICharacterBehavior
	{
		public Func <GameState, CharacterResolver, GameState> TestAction;

		public TestCharacterBehavior ()
		{
			TestAction = (s, c) => s;
		}

		public TestCharacterBehavior (Func <GameState, CharacterResolver, GameState> testAction)
		{
			TestAction = testAction;
		}

		public GameState Act (GameState state, CharacterResolver c)
		{
			return TestAction (state, c);
		}
	}
}

