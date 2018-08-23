using System;
using Xunit;

namespace LS.Core.Tests
{
	public class CharacterBehaviorTests
	{
		[Fact]
		public void WillChooseFirstAvailableSkill ()
		{
			BehaviorSkill[] behaviorSkills = { new BehaviorSkill ("Damage"), new BehaviorSkill ("Heal") };
			Skill [] skills = { Factory.DamageSkill, Factory.HealSkill };

			Behavior behavior = new Behavior (behaviorSkills);
			CharacterBehavior characterBehavior = new CharacterBehavior (behavior); 

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skills));
			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Damage", targettedSkill.Skill.Action.Name);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Enemies[0].ID, targettedSkill.TargetInfo.TargetID);

			state = state.UpdateCharacter (state.Party[0].WithUpdatedSkill (state.Party[0].Skills[0].WithAvailable (false)));
			targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Heal", targettedSkill.Skill.Action.Name);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.TargetID);
		}

		[Fact]
		public void WillNotChoseNonOwnedSkill()
		{
			BehaviorSkill[] behaviorSkills = { new BehaviorSkill ("Heal"), new BehaviorSkill ("Damage") };
			Skill[] skills = { Factory.DamageSkill };

			Behavior behavior = new Behavior (behaviorSkills);
			CharacterBehavior characterBehavior = new CharacterBehavior (behavior);

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithSkills (skills));
			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Damage", targettedSkill.Skill.Action.Name);
		}

		[Fact]
		public void WillOverrideWhenCondition ()
		{
			BehaviorSkill[] behaviorSkills = { new BehaviorSkill ("Damage"), new BehaviorSkill ("Heal", GameCondition.PartyHealthLow) };
			Skill[] skills = { Factory.DamageSkill, Factory.HealSkill };

			Behavior behavior = new Behavior (behaviorSkills);
			CharacterBehavior characterBehavior = new CharacterBehavior (behavior);

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithHealth (new Health (1, 10)).WithSkills (skills));

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Heal", targettedSkill.Skill.Action.Name);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.TargetID);
		}

		[Fact]
		public void WillChooseReasonableDamageAndHealTargets ()
		{
			throw new NotImplementedException ();
		}
	}
}
