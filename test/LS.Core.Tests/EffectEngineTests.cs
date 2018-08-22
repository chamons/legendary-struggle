﻿using System;
using Xunit;

using LS.Core;
using LS.Core.Configuration;
using System.Collections.Generic;

namespace LS.Core.Tests
{
	public class EffectEngineTests
	{
		static GameState GetDefaultEffectState ()
		{
			Character invoker = Character.Create (new Health (1, 1));
			Character target = Character.Create (new Health (50, 100));

			return new GameState (0, target.Yield (), invoker.Yield (), null, -1);
		}

		[Fact]
		public void HealSkillHeals ()
		{
			GameState state = GetDefaultEffectState ();
			EffectEngine effectEngine = Factory.EffectEngine;
			state = effectEngine.Apply (new Action ("Heal", TargettingInfo.From (state.Party[0], state.Enemies[0]), ActionType.Heal, 100), state);

			Assert.Equal (100, state.Enemies[0].Health.Current);
		}

		[Fact]
		public void DamageSkillDamages ()
		{
			GameState state = GetDefaultEffectState ();
			EffectEngine effectEngine = Factory.EffectEngine;
			state = effectEngine.Apply (new Action ("Damage", TargettingInfo.From (state.Party[0], state.Enemies[0]), ActionType.Damage, 100), state);

			Assert.Equal (0, state.Enemies[0].Health.Current);
		}

		[Fact]
		public void DelayedEffectApplies ()
		{
			GameState state = GetDefaultEffectState ();
			Action damageAction = new Action ("Damage", TargettingInfo.From (state.Party[0], state.Enemies[0]), ActionType.Damage, 100);
			state = state.WithDelayedActions (DelayedAction.Create (damageAction).WithCT (90).Yield ());

			bool delayedEffectFired = false;
			GameEngine engine = Factory.CreateDefaultGameEngine (state);
			engine.DelayedActions += (o, e) => delayedEffectFired = true;

			for (int i = 0 ; i < 10 ; ++i)
				engine.Process ();

			Assert.Equal (0, engine.CurrentState.Enemies[0].Health.Current);
			Assert.True (delayedEffectFired);
		}
	}
}
