using System.IO;

namespace LS.Platform
{
	public interface IFileStorage
	{
		string SaveLocation { get; }
		string LogLocation { get; }

		StreamWriter GetLogStream ();

		bool FileExists (string file);
		void SaveFile (string file, byte[] contents);
		byte[] LoadFile (string file);
		void DeleteFile (string file);
	}
}
