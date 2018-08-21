using System;

namespace LS.Core
{
	public interface IEffectEngine
	{
	}
	
	public class EffectEngine : IEffectEngine
	{
		public GameState Apply (Action action, GameState state)
		{
			switch (action.Type)
			{
				case ActionType.Damage:
					return ApplyDamage (action, state);
				case ActionType.Heal:
					return ApplyHeal (action, state);
				case ActionType.None:
					return state;
				default:
					throw new NotImplementedException ($"EffectEngine.Apply with {action.Type}");
			}
		}

		GameState ApplyHeal (Action action, GameState state)
		{
			//int power =  
			return state;
		}

		GameState ApplyDamage (Action action, GameState state)
		{
			return state;
		}
	}
}
