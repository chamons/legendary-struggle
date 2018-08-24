using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Nett;
using System.Collections.Generic;

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

		public BehaviorInfo [] BehaviorInfos { get; set; }

		public List<BehaviorSet> GetBehaviorsSets ()
		{
			List<BehaviorSet> sets = new List<BehaviorSet> ();
			foreach (var info in BehaviorInfos)
			{
				var behaviorSkills = info.Skills.Select (x => new BehaviorSkill (x.Name, ParseCondition (x.OverrideCondition)));
				sets.Add (new BehaviorSet (new Behavior (behaviorSkills), info.Name));
			}
			return sets;
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
