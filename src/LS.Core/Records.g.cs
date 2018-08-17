using System;
using System.Collections.Generic;
using System.Collections.Immutable;

partial struct Character
{
}

partial struct GameState
{
	public ImmutableArray<Character> Enemies { get; }
	public ImmutableArray<Character> Party { get; }

	public GameState (IEnumerable<Character> enemies, IEnumerable<Character> party)
	{
		Enemies = ImmutableArray.CreateRange (enemies ?? Array.Empty<Character> ());
		Party = ImmutableArray.CreateRange (party ?? Array.Empty<Character> ());
	}
}
