using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Nett;

namespace LS.Core.Configuration
{
	public class Behaviors
	{
		public static Behaviors LoadDefault()
		{
			Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LS.Core.Configuration.Behavior.tml");
			return Toml.ReadStream<Behaviors> (stream);
		}

		public static Behaviors Load(string file)
		{
			return Toml.ReadFile<Behaviors> (file);
		}

		public BehaviorInfo [] BehaviorSets { get; set; }

		public Behavior GetBehaviorSet (string name)
		{
			BehaviorInfo info = BehaviorSets.First (x => x.Name == name);
			var behaviorSkills = info.Skills.Select (x => new BehaviorSkill (x.Name, ParseCondition (x.OverrideCondition)));
			return new Behavior (behaviorSkills);
		}

		GameCondition ParseCondition (string overrideCondition)
		{
			if (overrideCondition == null)
				return GameCondition.None;
			return (GameCondition)Enum.Parse (typeof (GameCondition), overrideCondition);
		}
	}

	public class BehaviorInfo
	{
		public string Name { get; set; }
		public BehaviorSkillInfo[] Skills { get; set; }
	}

	public class BehaviorSkillInfo
	{
		public string Name { get; set; }
		public string OverrideCondition { get; set; }
	}
}
