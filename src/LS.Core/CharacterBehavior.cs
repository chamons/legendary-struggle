using System;
using System.Collections.Immutable;
using System.Linq;

namespace LS.Core
{
	public interface ICharacterBehavior
	{
		TargettedSkill Act (GameState state, ItemResolver<Character> c);
	}

	public class CharacterBehavior : ICharacterBehavior
	{
		Behavior Behavior;

		public CharacterBehavior (Behavior behavior)
		{
			Behavior = behavior;
		}

		public TargettedSkill Act (GameState state, ItemResolver<Character> c)
		{
			foreach (BehaviorSkill behaviorSkill in Behavior.Skills)
			{
				Skill skill = c.Item.Skills.FirstOrDefault (x => x.Action.Name == behaviorSkill.SkillName);
				if (skill != null && skill.Available)
				{
					TargettingInfo targetting = SelectTarget (skill, state, c);
					if (targetting != TargettingInfo.Empty)
						return new TargettedSkill (skill, targetting);
				}
			}
			return null;
		}

		public TargettingInfo SelectTarget (Skill skill, GameState state, ItemResolver<Character> c)
		{
			bool isEnemy = state.Enemies.Contains (c.Item);
			ImmutableArray<Character> team = isEnemy ? state.Enemies : state.Party;
			ImmutableArray<Character> opposingTeam = isEnemy ? state.Party : state.Enemies;

			// This is braindead
			if (skill.Action.Type.HasFlag (ActionType.Heal))
			{
				return TargettingInfo.From (c, team[0]);
			}

			if (skill.Action.Type.HasFlag (ActionType.Damage))
			{
				return TargettingInfo.From (c, opposingTeam[0]);
			}

			return TargettingInfo.Empty;
		}

		Character FindLowestHealth (ImmutableArray<Character> characters)
		{
			return characters.OrderBy (x => x.Health.Current).Last ();
		}
	}
}

