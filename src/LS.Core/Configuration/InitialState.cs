using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Nett;
using System.Collections.Generic;

namespace LS.Core.Configuration
{
	public class InitialState
	{
		public static InitialState LoadDefault (Skills skillConfig)
		{
			Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("LS.Core.Configuration.InitialState.tml");
			InitialState state = Toml.ReadStream<InitialState> (stream);
			state.SkillConfig = skillConfig;
			return state;
		}

		public static InitialState Load (string file, Skills skillConfig)
		{
			InitialState state = Toml.ReadFile<InitialState> (file);
			state.SkillConfig = skillConfig;
			return state;
		}

		Skills SkillConfig;
		public CharacterInfo[] Party { get; set; }
		public BattleInfo [] Battles { get; set; }

		public GameState CreateInitialBattle ()
		{
			var party = Party.Select (x => Create (x)).ToList ();
			var state = new GameState (0, party, null, null, party.First ().ID);
			return LoadBattle (state, 0);
		}

		public GameState LoadBattle (GameState state, int index)
		{
			state = GameEngine.Reset (state);
			var enemies = Battles[index].Characters.Select (x => Create (x));
			return state.WithEnemies (enemies);
		}

		Character Create (CharacterInfo info)
		{
			var skills = ParseSkills (info);
			return Character.Create (info.Name, info.CharacterClass, new Health (info.Health, info.Health)).WithSkills (skills);
		}

		IEnumerable <Skill> ParseSkills (CharacterInfo info)
		{
			List<Skill> skills = new List<Skill> ();
			foreach (string skillName in info.Skills)
			{
				if (skillName.Contains ("|"))
				{
					string[] parts = skillName.Split (new char[] { '|' });
					//skills.Add (SkillConfig.GetSkill (parts[0]).WithName (parts[1]));
					skills.Add (SkillConfig.GetSkill (parts[0]));

				}
				else
				{
					skills.Add (SkillConfig.GetSkill (skillName));
				}
			}
			return skills;

		}
	}

	public class BattleInfo
	{
		public string Name { get; set; }
		public CharacterInfo[] Characters { get; set; }
	}

	public class CharacterInfo
	{
		public string Name { get; set; }
		public string CharacterClass { get; set; }
		public string[] Skills { get; set; }
		public int Health { get; set; }
	}
}
