using System;
using System.Linq;

namespace LS.Core
{
	public interface ISkillEngine
	{
		GameState ApplyTargettedSkill (TargettedSkill skill, GameState state);
		GameState Wait (ItemResolver<Character> c, GameState state);
	}

	public class SkillEngine : ISkillEngine
	{
		public IEffectEngine EffectEngine;

		public SkillEngine (IEffectEngine effectEngine)
		{
			EffectEngine = effectEngine;
		}

		Action CooldownAction = new Action (ActionType.Cooldown, 0);

		public GameState ApplyTargettedSkill (TargettedSkill s, GameState state)
		{
			if (!s.Skill.Available)
				throw new InvalidOperationException ($"{s.TargetInfo.InvokerID} tried to use skill {s.Skill.ID} but it is not available for use.");

			AssertContainSkill (s, state);

			if (s.Skill.Cooldown > 0)
				state = ApplyCooldown (s, state);

			state = ApplyCTCost (s, state);

			TargettedAction targetAction = s.CreateAction ();

			if (s.Skill.Delay > 0)
				return state.AddDelayedAction (DelayedAction.Create (targetAction, Time.ActionAmount - s.Skill.Delay, s));
			else
				return EffectEngine.Apply (targetAction, state);
		}

		GameState ApplyCTCost (TargettedSkill s, GameState state)
		{
			Character character = state.AllCharacters.WithID (s.TargetInfo.InvokerID);
			int cost = s.Skill.Delay + Time.ActionAmount;
			return state.UpdateCharacter (Time.SpendAction (character, cost));
		}

		GameState ApplyCooldown (TargettedSkill s, GameState state)
		{
			TargettedAction cooldownAction = CreateCooldownAction (s);
			state = state.AddDelayedAction (DelayedAction.Create (cooldownAction, Time.ActionAmount - s.Skill.Cooldown));

			Character character = state.AllCharacters.WithID (s.TargetInfo.InvokerID);
			return state.UpdateCharacter (character.WithUpdatedSkill (s.Skill.WithAvailable (false)));
		}

		TargettedAction CreateCooldownAction (TargettedSkill s)
		{
			TargettingInfo targettingInfo = new TargettingInfo (s.TargetInfo.InvokerID, s.Skill.ID);
			return new TargettedAction (CooldownAction, targettingInfo);
		}

		public GameState Wait (ItemResolver<Character> c, GameState state)
		{
			return state.UpdateCharacter (Time.SpendAction (c));
		}

		static void AssertContainSkill (TargettedSkill s, GameState state)
		{
			Character invoker = state.AllCharacters.WithID (s.TargetInfo.InvokerID);
			if (!invoker.Skills.Any (x => x.ID == s.Skill.ID))
				throw new InvalidOperationException ($"{invoker.ID} tried to use skill {s.Skill.ID} without having it.");
		}
	}
}
