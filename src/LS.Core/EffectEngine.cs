using System;
using LS.Core.Configuration;

namespace LS.Core
{
	public interface IEffectEngine
	{
		GameState Apply(Action action, GameState state);
	}

	public class EffectEngine : IEffectEngine
	{
		IRandomGenerator Random;
		ConfigData Config;

		public EffectEngine (IRandomGenerator random, ConfigData config)
		{
			Random = random;
			Config = config;
		}

		public GameState Apply (Action action, GameState state)
		{
			switch (action.Type)
			{
				case ActionType.Damage:
					return ApplyDamage (action, state);
				case ActionType.Heal:
					return ApplyHeal (action.Power, CharacterResolver.Create (action.TargetInfo.TargetID, state), state);
				case ActionType.None:
					return state;
				default:
					throw new NotImplementedException ($"EffectEngine.Apply with {action.Type}");
			}
		}

		GameState ApplyHeal (int power, ItemResolver<Character> character, GameState state)
		{
			int amount = CalculateHealAmount (power);
			return state.UpdateCharacter (character.Item.WithDeltaCurrentHealth (amount));
		}

		int CalculateHealAmount (int power)
		{
			int powerBonus = power - Config.Effects.PowerBase;
			double powerScaleBonus = ((powerBonus * Config.Effects.HealingPowerScalePercentagePerPoint) + 100) / 100;
			int powerFlatBonus = (int)Math.Floor (powerBonus * Config.Effects.HealingPowerFlatPerPoint);
			int diceCount = (int)Math.Floor (10 * powerScaleBonus);
			Dice powerDice = new Dice (diceCount, 3);
			return (powerDice.Roll (Random) * 10) + powerFlatBonus;
		}

		GameState ApplyDamage (Action action, GameState state)
		{
			return state;
		}
	}
}
