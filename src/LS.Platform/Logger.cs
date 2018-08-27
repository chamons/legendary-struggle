using System;
using System.IO;

namespace LS.Platform
{
	public enum LogMask
	{
		None,
		Engine,
		UI,
		Animation,
		All
	}

	public enum Servarity
	{
		Diagnostic,
		Normal
	}

	public interface ILogger
	{
		LogMask DiagnosticMask { get; set; }
		void Log (string message, LogMask mask, Servarity sevarity = Servarity.Diagnostic);
		void Log (Func<string> messageProc, LogMask mask, Servarity sevarity = Servarity.Diagnostic);
	}

	public class Logger : ILogger
	{
		StreamWriter LogStream;

		public LogMask DiagnosticMask { get; set; } = LogMask.None;

		public Logger (IFileStorage storage)
		{
			LogStream = storage.GetLogStream ();
		}

		public void Log (string message, LogMask mask, Servarity sevarity = Servarity.Diagnostic)
		{
			Log (() => message, mask, sevarity);
		}

		public void Log (Func<string> messageProc, LogMask mask, Servarity sevarity = Servarity.Diagnostic)
		{
			bool shouldWriteToLog = false;
			bool shouldWriteToDebug = false;

			if (sevarity == Servarity.Normal)
			{
				shouldWriteToLog = true;
				shouldWriteToDebug = true;
			}
			else if (mask == DiagnosticMask || DiagnosticMask == LogMask.All)
			{
				shouldWriteToLog = true;
			}

			if (shouldWriteToDebug || shouldWriteToLog)
			{
				string message = messageProc ();

				if (shouldWriteToDebug)
					System.Diagnostics.Debug.WriteLine ($"[{mask}] {message}");
				if (shouldWriteToLog)
					LogStream.WriteLine ($"[{mask}] {message}");
			}
			if (sevarity == Servarity.Normal)
				LogStream.Flush ();
		}
	}
}
