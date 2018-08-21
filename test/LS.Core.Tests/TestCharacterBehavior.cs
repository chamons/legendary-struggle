using System;

namespace LS.Core.Tests
{
	public class TestCharacterBehavior : ICharacterBehavior
	{
		public Func <GameState, ItemResolver<Character>, GameState> TestAction;

		public TestCharacterBehavior ()
		{
			TestAction = (s, c) => s;
		}

		public TestCharacterBehavior (Func <GameState, ItemResolver<Character>, GameState> testAction)
		{
			TestAction = testAction;
		}

		public GameState Act (GameState state, ItemResolver<Character> c)
		{
			return TestAction (state, c);
		}
	}
}

