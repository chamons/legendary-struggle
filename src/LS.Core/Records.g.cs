using System;
using System.Collections.Generic;
using System.Collections.Immutable;

public partial struct Character
{
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
}
