using System;
using System.IO;
using System.Reflection;
using Nett;

namespace LS.Core.Configuration
{
	public class ConfigData
	{
		public static ConfigData LoadDefault ()
		{
			Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LS.Core.Configuration.Data.tml");
			return Toml.ReadStream<ConfigData> (stream);
		}

		public static ConfigData Load (string file)
		{
			return Toml.ReadFile<ConfigData> (file);
		}

		public Effects Effects { get; set; }

	}

	public class Effects
	{
		public int PowerBase { get; set; }
		public double HealingPowerScalePercentagePerPoint { get; set; }
		public double DamagePowerScalePercentagePerPoint { get; set; }
		public double HealingPowerFlatPerPoint { get; set; }
		public double DamagePowerFlatPerPoint { get; set; }
	}
}
