using System;
namespace LS.Core
{
	// TODO - These should just be record extensions?
	public static class Time
	{
		public static Character Increment (Character c)
		{
			return c.WithCT (c.CT + 1);
		}

		public static DelayedAction Increment (DelayedAction a)
		{
			return a.WithCT (a.CT + 1);
		}

		public static ITimeable Increment (ITimeable e)
		{
			if (e is Character c)
				return Increment (c);
			if (e is DelayedAction d)
				return Increment (d);
			throw new NotImplementedException ();
		}

		// TODO - This is hard coded in places?
		public static bool IsReady (ITimeable c) => c.CT >= 100;

		// TODO - Same here?
		public static Character SpendAction (Character c)
		{
			return c.WithCT (c.CT - 100);
		}
	}
}
