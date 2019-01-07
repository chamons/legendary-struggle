using System;
using System.IO;
using System.Linq;
using LS.Platform;

namespace LS.Utilities
{
	class FileStorage : IFileStorage
	{
		const string FolderName = "Legendary Struggle";

		public string SaveLocation
		{
			get
			{
#if __UNIFIED__
				string savedGamePath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library/Application Support/");
#else
				string savedGamePath = System.Environment.ExpandEnvironmentVariables ("%USERPROFILE%\\Saved Games");
#endif
				return Path.Combine (savedGamePath, FolderName, "LS.sav");
			}
		}

		public string LogLocation
		{
			get
			{
#if __UNIFIED__
				string logPath = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.UserProfile), "Library/Application Support/");
#else
				string logPath = System.Environment.ExpandEnvironmentVariables ("%USERPROFILE%\\Saved Games");
#endif
				return Path.Combine (logPath, FolderName, "log.txt");
			}
		}

		public StreamWriter GetLogStream ()
		{
			Directory.CreateDirectory (Path.GetDirectoryName (LogLocation));
			return new StreamWriter (LogLocation, false);
		}

		public byte[] LoadFile (string filename)
		{
			return File.ReadAllBytes (filename);
		}

		public void SaveFile (string filename, byte[] contents)
		{
			Directory.CreateDirectory (Path.GetDirectoryName (filename));
			File.WriteAllBytes (filename, contents);
		}

		public bool FileExists (string filename)
		{
			return File.Exists (filename);
		}

		public void DeleteFile (string filename)
		{
			File.Delete (filename);
		}
	}
}