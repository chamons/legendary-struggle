using System;
using Xunit;

namespace LS.Core.Tests
{
	public class CharacterBehaviorTests
	{
		[Fact]
		public void WillChooseFirstAvailableSkill ()
		{
			CharacterBehavior characterBehavior = CreateTestsBehavior (new BehaviorSkill [] { new BehaviorSkill ("Damage"), new BehaviorSkill ("Heal") });
			GameState state = CreateTestState (new Skill [] { Factory.DamageSkill, Factory.HealSkill });

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Damage", targettedSkill.Skill.CosmeticName);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Enemies[0].ID, targettedSkill.TargetInfo.TargetID);

			state = state.UpdateCharacter (state.Party[0].WithUpdatedSkill (state.Party[0].Skills[0].WithAvailable (false)));
			targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Heal", targettedSkill.Skill.CosmeticName);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.TargetID);
		}

		[Fact]
		public void WillNotChoseNonOwnedSkill()
		{
			CharacterBehavior characterBehavior = CreateTestsBehavior (new BehaviorSkill[] { new BehaviorSkill ("Heal"), new BehaviorSkill ("Damage") });
			GameState state = CreateTestState (new Skill[] { Factory.DamageSkill });

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Damage", targettedSkill.Skill.CosmeticName);
		}

		[Fact]
		public void WillOverrideWhenCondition ()
		{
			CharacterBehavior characterBehavior = CreateTestsBehavior (new BehaviorSkill[] { new BehaviorSkill ("Damage"), new BehaviorSkill ("Heal", GameCondition.PartyHealthLow) });
			GameState state = CreateTestState (new Skill[] { Factory.DamageSkill, Factory.HealSkill });

			state = state.UpdateCharacter (state.Party[0].WithHealth (new Health (1, 10)));

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Heal", targettedSkill.Skill.CosmeticName);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.TargetID);
		}

		[Fact]
		public void WillChooseReasonableHealTarget ()
		{
			CharacterBehavior characterBehavior = CreateTestsBehavior (new BehaviorSkill[] { new BehaviorSkill ("Heal") });
 			GameState state = CreateMultiTargetTestState (new Skill [] { Factory.HealSkill });

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Heal", targettedSkill.Skill.CosmeticName);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Party[1].ID, targettedSkill.TargetInfo.TargetID);
		}

		[Fact]
		public void WillChooseReasonableDamageTarget ()
		{
			CharacterBehavior characterBehavior = CreateTestsBehavior (new BehaviorSkill[] { new BehaviorSkill ("Damage") });
			GameState state = CreateMultiTargetTestState (new Skill[] { Factory.DamageSkill });

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Damage", targettedSkill.Skill.CosmeticName);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Enemies[1].ID, targettedSkill.TargetInfo.TargetID);
		}

		[Fact]
		public void AcceptsSkillWithCosmeticName ()
		{
			CharacterBehavior characterBehavior = CreateTestsBehavior (new BehaviorSkill[] { new BehaviorSkill ("Damage") });
			GameState state = CreateMultiTargetTestState (new Skill[] { new Skill (IDs.Next (), "Scratch", "Damage", Factory.DamageAction, true, 0, 0 )});

			TargettedSkill targettedSkill = characterBehavior.Act (state, CharacterResolver.Create (state.Party[0], state));
			Assert.Equal ("Scratch", targettedSkill.Skill.CosmeticName);
			Assert.Equal ("Damage", targettedSkill.Skill.SkillName);
			Assert.Equal (state.Party[0].ID, targettedSkill.TargetInfo.InvokerID);
			Assert.Equal (state.Enemies[1].ID, targettedSkill.TargetInfo.TargetID);
		}

		static CharacterBehavior CreateTestsBehavior(BehaviorSkill[] behaviorSkills)
		{
			BehaviorSet behaviorSet = new BehaviorSet (new Behavior (behaviorSkills), "Player");
			return new CharacterBehavior (behaviorSet.Yield ());
		}

		static GameState CreateTestState(Skill[] skills)
		{
			GameState state = Factory.DefaultGameState;
			return state.UpdateCharacter (state.Party[0].WithSkills (skills));
		}

		static GameState CreateMultiTargetTestState(Skill[] skills)
		{
			Character[] party = { Character.Create ("Player", "Player", new Health (10, 10)).WithSkills (skills), Character.Create ("Player", "Player", new Health (10, 100)) };
			Character[] enemies = { Character.Create ("Enemy", "Enemy", new Health (10, 10)), Character.Create ("Enemy", "Enemy", new Health (5, 50)) };
			return new GameState (0, party, enemies, null, -1, "");
		}
	}
}
