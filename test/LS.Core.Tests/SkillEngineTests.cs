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
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()).WithCT (Time.ActionAmount));

			SkillEngine engine = new SkillEngine (effectEngine);
			state = engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			Assert.False (state.Party[0].IsCasting);
			Assert.Single (effectEngine.ActionsUsed);
			Assert.Equal (0, state.Party[0].CT);
		}

		[Fact]
		public void SkillsCanApplyEffectWithDelay ()
		{
			Skill skill = Factory.TestDelayedSkill;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()).WithCT (Time.ActionAmount));

			SkillEngine engine = new SkillEngine (effectEngine);
			state = engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			Assert.Empty (effectEngine.ActionsUsed);
			Assert.Contains (state.DelayedActions, x => x.SourceSkill.Skill.CosmeticName == skill.CosmeticName);
			Assert.Equal (50, state.DelayedActions[0].CT);

			Assert.True (state.Party[0].IsCasting);

			CastingInfo castingInfo = state.Party[0].Casting;
			Assert.Equal (state.Party[0].Skills[0], castingInfo.Skill);
			Assert.Equal (skill.Delay, castingInfo.Duration);
			Assert.Equal (0, castingInfo.StartingTick);

			Assert.Equal (-50, state.Party[0].CT);
		}

		[Fact]
		public void SkillCasting_RemovingCasting_WhenComplete ()
		{
			Skill skill = Factory.TestDelayedSkill;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skill.Yield ()).WithCT (Time.ActionAmount));

			SkillEngine skillEngine = new SkillEngine (effectEngine);
			state = skillEngine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])), state);

			Assert.True (state.Party[0].IsCasting);
			GameEngine engine = Factory.CreateGameEngine (state, skillEngine: skillEngine);
			for (int i = 0; i < Time.ActionAmount; ++i)
				engine.Process ();

			Assert.False (engine.CurrentState.Party[0].IsCasting);
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

			for (int i = 0 ; i < Time.ActionAmount; ++i)
				engine.Process ();

			Assert.DoesNotContain (engine.CurrentState.DelayedActions, x => x.TargetAction.Action.Type == ActionType.Cooldown);
			Assert.True (engine.CurrentState.Party[0].Skills[0].Available);
		}
	}
}
