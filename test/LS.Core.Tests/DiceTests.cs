using System;
using Xunit;

namespace LS.Core.Tests
{
	public class DiceTests
	{
		[Fact]
		public void SimpleAddition ()
		{
			Dice one = new Dice (1, 1);
			Assert.Equal (Dice.Zero, Dice.Zero.Add (Dice.Zero));
			Assert.Equal (one, Dice.Zero.Add (one));
			Assert.Equal (new Dice (5, 3), (new Dice (2, 3)).Add (new Dice (3, 3)));
		}

		[Fact]
		public void InvalidAdditionThrows ()
		{
			Dice first = new Dice (2, 3);
			Dice second = new Dice (3, 4);
			Assert.Throws<InvalidOperationException> (() =>
			{
				Dice value = first.Add (second);
			});
		}

		[Fact]
		public void SmokeTest ()
		{
			RandomGenerator random = new RandomGenerator (42);
			Dice dice = new Dice (3, 4, 2);
			int value = dice.Roll (random);
			Assert.True (value >= 5 && value <= 14);
		}

		[Fact]
		public void MaxRoll ()
		{
			Dice dice = new Dice (3, 4, 2);
			Assert.Equal (14, dice.MaxRoll ());
		}
	}
}
