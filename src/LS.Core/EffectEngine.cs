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
			Character invoker = state.AllCharacters.WithIDOrNull (action.TargetInfo.InvokerID);
			Character target = state.AllCharacters.WithIDOrNull (action.TargetInfo.TargetID);
			if (invoker == null || target == null)
				return state;

			if (!invoker.IsAlive || !target.IsAlive)
				return state;

			switch (action.Type)
			{
				case ActionType.Damage:
					return ApplyDamage (action.Power, CharacterResolver.Create (action.TargetInfo.TargetID, state), state);
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

		GameState ApplyDamage (int power, ItemResolver<Character> character, GameState state)
		{
			int amount = CalculateDamageAmount (power);
			return state.UpdateCharacter (character.Item.WithDeltaCurrentHealth (amount));
		}

		int CalculateLineralScalingValue (int power, double percentScale, double flatScale)
		{
			int powerBonus = power - Config.Effects.PowerBase;
			double powerScaleBonus = ((powerBonus * percentScale) + 100) / 100;
			int powerFlatBonus = (int)Math.Floor (powerBonus * flatScale);
			int diceCount = (int)Math.Floor (10 * powerScaleBonus);
			Dice powerDice = new Dice (diceCount, 3);
			return (powerDice.Roll (Random) * 10) + powerFlatBonus;
		}

		int CalculateHealAmount (int power)
		{
			return CalculateLineralScalingValue (power, Config.Effects.HealingPowerScalePercentagePerPoint, Config.Effects.HealingPowerFlatPerPoint);
		}

		int CalculateDamageAmount(int power)
		{
			int amount = CalculateLineralScalingValue (power, Config.Effects.DamagePowerScalePercentagePerPoint, Config.Effects.DamagePowerFlatPerPoint);
			return amount * -1;
		}
	}
}
