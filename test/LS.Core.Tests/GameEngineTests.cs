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
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Enemies[0].WithCT (50));

			GameEngine engine = Factory.CreateGameEngine (state, new TestCharacterBehavior ((s, c) => {
				acted.Add (c.Item.ID);
				return null;
			}));

			for (int i = 0; i < 90; ++i)
				Assert.True (engine.Process ());

			Assert.Single (acted);
			Assert.Contains (acted, x => x == engine.CurrentState.Enemies[0].ID);
		}

		[Fact]
		public void DelayedActionTakeActionsAndDisappear ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			TargettedAction testAction = new TargettedAction (new Action ("Test", ActionType.None, 0), TargettingInfo.Empty);
			state = state.WithDelayedActions (DelayedAction.Create (testAction).WithCT (90).Yield ());
			long delayedActionID = state.DelayedActions[0].ID;

			GameEngine engine = Factory.CreateDefaultGameEngine (state);

			engine.DelayedActions += (s, a) => acted.Add (a.ID);

			for (int i = 0; i < 10; ++i)
				Assert.True (engine.Process ());

			Assert.Single (acted);
			Assert.Contains (acted, x => x == delayedActionID);
			Assert.Empty (engine.CurrentState.DelayedActions);
		}

		[Fact]
		public void BlockedWhenActiveCharacterTurn ()
		{
			var acted = new HashSet<long> ();

			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (Time.ActionAmount));
			state = state.WithActivePlayerID (state.Party[0].ID);

			GameEngine engine = Factory.CreateGameEngine (state, new TestCharacterBehavior ((s, c) => {
				acted.Add (c.Item.ID);
				return null;
			}));

			Assert.False (engine.Process ());

			Assert.Empty (acted);
			Assert.Equal (Time.ActionAmount, state.Party[0].CT);
			Assert.Equal (0, state.Enemies[0].CT);
		}

		[Fact]
		public void CharacterBehavior_ReturnsSkill_GivenToSkillEngine ()
		{
			GameState state = Factory.DefaultGameState;
			state = state.UpdateCharacter (state.Party[0].WithCT (99));

			Skill skill = Factory.TestSkill;

			ICharacterBehavior characterBehavior = new TestCharacterBehavior ((s, c) => new TargettedSkill (skill, TargettingInfo.Empty));

			TestSkillEngine skillEngine = new TestSkillEngine ();
			GameEngine engine = Factory.CreateGameEngine (state, characterBehavior, skillEngine);
			engine.Process ();

			Assert.Single (skillEngine.SkillsUsed);
			Assert.Contains (skillEngine.SkillsUsed, x => x.ID == skill.ID);
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
			engine.ProcessActivePlayerAction (new TargettedSkill (skill, TargettingInfo.Empty));

			Assert.Single (skillEngine.SkillsUsed);
			Assert.Contains (skillEngine.SkillsUsed, x => x.ID == skill.ID);
			Assert.Equal (0, engine.CurrentState.Party[0].CT);
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
	}
}
