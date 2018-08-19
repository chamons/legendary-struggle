using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace LS.Core
{
	public interface IIdentifiable
	{
		long ID { get; }
	}

	public partial class GameState
	{
		public long Tick { get; }
		public ImmutableArray<Character> Enemies { get; }
		public ImmutableArray<Character> Party { get; }
		public long ActivePlayerID { get; }
		List <CharacterResolver> ActiveResolvers;

		public GameState (long tick, IEnumerable<Character> enemies, IEnumerable<Character> party, long activePlayerID)
		{
			Tick = tick;
			Enemies = ImmutableArray.CreateRange (enemies ?? Array.Empty<Character> ());
			Party = ImmutableArray.CreateRange (party ?? Array.Empty<Character> ());
			ActivePlayerID = activePlayerID;
		}

		public GameState WithTick (long tick)
		{
			return new GameState (tick, Enemies, Party, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithEnemies (IEnumerable<Character> enemies)
		{
			return new GameState (Tick, enemies, Party, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithParty (IEnumerable<Character> party)
		{
			return new GameState (Tick, Enemies, party, ActivePlayerID) { ActiveResolvers = this.ActiveResolvers };
		}

		public GameState WithActivePlayerID (long activePlayerID)
		{
			return new GameState (Tick, Enemies, Party, activePlayerID) { ActiveResolvers = this.ActiveResolvers };
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

	public partial struct Character : IIdentifiable
	{
		public long ID { get; }
		public int CT { get; }

		public Character (long id, int ct = 0)
		{
			ID = id;
			CT = ct;
		}

		public Character WithID (long id)
		{
			return new Character (id, CT);
		}

		public Character WithCT (int ct)
		{
			return new Character (ID, ct);
		}
	}
}
