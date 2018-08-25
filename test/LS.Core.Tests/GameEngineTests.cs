using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	public class GameEngineTests
	{
		[Fact]
		public void ProcessingIncrementsTicks ()
		{
			GameEngine engine = Factory.CreateDefaultGameEngine (Factory.EmptyGameState);
			Assert.True (engine.Process ());
			Assert.Equal (1, engine.CurrentState.Tick);
		}
		
		[Fact]
		public void OtherCharactersTakeActions ()
		{
			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Enemies[0].WithCT (50));
		
			TestCharacterBehavior behavior = new TestCharacterBehavior ();
			GameEngine engine = Factory.CreateGameEngine (state, behavior);

			for (int i = 0; i < 90; ++i)
				Assert.True (engine.Process ());

			Assert.Single (behavior.CharactersActed);
			Assert.Contains (behavior.CharactersActed.ToList (), x => x.ID == engine.CurrentState.Enemies[0].ID);
		}

		[Fact]
		public void DelayedActionTakeActionsAndDisappear ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			TargettedAction testAction = new TargettedAction (new Action (ActionType.None, 0), TargettingInfo.Empty);
			state = state.WithDelayedActions (DelayedAction.Create (testAction).WithCT (90).Yield ());
			long delayedActionID = state.DelayedActions[0].ID;

			GameEngine engine = Factory.CreateDefaultGameEngine (state);

			engine.DelayedActions += (s, a) => acted.Add (a.Action.ID);

			for (int i = 0; i < 10; ++i)
				Assert.True (engine.Process ());

			Assert.Single (acted);
			Assert.Contains (acted, x => x == delayedActionID);
			Assert.Empty (engine.CurrentState.DelayedActions);
		}

		[Fact]
		public void BlockedWhenActiveCharacterTurn ()
		{
			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (Time.ActionAmount));
			state = state.WithActivePlayerID (state.Party[0].ID);

			TestCharacterBehavior behavior = new TestCharacterBehavior ();
			GameEngine engine = Factory.CreateGameEngine (state, behavior);

			Assert.False (engine.Process ());

			Assert.Empty (behavior.CharactersActed);
			Assert.Equal (Time.ActionAmount, state.Party[0].CT);
			Assert.Equal (0, state.Enemies[0].CT);
		}

		[Fact]
		public void CharacterBehavior_ReturnsSkill_GivenToSkillEngine ()
		{
			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (99));

			Skill skill = Factory.TestSkill;

			TestCharacterBehavior characterBehavior = new TestCharacterBehavior ();
			characterBehavior.SkillToReturn = new TargettedSkill (skill, TargettingInfo.Self (state.Party[0]));

			TestSkillEngine skillEngine = new TestSkillEngine ();
			GameEngine engine = Factory.CreateGameEngine (state, characterBehavior, skillEngine);

			Skill skillReportedUsed = null;
			engine.SkillUsed += (o, s) => skillReportedUsed = s.Skill;
			engine.Process ();

			Assert.Single (skillEngine.SkillsUsed);
			Assert.Contains (skillEngine.SkillsUsed, x => x.ID == skill.ID);
			Assert.Equal (skillReportedUsed, skillEngine.SkillsUsed.First ());
		}

		[Fact]
		public void ActiveCharacterCanUseSkill ()
		{
			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (Time.ActionAmount));
			state = state.WithActivePlayerID (state.Party[0].ID);

			Skill skill = Factory.TestSkill;

			TestSkillEngine skillEngine = new TestSkillEngine ();
			GameEngine engine = Factory.CreateGameEngine (state, skillEngine: skillEngine);
			Skill skillReportedUsed = null;
			engine.SkillUsed += (o, s) => skillReportedUsed = s.Skill;

			engine.ProcessActivePlayerAction (new TargettedSkill (skill, TargettingInfo.Self (state.Party[0])));

			Assert.Single (skillEngine.SkillsUsed);
			Assert.Contains (skillEngine.SkillsUsed, x => x.ID == skill.ID);
			Assert.Equal (skillReportedUsed, skillEngine.SkillsUsed.First ());
		}

		[Fact]
		public void ActivePlayerUsingSkillWhenNonActiveThrows()
		{
			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (50));
			state = state.WithActivePlayerID (state.Party[0].ID);

			Skill skill = Factory.TestSkill;

			TestSkillEngine skillEngine = new TestSkillEngine ();
			GameEngine engine = Factory.CreateGameEngine (state, skillEngine: skillEngine);
			Assert.Throws <InvalidOperationException> (() => engine.ProcessActivePlayerAction (new TargettedSkill (skill, TargettingInfo.Empty)));
		}

		[Fact]
		public void ActivePlayerDoesNotTakeAutomatedAction_WhenCTsAreSynced ()
		{
			Character [] party = { Factory.Player, Factory.Player };
			GameState state = new GameState (0, party, null, null, party[0].ID);

			TestCharacterBehavior behavior = new TestCharacterBehavior ();

			GameEngine engine = Factory.CreateGameEngine (state, behavior);
			for (int i = 0; i < Time.ActionAmount; ++i)
				engine.Process ();

			Assert.Single (behavior.CharactersActed);
			Assert.True (engine.BlockedOnActive);
		}

		[Fact]
		public void DeadCharacters_DontTakeActionsOrCT()
		{
			Character[] party = { Factory.Player, Factory.Player.WithCurrentHealth (0) };
			GameState state = new GameState (0, party, null, null, -1);

			TestCharacterBehavior behavior = new TestCharacterBehavior ();

			GameEngine engine = Factory.CreateGameEngine (state, behavior);
			for (int i = 0; i < Time.ActionAmount; ++i)
				engine.Process ();

			Assert.Single (behavior.CharactersActed);
			Assert.Equal (0, engine.CurrentState.Party[1].CT);
		}
	}
}
