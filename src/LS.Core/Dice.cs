using System;

namespace LS.Core
{
	public interface IRandomGenerator
	{
		int Roll (int min, int max);
	}

	public class RandomGenerator : IRandomGenerator
	{
		Random Random;

		public RandomGenerator ()
		{
			Random = new Random ();
		}

		public RandomGenerator (int seed)
		{
			Random = new Random (seed);
		}

		public int Roll (int min, int max) => Random.Next (min, max);
	}

	public class Dice
	{
		public static Dice Invalid = new Dice (-1, -1, -1);
		public static Dice Zero = new Dice (0, 0, 0);

		public int Rolls { get; }
		public int Faces { get; }
		public int Constant { get; }

		public Dice (int rolls, int faces, int constant = 0)
		{
			Rolls = rolls;
			Faces = faces;
			Constant = constant;
		}

		public Dice Add (Dice other)
		{
			if (other.Equals (Zero))
				return this;
			if (Equals (Zero))
				return other;

			if (Faces != other.Faces)
				throw new InvalidOperationException ($"Can't add dice: {this} + {other}");

			return new Dice (Rolls + other.Rolls, Faces, Constant + other.Constant);
		}

		public override bool Equals(object obj) => (obj is Dice dice) && Equals (dice);
		public bool Equals(Dice other) => (Rolls, Faces, Constant) == (other.Rolls, other.Faces, other.Constant);
		public override int GetHashCode() => (Rolls, Faces, Constant).GetHashCode ();
		public static bool operator ==(Dice left, Dice right) => Equals (left, right);
		public static bool operator !=(Dice left, Dice right) => !Equals (left, right);

		public override string ToString () => $"{Rolls}d{Faces} + {Constant}";
	}

	public static class DiceExtensions
	{
		public static int Roll (this Dice dice, IRandomGenerator random)
		{
			int total = 0;
			for (int i = 0; i < dice.Rolls; i++)
				total += random.Roll (1, dice.Faces);
			return total + dice.Constant;
		}

		public static int MaxRoll (this Dice dice)
		{
			return (dice.Faces * dice.Rolls) + dice.Constant;
		}
	}
}
