using System;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	static class Factory
	{
		public static Character Enemy => Character.Create (new Health (1, 1));
		public static Character Player => Character.Create (new Health (1, 1));

		public static GameState EmptyGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), null, null, Player.ID);
			}
		}

		public static GameState DefaultGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), Enemy.Yield (), null, Player.ID);
			}
		}

		public static GameEngine CreateDefaultGameEngine (GameState state)
		{
			return new GameEngine (state, new DefaultCharacterBehavior (), new EffectEngine ());
		}

		public static GameEngine CreateGameEngine (GameState state, ICharacterBehavior characterBehavior)
		{
			return new GameEngine (state, characterBehavior, new EffectEngine ());
		}
	}
}
