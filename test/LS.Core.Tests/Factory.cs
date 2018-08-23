using System;
using Xunit;

using LS.Core;
using LS.Core.Configuration;

namespace LS.Core.Tests
{
	static class Factory
	{
		public static Character Enemy => Character.Create (new Health (1, 1));
		public static Character Player => Character.Create (new Health (1, 1));
		public static EffectEngine EffectEngine => new EffectEngine (new RandomGenerator (), ConfigData.LoadDefault ());
		public static SkillEngine SkillEngine => new SkillEngine (EffectEngine);
	
		public static Skill TestSkill = Skill.Create (new Action ("Test", ActionType.None, 0), 0, 0);
		public static Skill TestDelayedSkill = Skill.Create (new Action ("Test", ActionType.None, 0), 0, 50);
		public static Skill TestSkillCooldown = Skill.Create (new Action ("Test", ActionType.None, 0), 75, 0);

		public static Action HealAction => new Action ("Heal", ActionType.Heal, 100);
		public static Action DamageAction = new Action ("Damage", ActionType.Damage, 100);
		public static Action StatusEffectAction = new Action ("Freeze", ActionType.Effect, 100, "Chilled");

		public static GameState EmptyGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), null, null, Player.ID);
			}
		}

		public static GameState DefaultGameState
		{
			get
			{
				return new GameState (0, Player.Yield (), Enemy.Yield (), null, Player.ID);
			}
		}

		public static GameEngine CreateDefaultGameEngine (GameState state)
		{
			return new GameEngine (state, new DefaultCharacterBehavior (), SkillEngine, EffectEngine);
		}

		public static GameEngine CreateGameEngine (GameState state, ICharacterBehavior characterBehavior = null, ISkillEngine skillEngine = null, IEffectEngine effectEngine = null)
		{
			characterBehavior = characterBehavior ?? new TestCharacterBehavior ();
			effectEngine = effectEngine ?? EffectEngine;
			skillEngine = skillEngine ?? new SkillEngine (effectEngine);
			return new GameEngine (state, characterBehavior, skillEngine, effectEngine);
		}
	}
}
