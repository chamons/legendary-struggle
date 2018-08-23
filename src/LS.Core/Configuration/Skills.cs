using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Nett;

namespace LS.Core.Configuration
{
	public struct SkillInfo
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string EffectName { get; set; }
		public int Power { get; set; }
		public int Cooldown { get; set; }
		public int Delay { get; set; }
	}

	public class Skills
	{
		public static Skills LoadDefault ()
		{
			Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LS.Core.Configuration.Skills.tml");
			return Toml.ReadStream<Skills> (stream);
		}

		public static Skills Load(string file)
		{
			return Toml.ReadFile<Skills> (file);
		}

		public SkillInfo [] SkillInfo { get; set; }

		public Skill GetSkill (string name)
		{
			SkillInfo info = SkillInfo.First (x => x.Name == name);
			Action action = new Action (name, GetAction (info), info.Power, info.EffectName);
			return Skill.Create (action, info.Cooldown, info.Delay);
		}

		ActionType GetAction (SkillInfo info)
		{
			string [] parts = info.Type.Split (new char [] { '|' });
			ActionType type = ActionType.None;
			foreach (var part in parts)
				type |= (ActionType)Enum.Parse (typeof (ActionType), part);
			return type;
		}
	}
}
