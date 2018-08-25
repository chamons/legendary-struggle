using System;
using System.IO;
using System.Reflection;
using System.Linq;
using Nett;

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
			var skills = info.Skills.Select (x => SkillConfig.GetSkill (x));

			return Character.Create (info.Name, new Health (info.Health, info.Health)).WithSkills (skills);
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
		public string[] Skills { get; set; }
		public int Health { get; set; }
	}
}
