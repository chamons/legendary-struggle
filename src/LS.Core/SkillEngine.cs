using System;
using System.Linq;

namespace LS.Core
{
	public interface ISkillEngine
	{
		GameState ApplyTargettedSkill (TargettedSkill skill, GameState state);
	}

	public class SkillEngine : ISkillEngine
	{
		public IEffectEngine EffectEngine;

		public SkillEngine (IEffectEngine effectEngine)
		{
			EffectEngine = effectEngine;
		}

		Action CooldownAction = new Action ("Cooldown", ActionType.Cooldown, 0);

		public GameState ApplyTargettedSkill (TargettedSkill s, GameState state)
		{
			if (!s.Skill.Available)
				throw new InvalidOperationException ($"{s.TargetInfo.InvokerID} tried to use skill {s.Skill.ID} but it is not available for use.");

			AssertContainSkill (s, state);

			if (s.Skill.Cooldown > 0)
			{
				TargettingInfo targettingInfo = new TargettingInfo (s.TargetInfo.InvokerID, s.Skill.ID);
				TargettedAction cooldownAction = new TargettedAction (CooldownAction, targettingInfo);
				state = state.AddDelayedAction (DelayedAction.Create (cooldownAction, 100 - s.Skill.Cooldown));
				Character character = state.AllCharacters.WithID (s.TargetInfo.InvokerID);
				state = state.UpdateCharacter (character.WithUpdatedSkill (s.Skill.WithAvailable (false)));
			}

			TargettedAction targetAction = s.CreateAction ();

			if (s.Skill.Delay > 0)
				return state.AddDelayedAction (DelayedAction.Create (targetAction, 100 - s.Skill.Delay));
			else
				return EffectEngine.Apply (targetAction, state);
		}

		static void AssertContainSkill (TargettedSkill s, GameState state)
		{
			Character invoker = state.AllCharacters.WithID (s.TargetInfo.InvokerID);
			if (!invoker.Skills.Any (x => x.ID == s.Skill.ID))
				throw new InvalidOperationException ($"{invoker.ID} tried to use skill {s.Skill.ID} without having it.");
		}
	}
}
