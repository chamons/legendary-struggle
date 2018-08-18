using System;
namespace LS.Core
{
	public static class Time
	{
		public static Character Increment (Character c)
		{
			return c.WithCT (c.CT + 1);
		}

		public static bool IsReady (Character c) => c.CT >= 100;

		public static Character SpendAction (Character c)
		{
			return c.WithCT (c.CT - 100);
		}
	}
}
