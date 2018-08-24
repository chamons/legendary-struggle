using System;
using System.Linq;
using System.Text;
using LS.Core;

namespace LS.Console
{
    class Program
    {
		public void Run ()
		{
			int level = 0;
			GameEngine engine = GameClient.Create ();
			engine.SkillUsed += (o, e) => { PrintSkillUsed (e); PrintState (engine.CurrentState); };
			engine.DelayedActions += (o, e) => PrintDelayedAction (e);
			engine.SkillChannelStarted += (o, e) => { PrintChannelStarted (e); PrintState (engine.CurrentState); };
			engine.SkillChannelEnded += (o, e) => { PrintChannelEnded (e); PrintState (engine.CurrentState); };

			PrintState (engine.CurrentState);
			while (true)
			{
				if (engine.CurrentState.Enemies.All (x => !x.IsAlive))
				{
					engine.LoadState (GameClient.LoadLevel (engine.CurrentState, level + 1));
					level = (level + 1).Clamp (0, 2);
					System.Console.WriteLine ($"\nLoading Level {level + 1}.");
					PrintState (engine.CurrentState);
				}
				if (!engine.CurrentState.ActiveCharacter.IsAlive)
				{
					engine.LoadState (GameClient.LoadLevel (engine.CurrentState, 0));
					level = 0;
					System.Console.WriteLine ("\nLoading Level 1.");
					PrintState (engine.CurrentState);
				}

				if (engine.BlockedOnActive)
				{
					string input = ReadLine.Read (CreatePrompt (engine.CurrentState.ActiveCharacter));
					if (input == "q")
						return;
					if (int.TryParse (input, out int number))
					{
						if (number >= 0 && number < engine.CurrentState.ActiveCharacter.Skills.Length)
						{
							Skill s = engine.CurrentState.ActiveCharacter.Skills [number];
							input = ReadLine.Read ("E, T, M?");

							Action<long> use = (id) =>
							{
								TargettingInfo info = TargettingInfo.From (engine.CurrentState.ActivePlayerID, id);
								engine.ProcessActivePlayerAction (new TargettedSkill (s, info));

							};

							switch (input)
							{
								case "E":
									use (engine.CurrentState.Enemies[0].ID);
									break;
								case "T":
									use (engine.CurrentState.Party[0].ID);
									break;
								case "M":
									use (engine.CurrentState.Party[1].ID);
									break;
							}
						}
					}
				}
				engine.Process ();
			}
		}

		string CreatePrompt (Character character)
		{
			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < character.Skills.Length; ++i)
				builder.Append ($"{i} {character.Skills[i].Action.Name} ");
			builder.Append (")");
			return builder.ToString ();
		}

		void PrintDelayedAction (DelayedActionFiredEventArgs d)
		{
			System.Console.WriteLine ("Delayed Action: " + d.Action.TargetAction.Action.Name);
		}

		void PrintSkillUsed (SkillUsedEventArgs d)
		{
			System.Console.WriteLine ($"\n{d.Character.Name} used {d.Skill.Action.Name}.");
		}

		void PrintChannelStarted (SkillChannelEventArgs d)
		{
			System.Console.WriteLine ($"\n{d.Character.Name} begun channeling {d.Skill.Action.Name}.");
		}

		void PrintChannelEnded (SkillChannelEventArgs d)
		{
			System.Console.WriteLine ($"\n{d.Character.Name} finished channeling {d.Skill.Action.Name}.");
		}

		void PrintState (GameState state)
		{
			foreach (var character in state.AllCharacters)
				System.Console.WriteLine (character);
		}


        static void Main(string[] args)
        {
			(new Program ()).Run ();
        }
    }
}
