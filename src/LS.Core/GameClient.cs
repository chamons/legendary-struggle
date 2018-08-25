using System;
using System.Linq;

namespace LS.Core
{
	public interface IGameClientDelegate
	{
		void OnDelayedAction (DelayedAction action);
		void OnSkillUsed (Character character, Skill skill);
		void OnSkillChannelStarted (Character character, Skill skill);
		void OnSkillChannelEnded (Character character, Skill skill);
		void OnTick (long tick);
		void OnNewLevel (int level);
	}

	public class GameClient
	{
		Configuration.InitialState InitialState;
		GameEngine Engine;
		public IGameClientDelegate Delegate { get; set; }
		int Level = 0;

		public GameState CurrentState => Engine.CurrentState;
		public bool WaitingOnUserInput => Engine.BlockedOnActive;

		public GameClient ()
		{
			CharacterBehavior behavior = new CharacterBehavior (Configuration.Behaviors.LoadDefault ().GetBehaviorsSets ());
			EffectEngine effectEngine = new EffectEngine (new RandomGenerator (), Configuration.ConfigData.LoadDefault ());
			SkillEngine skillEngine = new SkillEngine (effectEngine);

			Configuration.Skills skills = Configuration.Skills.LoadDefault ();
			InitialState = Configuration.InitialState.LoadDefault (skills);
			GameState state = InitialState.CreateInitialBattle ();
			Engine = new GameEngine (state, behavior, skillEngine, effectEngine);
			Engine.DelayedActions += (o, e) => Delegate?.OnDelayedAction (e.Action);
			Engine.SkillUsed += (o, e) => Delegate?.OnSkillUsed (e.Character, e.Skill);
			Engine.SkillChannelStarted += (o, e) => Delegate?.OnSkillChannelStarted (e.Character, e.Skill);
			Engine.SkillChannelEnded += (o, e) => Delegate?.OnSkillChannelEnded (e.Character, e.Skill);
		}

		public void Process ()
		{
			if (!Engine.BlockedOnActive)
			{
				Engine.Process ();
				Delegate?.OnTick (Engine.CurrentState.Tick);
				CheckForNewLevel ();
			}
		}

		public void ProcessAction (int skillIndex, long targetID)
		{
			Skill s = Engine.CurrentState.ActiveCharacter.Skills[skillIndex];
			TargettingInfo info = TargettingInfo.From (Engine.CurrentState.ActivePlayerID, targetID);
			Engine.ProcessActivePlayerAction (new TargettedSkill (s, info));
			CheckForNewLevel ();
		}

		void LoadLevel (GameState state, int level)
		{
			Configuration.Skills skills = Configuration.Skills.LoadDefault ();
			Engine.LoadState (Configuration.InitialState.LoadDefault (skills).LoadBattle (state, level));
			Delegate?.OnNewLevel (level);
		}

		void CheckForNewLevel ()
		{
			if (Engine.CurrentState.Enemies.All (x => !x.IsAlive))
			{
				LoadLevel (Engine.CurrentState, Level + 1);
				Level = (Level + 1).Clamp (0, 2);
			}
			if (!Engine.CurrentState.ActiveCharacter.IsAlive)
			{
				LoadLevel (Engine.CurrentState, 0);
				Level = 0;
			}
		}
	}
}
