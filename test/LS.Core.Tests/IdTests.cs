using System;
using Xunit;

using LS.Core;

namespace LS.Core.Tests
{
	public class IDTests 
	{
		[Fact]
		public void IdsAreIncremental ()
		{
			Assert.Equal (101, IDs.Next());
			Assert.Equal (102, IDs.Next());
			Assert.Equal (103, IDs.Next());
		}
	}
}
