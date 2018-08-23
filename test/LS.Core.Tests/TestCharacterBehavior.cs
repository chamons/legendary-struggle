using System;

namespace LS.Core.Tests
{
	public class TestCharacterBehavior : ICharacterBehavior
	{
		public Func <GameState, ItemResolver<Character>, TargettedSkill> TestAction;

		public TestCharacterBehavior ()
		{
			TestAction = (s, c) => null;
		}

		public TestCharacterBehavior (Func <GameState, ItemResolver<Character>, TargettedSkill> testAction)
		{
			TestAction = testAction;
		}

		public TargettedSkill Act (GameState state, ItemResolver<Character> c)
		{
			return TestAction (state, c);
		}
	}
}

