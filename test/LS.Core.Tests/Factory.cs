using System;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	static class Factory
	{
		public static Character Player
		{
			get
			{
				return new Character (42);
			}
		}

		public static GameState DefaultGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), null);
			}
		}
	}
}
