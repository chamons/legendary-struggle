using System.Collections.Generic;
using System.Linq;

namespace LS.Core.Tests
{
	public class TestRandomGenerator : IRandomGenerator 
	{
		List<int> Numbers;

		public TestRandomGenerator (IEnumerable <int> numbers)
		{
			Numbers = numbers.ToList ();
		}

		public int Roll (int min, int max)
		{
			int next = Numbers.First ();
			Numbers.RemoveAt (0);
			return next;
		}
	}
}
