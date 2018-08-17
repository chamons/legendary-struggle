using System;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	public class GameEngineTests
	{
		[Fact]
		public void ProcessingIncrementsTicks ()
		{
			GameEngine engine = new GameEngine (Factory.EmptyGameState);
			engine.Process ();
			Assert.Equal (1, engine.CurrentState.Tick);
		}
		
		[Fact]
		public void CharactersTakeActions ()
		{
		}
	}
}
