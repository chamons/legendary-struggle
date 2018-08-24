using System;
using System.Collections.Generic;
using System.Linq;

namespace LS.Core.Tests
{
	public class TestCharacterBehavior : ICharacterBehavior
	{
		public HashSet<Character> CharactersActed = new HashSet<Character> ();
		public TargettedSkill SkillToReturn = null;

		public TargettedSkill Act (GameState state, ItemResolver<Character> c)
		{
			CharactersActed.Add (c.Item);
			return SkillToReturn;
		}
	}

	public class TestEffectEngine : IEffectEngine
	{
		public HashSet<Action> ActionsUsed = new HashSet<Action> ();

		public GameState Apply(TargettedAction targetAction, GameState state)
		{
			ActionsUsed.Add (targetAction.Action);
			return state;
		}
	}

	public class TestSkillEngine : ISkillEngine
	{
		public HashSet<Skill> SkillsUsed = new HashSet<Skill> ();

		public GameState ApplyTargettedSkill (TargettedSkill skill, GameState state)
		{
			SkillsUsed.Add (skill.Skill);
			return state;
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

