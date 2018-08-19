using System;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	static class Factory
	{
		public static Character Enemy => Character.Create ();
		public static Character Player => Character.Create ();

		public static GameState EmptyGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), null, Player.ID);
			}
		}

		public static GameState DefaultGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), Enemy.Yield (), Player.ID);
			}
		}
	}
}
