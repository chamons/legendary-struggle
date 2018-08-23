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
		}

		[Fact]
		public void SkillFailsIfInvokerDoesNotHave ()	
		{
			Skill skill = Factory.TestDelayedSkill;
			TestEffectEngine effectEngine = new TestEffectEngine ();

			GameState state = Factory.DefaultGameState;
			SkillEngine engine = new SkillEngine (effectEngine);

			Assert.Throws<InvalidOperationException> (() => engine.ApplyTargettedSkill (new TargettedSkill (skill, TargettingInfo.Empty), state));
		}

		[Fact]
		public void CooldownPreventsReuse ()
		{
			throw new NotImplementedException ();
		}

		[Fact]
		public void CooldownDisappearsAfterTime ()
		{
			throw new NotImplementedException ();
		}

		[Fact]
		public void SkillSkipsIfInvokerIsDead ()
		{
			throw new NotImplementedException ();
		}

		[Fact]
		public void SkillSkipsIfInvokerIsGone ()
		{
			throw new NotImplementedException ();
		}

		[Fact]
		public void SkillSkipsIfTargetIsDead ()
		{
			throw new NotImplementedException ();
		}

		[Fact]
		public void SkillSkipsIfTargetIsMissing ()
		{
			throw new NotImplementedException ();
		}

	}
}
