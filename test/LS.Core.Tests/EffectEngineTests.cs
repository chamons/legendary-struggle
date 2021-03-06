﻿using System;
using Xunit;

namespace LS.Core.Tests
{
	public class EffectEngineTests
	{
		static GameState GetDefaultEffectState ()
		{
			Character invoker = Character.Create ("Invoker", "Invoker", new Health (1, 1));
			Character target = Character.Create ("Target", "Target", new Health (50, 100));

			return new GameState (0, invoker.Yield (), target.Yield (), null, -1, "");
		}

		[Fact]
		public void HealSkillHeals ()
		{
			GameState state = GetDefaultEffectState ();
			EffectEngine effectEngine = Factory.EffectEngine;
			TargettedAction healAction = new TargettedAction (Factory.HealAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));
			state = effectEngine.Apply (healAction, state);

			Assert.Equal (100, state.Enemies[0].Health.Current);
		}

		[Fact]
		public void DamageSkillDamages ()
		{
			GameState state = GetDefaultEffectState ();
			EffectEngine effectEngine = Factory.EffectEngine;
			TargettedAction damageAction = new TargettedAction (Factory.DamageAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));
			state = effectEngine.Apply (damageAction, state);

			Assert.Equal (0, state.Enemies[0].Health.Current);
		}

		[Fact]
		public void DelayedEffectApplies ()
		{
			GameState state = GetDefaultEffectState ();
			TargettedAction damageAction = new TargettedAction (Factory.DamageAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));
			state = state.WithDelayedActions (DelayedAction.Create (damageAction).WithCT (90).Yield ());

			bool delayedEffectFired = false;
			GameEngine engine = Factory.CreateDefaultGameEngine (state);
			engine.DelayedActions += (o, e) => delayedEffectFired = true;

			for (int i = 0 ; i < 10 ; ++i)
				engine.Process ();

			Assert.Equal (0, engine.CurrentState.Enemies[0].Health.Current);
			Assert.True (delayedEffectFired);
		}

		[Fact]
		public void DelayedEffect_WithSourceDead ()
		{
			TargetLossTestCore (state => state.UpdateCharacter (state.Party[0].WithCurrentHealth (0)), 50);
		}

		[Fact]
		public void DelayedEffect_WithSourceGone ()
		{
			TargetLossTestCore (state => state.WithParty (null), 50);
		}

		[Fact]
		public void DelayedEffect_WithTargetDead()
		{
			TargetLossTestCore (state => state.UpdateCharacter (state.Enemies[0].WithCurrentHealth (0)), 0);
		}

		[Fact]
		public void DelayedEffect_WithTargetGone()
		{
			TargetLossTestCore (state => state.WithEnemies (null), -1);
		}

		static void TargetLossTestCore (Func<GameState, GameState> testProc, int expectedHealth)
		{
			GameState state = GetDefaultEffectState ();
			TargettedAction damageAction = new TargettedAction (Factory.DamageAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));
			state = state.WithDelayedActions (DelayedAction.Create (damageAction).WithCT (90).Yield ());
			state = testProc (state);

			GameEngine engine = Factory.CreateDefaultGameEngine (state);
			for (int i = 0; i < 10; ++i)
				engine.Process ();

			if (expectedHealth != -1)
				Assert.Equal (expectedHealth, engine.CurrentState.Enemies[0].Health.Current);
		}

		[Fact]
		public void StatusEffectSkill_AddsEffect_AndRemovesAfterDuration ()
		{
			GameState state = GetDefaultEffectState ();
			TargettedAction statusAction = new TargettedAction (Factory.StatusEffectAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));

			EffectEngine effectEngine = Factory.EffectEngine;
			state = effectEngine.Apply (statusAction, state);

			Assert.True (state.Enemies[0].HasEffect ("Chilled"));
			Assert.Single (state.DelayedActions);
			Assert.Contains (state.DelayedActions, x => x.TargetAction.Action.Type == ActionType.RemoveEffect && x.TargetAction.Action.EffectName == "Chilled");

			GameEngine engine = Factory.CreateGameEngine (state, effectEngine: effectEngine);
			for (int i = 0; i < Time.ActionAmount; ++i)
				engine.Process ();

			Assert.False (engine.CurrentState.Enemies[0].HasEffect ("Chilled"));
			Assert.Empty (engine.CurrentState.DelayedActions);
		}

		[Fact]
		public void MultipleStatusEffectApplications_ExtendDuration ()
		{
			GameState state = GetDefaultEffectState ();
			TargettedAction statusAction = new TargettedAction (Factory.StatusEffectAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));

			EffectEngine effectEngine = Factory.EffectEngine;
			state = effectEngine.Apply (statusAction, state);
			state = effectEngine.Apply (statusAction, state);

			Assert.True (state.Enemies[0].HasEffect ("Chilled"));
			Assert.Single (state.DelayedActions);
			Assert.True (state.DelayedActions[0].TargetAction.Action.Type == ActionType.RemoveEffect);
			Assert.True (state.DelayedActions[0].TargetAction.Action.EffectName == "Chilled");

			// Two Time.ActionAmount total (100 -> 0 -> -100)
			Assert.True (state.DelayedActions[0].CT == -Time.ActionAmount);
		}

		[Fact]
		public void EffectsCanBeOfMultipleTypes ()
		{
			GameState state = GetDefaultEffectState ();
			Action multiAction = new Action (ActionType.Damage | ActionType.Effect, 200, "Burning");
			TargettedAction targettedAction = new TargettedAction (multiAction, TargettingInfo.From (state.Party[0], state.Enemies[0]));

			EffectEngine effectEngine = Factory.EffectEngine;
			state = effectEngine.Apply (targettedAction, state);

			Assert.Equal (0, state.Enemies[0].Health.Current);
			Assert.True (state.Enemies[0].HasEffect ("Burning"));
		}
	}
}
