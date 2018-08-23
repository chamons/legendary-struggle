using System;
using System.Collections.Generic;
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
			foreach (BehaviorSkill behaviorSkill in Behavior.Skills.Where (x => x.OverrideCondition != GameCondition.None))
			{
				if (ConditionFinder.IsConditionTrue (behaviorSkill.OverrideCondition, state, c))
				{
					TargettedSkill targettedSkill = ConsiderSkill (behaviorSkill.SkillName, state, c);
					if (targettedSkill != null)
						return targettedSkill;
				}
			}
			
			foreach (BehaviorSkill behaviorSkill in Behavior.Skills)
			{
				TargettedSkill targettedSkill = ConsiderSkill (behaviorSkill.SkillName, state, c);
				if (targettedSkill != null)
					return targettedSkill;
			}
			return null;
		}

		TargettedSkill ConsiderSkill (string skillName, GameState state, ItemResolver<Character> c)
		{
			Skill skill = c.Item.SkillWithName (skillName);
			if (skill != null && skill.Available)
			{
				TargettingInfo targetting = SelectTarget (skill, state, c);
				if (targetting != TargettingInfo.Empty)
					return new TargettedSkill (skill, targetting);
			}
			return null;
		}

		public TargettingInfo SelectTarget (Skill skill, GameState state, ItemResolver<Character> c)
		{
			bool isEnemy = state.Enemies.Contains (c.Item);
			ImmutableArray<Character> team = isEnemy ? state.Enemies : state.Party;
			ImmutableArray<Character> opposingTeam = isEnemy ? state.Party : state.Enemies;

			// This logic is very greedy, and doesn't consider anything but % health
			if (skill.Action.Type.HasFlag (ActionType.Heal))
				return ConditionFinder.FindBestHealingTarget (state, c);

			if (skill.Action.Type.HasFlag (ActionType.Damage))
				return ConditionFinder.FindBestDamageTarget (state, c);

			return TargettingInfo.Empty;
		}
	}

	public static class ConditionFinder
	{
		public static TargettingInfo FindBestHealingTarget (GameState state, ItemResolver<Character> c)
		{
			Character lowestHealth = state.GetTeammates (c).OrderBy (x => x.Health.Current / (double)x.Health.Max).First ();
			return TargettingInfo.From (c.Item.ID, lowestHealth.ID);
		}

		public static TargettingInfo FindBestDamageTarget (GameState state, ItemResolver<Character> c)
		{
			Character lowestHealth = state.GetOpponents (c).OrderBy (x => x.Health.Current / (double)x.Health.Max).First ();
			return TargettingInfo.From (c.Item.ID, lowestHealth.ID);
		}

		public static bool IsConditionTrue (GameCondition condition, GameState state, ItemResolver<Character> c)
		{
			switch (condition)
			{
				case GameCondition.EnemyHealthLow:
					return state.GetOpponents (c).Any (x => x.Health.IsLow);
				case GameCondition.PartyHealthLow:
					return state.GetTeammates (c).Any (x => x.Health.IsLow);
				default:
					throw new NotImplementedException ($"{condition} in ConditionFinder.IsConditionTrue");
			}
		}
	}
}

