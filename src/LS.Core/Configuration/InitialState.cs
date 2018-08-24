using System;
using System.IO;
using System.Reflection;
using Nett;

namespace LS.Core.Configuration
{
	public class InitialState
	{
		public static InitialState LoadDefault()
		{
			Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LS.Core.Configuration.InitialState.tml");
			return Toml.ReadStream<InitialState> (stream);
		}

		public static InitialState Load (string file)
		{
			return Toml.ReadFile<InitialState> (file);
		}

		public CharacterInfo[] Party { get; set; }
		public BattleInfo [] Battles { get; set; }
	}

	public class BattleInfo
	{
		public string Name { get; set; }
		public CharacterInfo[] Characters { get; set; }
	}

	public class CharacterInfo
	{
		public string Name { get; set; }
		public string[] Skills { get; set; }
		public int Health { get; set; }
	}
}
