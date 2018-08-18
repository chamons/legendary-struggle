using System;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	public class IDTests 
	{
		[Fact]
		public void IDsIncreaseOverTime ()
		{
			long [] ids = { IDs.Next (), IDs.Next (), IDs.Next () };
			Assert.True (ids[0] < ids [1]);
			Assert.True (ids[1] < ids [2]);
		}
	}
}
