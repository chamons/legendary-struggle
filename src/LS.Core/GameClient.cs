using System;
namespace LS.Core
{
	public static class GameClient
	{
		public static GameEngine Create ()
		{
			CharacterBehavior behavior = new CharacterBehavior (Configuration.Behaviors.LoadDefault ().GetBehaviorsSets ());
			EffectEngine effectEngine = new EffectEngine (new RandomGenerator (), Configuration.ConfigData.LoadDefault ());
			SkillEngine skillEngine = new SkillEngine (effectEngine);

			Configuration.Skills skills = Configuration.Skills.LoadDefault ();
			GameState state = Configuration.InitialState.LoadDefault (skills).CreateInitialBattle ();

			return new GameEngine (state, behavior, skillEngine, effectEngine);
		}
	}
}
