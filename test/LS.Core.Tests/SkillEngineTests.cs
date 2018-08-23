using System;
using Xunit;

namespace LS.Core.Tests
{
	public class SkillEngineTests
	{
		[Fact]
		public void SkillsCanApplyEffectDirectly ()
		{
			Skill skill = Factory.TestSkill;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()));

			SkillEngine engine = new SkillEngine (effectEngine);
			state = engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			Assert.Single (effectEngine.ActionsUsed);
			Assert.Contains (effectEngine.ActionsUsed, x => x.Name == skill.Action.Name);
		}

		[Fact]
		public void SkillsCanApplyEffectWithDelay ()
		{
			Skill skill = Factory.TestDelayedSkill;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()));

			SkillEngine engine = new SkillEngine (effectEngine);
			state = engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			Assert.Empty (effectEngine.ActionsUsed);
			Assert.Contains (state.DelayedActions, x => x.TargetAction.Action.Name == skill.Action.Name);
			Assert.Equal (50, state.DelayedActions[0].CT);
		}

		[Fact]
		public void SkillFailsIfInvokerDoesNotHave ()	
		{
			Skill skill = Factory.TestSkill;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			SkillEngine engine = new SkillEngine (effectEngine);

			Assert.Throws<InvalidOperationException> (() => engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state));
		}

		[Fact]
		public void CooldownPreventsReuse ()
		{
			Skill skill = Factory.TestSkillCooldown;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()));

			SkillEngine engine = new SkillEngine (effectEngine);
			state = engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			Assert.Single (state.DelayedActions);
			Assert.Contains (state.DelayedActions, x => x.TargetAction.Action.Type == ActionType.Cooldown);
			Assert.False (state.Party[0].Skills[0].Available);
			Assert.Equal (25, state.DelayedActions[0].CT);

			Assert.Throws<InvalidOperationException> (() => engine.ApplyTargettedSkill (new TargettedSkill (state.Party[0].Skills[0], TargettingInfo.Self (state.Party[0])), state));
		}

		[Fact]
		public void CooldownDisappearsAfterTime ()
		{
			Skill skill = Factory.TestSkillCooldown;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()));

			SkillEngine skillEngine = new SkillEngine (effectEngine);
			state = skillEngine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			GameEngine engine = Factory.CreateGameEngine (state, skillEngine: skillEngine);
			Assert.False (engine.CurrentState.Party[0].Skills[0].Available);

			for (int i = 0 ; i < 100 ; ++i)
				engine.Process ();

			Assert.DoesNotContain (engine.CurrentState.DelayedActions, x => x.TargetAction.Action.Type == ActionType.Cooldown);
			Assert.True (engine.CurrentState.Party[0].Skills[0].Available);
		}
	}
}
