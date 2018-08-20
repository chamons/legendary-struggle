using System;
using Xunit;

namespace LS.Core.Tests
{
	public class CharacterResolverTests
	{
		[Fact]
		public void ThrowsWhenItemRemoved ()
		{
			GameState state = Factory.DefaultGameState;
			CharacterResolver resolver = new CharacterResolver (state.Enemies[0], state);
			state = state.WithEnemies (null);
			resolver.Update (state);

			Assert.Throws<CharacterNotFoundException> (() => resolver.Item);
		}

		[Fact]
		public void ThrowsWhenReplacedWithAnother ()
		{
			GameState state = Factory.DefaultGameState;
			CharacterResolver resolver = new CharacterResolver (state.Enemies[0], state);
			state = state.WithEnemies ((new Character ()).Yield ());
			resolver.Update (state);

			Assert.Throws<CharacterNotFoundException> (() => resolver.Item);
		}

		[Fact]
		public void FindsUpdatedCharacter ()
		{
			GameState state = Factory.DefaultGameState;
			Character character = state.Party[0];
			CharacterResolver resolver = new CharacterResolver (character, state);

			state = state.WithParty (state.Party.ReplaceWithID (character.WithCT (100)));
			resolver.Update (state);

			Assert.Equal (100, resolver.Item.CT);
		}

		[Fact]
		public void ThrowsWhenStale ()
		{
			GameState state = Factory.DefaultGameState;
			GameEngine engine = new GameEngine (state, new TestCharacterBehavior ());

			CharacterResolver resolver = new CharacterResolver (state.Party[0], state);
			engine.Process ();

			Assert.Throws<StaleReferenceException> (() => resolver.Item);
		}
	}
}
