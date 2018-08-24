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
			GameState state = new GameState (0, null, null, null, -1);
			return new GameEngine (state, behavior, skillEngine, effectEngine);
		}
	}
}
