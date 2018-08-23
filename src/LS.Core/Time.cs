﻿using System;
namespace LS.Core
{
	public static class Time
	{
		public static ITimeable Increment (ITimeable e)
		{
			if (e is Character c)
				return Increment (c);
			if (e is DelayedAction d)
				return Increment (d);
			throw new NotImplementedException ();
		}

		static Character Increment(Character c)
		{
			return c.WithCT (c.CT + 1);
		}

		static DelayedAction Increment(DelayedAction a)
		{
			return a.WithCT (a.CT + 1);
		}

		public const int ActionAmount = 100;

		public static bool IsReady (ITimeable c) => c.CT >= ActionAmount;

		public static Character SpendAction (Character c)
		{
			return c.WithCT (c.CT - ActionAmount);
		}
	}
}
