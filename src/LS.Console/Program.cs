using System;
using System.Text;
using LS.Core;

namespace LS.Console
{
    class Program
    {
		public void Run ()
		{
			GameEngine engine = GameClient.Create ();
			engine.SkillUsed += (o, e) => { PrintState (engine.CurrentState); PrintSkillUsed (e); };
			engine.DelayedActions += (o, e) => PrintDelayedAction (e);
			engine.SkillChannelStarted += (o, e) => { PrintState (engine.CurrentState); PrintChannelStarted (e); };
			engine.SkillChannelEnded += (o, e) => { PrintState (engine.CurrentState); PrintChannelEnded (e); };

			PrintState (engine.CurrentState);
			while (true)
			{
				if (engine.BlockedOnActive)
				{
					string input = ReadLine.Read (CreatePrompt (engine.CurrentState.ActiveCharacter));
					if (input == "q")
						return;
				}
				engine.Process ();
			}
		}

		string CreatePrompt (Character character)
		{
			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < character.Skills.Length; ++i)
				builder.Append ($"{i +1} {character.Skills[i].Action.Name} ");
			builder.Append (" )");
			return builder.ToString ();
		}

		void PrintDelayedAction (DelayedActionFiredEventArgs d)
		{
			System.Console.WriteLine ("Delayed Action: " + d.Action.TargetAction.Action.Name);
		}

		void PrintSkillUsed (SkillUsedEventArgs d)
		{
			System.Console.WriteLine ($"{d.Character.Name} used {d.Skill.Action.Name}.");
		}

		void PrintChannelStarted (SkillChannelEventArgs d)
		{
			System.Console.WriteLine ($"{d.Character.Name} begun channeling {d.Skill.Action.Name}.");
		}

		void PrintChannelEnded (SkillChannelEventArgs d)
		{
			System.Console.WriteLine ($"{d.Character.Name} finished channeling {d.Skill.Action.Name}.");
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
