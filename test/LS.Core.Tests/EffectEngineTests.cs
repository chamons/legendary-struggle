using System;
using Xunit;

using LS.Core;
using System.Collections.Generic;

namespace LS.Core.Tests
{
	public class EffectEngineTests
	{
		[Fact]
		public void HealSkillHeals ()
		{
			Character invoker = Character.Create (new Health (1, 1));
			Character target = Character.Create (new Health (50, 100));

			GameState state = new GameState (0, target.Yield (), invoker.Yield (), null, -1);
			EffectEngine effectEngine = new EffectEngine ();
			state = effectEngine.Apply (new Action ("Heal", TargettingInfo.From (invoker, target), ActionType.Heal, 100), state);

			Assert.Equal (100, target.Health.Current);
		}


	}
}
