using System;
using System.Collections.Generic;
using System.Linq;

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

	public class TestSkillEngine : ISkillEngine
	{
		public Func<TargettedSkill, GameState, GameState> TestAction;

		public TestSkillEngine ()
		{
			TestAction = (skill, state) => state;
		}

		public TestSkillEngine (Func<TargettedSkill, GameState, GameState> testAction)
		{
			TestAction = testAction;
		}

		public GameState ApplyTargettedSkill (TargettedSkill skill, GameState state)
		{
			return TestAction (skill, state);
		}
	}


	public class TestRandomGenerator : IRandomGenerator
	{
		List<int> Numbers;

		public TestRandomGenerator(IEnumerable<int> numbers)
		{
			Numbers = numbers.ToList ();
		}

		public int Roll(int min, int max)
		{
			int next = Numbers.First ();
			Numbers.RemoveAt (0);
			return next;
		}
	}
}

