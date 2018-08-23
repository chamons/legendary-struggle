using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Nett;

namespace LS.Core.Configuration
{
	public class Behavior
	{
		public static BehaviorInfo LoadDefault()
		{
			Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LS.Core.Configuration.Behavior.tml");
			return Toml.ReadStream<BehaviorInfo> (stream);
		}

		public static BehaviorInfo Load(string file)
		{
			return Toml.ReadFile<BehaviorInfo> (file);
		}

		public BehaviorInfo [] Behaviors { get; set; }

		public BehaviorSkill GetBehavior (string name)
		{
			BehaviorInfo info = Behaviors.First (x => x.Name == name);
			return new BehaviorSkill ();
		}
	}

	public class BehaviorInfo
	{
		public string Name { get; set; }
		public BehaviorSkillInfo[] Skills { get; set; }
	}

	public class BehaviorSkillInfo
	{
		public string Skill { get; set; }
		public string OverrideCondition { get; set; }
	}
}
