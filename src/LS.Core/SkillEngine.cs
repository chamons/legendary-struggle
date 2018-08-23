using System;
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

		// This needs to consider cooldown, Available, etc
		public GameState ApplyTargettedSkill (TargettedSkill s, GameState state)
		{
			if (s != null)
			{
				TargettedAction action = s.CreateAction ();
				if (s.Skill.Delay > 0)
					return state.AddDelayedAction (DelayedAction.Create (action));
				else
					return EffectEngine.Apply (action, state);
			}
			return state;
		}
	}
}
