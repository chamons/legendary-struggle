﻿using System;
using System.Linq;
using System.Text;
using LS.Core;

namespace LS.Console
{
	class Program : IGameClientDelegate
	{
		GameClient Client;

		public void Run ()
		{
			Client = new GameClient ();
			Client.Delegate = this;
			PrintState ();
			while (true)
			{
				if (Client.WaitingOnUserInput)
				{
					string input = ReadLine.Read (CreatePrompt ());
					if (input == "q")
						return;
					if (int.TryParse (input, out int number))
					{
						if (number >= 0 && number < Client.CurrentState.ActiveCharacter.Skills.Length)
						{
							Skill s = Client.CurrentState.ActiveCharacter.Skills[number];
							input = ReadLine.Read ("E, T, M?");

							switch (input)
							{
								case "E":
									Client.ProcessAction (number, Client.CurrentState.Enemies[0].ID);
									break;
								case "T":
									Client.ProcessAction (number, Client.CurrentState.Party[0].ID);
									break;
								case "M":
									Client.ProcessAction (number, Client.CurrentState.Party[1].ID);
									break;
							}
						}
					}
				}
				else
				{
					Client.Process ();
				}
			}
		}

		string CreatePrompt ()
		{
			Character character = Client.CurrentState.ActiveCharacter;
			StringBuilder builder = new StringBuilder ();
			for (int i = 0; i < character.Skills.Length; ++i)
				builder.Append ($"{i} {character.Skills[i].Name} ");
			builder.Append (")");
			return builder.ToString ();
		}

		void PrintState ()
		{
			foreach (var character in Client.CurrentState.AllCharacters)
				System.Console.WriteLine (character);
		}

		static void Main (string[] args)
		{
			(new Program ()).Run ();
		}

		public void OnDelayedAction (DelayedAction action)
		{
		}

		public void OnSkillUsed (Character character, Skill skill)
		{
			System.Console.WriteLine ($"\n{character.Name} used {skill.Name}.");
			PrintState ();
		}

		public void OnSkillChannelStarted (Character character, Skill skill)
		{
			System.Console.WriteLine ($"\n{character.Name} begun channeling {skill.Name}.");
		}

		public void OnSkillChannelEnded (Character character, Skill skill)
		{
			System.Console.WriteLine ($"\n{character.Name} finished channeling {skill.Name}.");
		}

		public void OnTick (long tick)
		{
		}

		public void OnNewLevel (int level)
		{
			System.Console.WriteLine ($"\nStarting Level {level}.");
			PrintState ();
		}
	}
}
