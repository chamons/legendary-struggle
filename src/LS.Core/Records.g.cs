using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LS.Core
{
	public interface IIdentifiable
	{
		long ID { get; }
	}

	public partial struct Character : IIdentifiable
	{
		public long ID { get; }
		public int CT { get; }
		public bool IsActivePlayer { get; }

		public Character (long id, int ct = 0, bool isActivePlayer = false)
		{
			ID = id;
			CT = ct;
			IsActivePlayer = isActivePlayer;
		}

		public Character WithID (long id)
		{
			return new Character (id, CT, IsActivePlayer);
		}

		public Character WithCT (int ct)
		{
			return new Character (ID, ct, IsActivePlayer);
		}

		public Character WithIsActivePlayer (bool isActivePlayer)
		{
			return new Character (ID, CT, isActivePlayer);
		}
	}

	public partial struct GameState
	{
		public long Tick { get; }
		public ImmutableArray<Character> Enemies { get; }
		public ImmutableArray<Character> Party { get; }

		public GameState (long tick, IEnumerable<Character> enemies, IEnumerable<Character> party)
		{
			Tick = tick;
			Enemies = ImmutableArray.CreateRange (enemies ?? Array.Empty<Character> ());
			Party = ImmutableArray.CreateRange (party ?? Array.Empty<Character> ());
		}

		public GameState WithTick (long tick)
		{
			return new GameState (tick, Enemies, Party);
		}

		public GameState WithEnemies (IEnumerable<Character> enemies)
		{
			return new GameState (Tick, enemies, Party);
		}

		public GameState WithParty (IEnumerable<Character> party)
		{
			return new GameState (Tick, Enemies, party);
		}

		public IEnumerable <Character> AllCharacters
		{
			get
			{
				foreach (Character e in Enemies)
					yield return e;
				foreach (Character p in Party)
					yield return p;
			}
		}
	}
}
