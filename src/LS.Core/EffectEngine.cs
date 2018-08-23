using System;
using System.Linq;
using LS.Core.Configuration;

namespace LS.Core
{
	public interface IEffectEngine
	{
		GameState Apply (TargettedAction targetAction, GameState state);
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

		public GameState Apply (TargettedAction targettedAction, GameState state)
		{
			TargettingInfo targettingInfo = targettedAction.TargetInfo;
			Character invoker = state.AllCharacters.WithIDOrNull (targettingInfo.InvokerID);
			if (invoker == null || !invoker.IsAlive)
				return state;

			Action action = targettedAction.Action;
			ActionType type = action.Type;
			if (type.HasFlag (ActionType.Damage) || type.HasFlag (ActionType.Heal))
			{
					Character target = state.AllCharacters.WithIDOrNull (targettingInfo.TargetID);
					if (target == null || !target.IsAlive)
						return state;
			}

			if (type.HasFlag (ActionType.Damage))
				state = ApplyDamage (action.Power, CharacterResolver.Create (targettingInfo.TargetID, state), state);

			if (type.HasFlag (ActionType.Heal))
				state = ApplyHeal (action.Power, CharacterResolver.Create (targettingInfo.TargetID, state), state);

			if (type.HasFlag (ActionType.Cooldown))
				state = ApplyCooldown (targettingInfo, state);

			if (type.HasFlag (ActionType.Effect))
				state = ApplyEffect (action.EffectName, action.Power, targettingInfo, state);

			if (type.HasFlag (ActionType.RemoveEffect))
				state = RemoveEffect (action.EffectName, targettingInfo, state);

			return state;
		}

		GameState RemoveEffect (string effectName, TargettingInfo targettingInfo, GameState state)
		{
			Character c = state.AllCharacters.WithID (targettingInfo.TargetID);
			c = c.WithStatusEffects (c.StatusEffects.RemoveAll (x => x.Name == effectName));
			return state.UpdateCharacter (c);
		}

		GameState ApplyEffect (string effectName, int power, TargettingInfo targettingInfo, GameState state)
		{
			var character = CharacterResolver.Create (targettingInfo.TargetID, state);
			if (character.Item.HasEffect (effectName))
				return ExtendEffect (effectName, power, character, state);
			else
				return AddNewEffect (effectName, power, character, state);
		}

		static GameState ExtendEffect (string effectName, int power, ItemResolver<Character> character, GameState state)
		{
			DelayedAction removalAction = state.DelayedActions.First (x => {
				bool isRemoval = x.TargetAction.Action.Type == ActionType.RemoveEffect;
				bool removingCorrectEffect = x.TargetAction.Action.EffectName == effectName;
				bool correctCharacter = x.TargetAction.TargetInfo.TargetID == character.Item.ID;
				return isRemoval && removingCorrectEffect && correctCharacter;
			});
			return state.ExtendDelayedAction (removalAction, -power);
		}

		static GameState AddNewEffect (string effectName, int power, ItemResolver<Character> character, GameState state)
		{
			state = state.UpdateCharacter (character.Item.AddStatusEffect (new StatusEffect (effectName)));
			character.Update (state);

			Action removeAction = new Action (effectName, ActionType.RemoveEffect, 0, effectName);
			TargettedAction removeActionTargetted = new TargettedAction (removeAction, TargettingInfo.Self (character));
			DelayedAction removeEffectAction = DelayedAction.Create (removeActionTargetted, Time.ActionAmount - power);
			return state.AddDelayedAction (removeEffectAction);
		}

		GameState ApplyCooldown (TargettingInfo targettingInfo, GameState state)
		{
			Character c = state.AllCharacters.WithID (targettingInfo.InvokerID);
			c = c.WithUpdatedSkill (c.Skills.WithID (targettingInfo.TargetID).WithAvailable (true));
			return state.UpdateCharacter (c);
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
