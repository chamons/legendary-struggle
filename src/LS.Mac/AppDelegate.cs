using System;
using AppKit;
using Foundation;
using LS.Platform;

namespace LS.Mac
{
	[Register ("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		ILogger Logger;

		public override void DidFinishLaunching (NSNotification notification)
		{
			ObjCRuntime.Runtime.MarshalManagedException += (sender, args) =>
			{
				Exception e = args.Exception;
				Logger.Log ($"Uncaught exception \"{e.Message}\" with stacktrace:\n {e.StackTrace}. Exiting.", LogMask.All, Servarity.Normal);
				throw e;
			};
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}
	}
}
