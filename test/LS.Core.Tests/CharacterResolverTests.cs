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
			EnemyResolver resolver = new EnemyResolver (state.Enemies[0], state);
			state = state.WithEnemies (null);
			resolver.Update (state);

			Assert.Throws<CharacterNotFoundException> (() => resolver.Item);
		}

		[Fact]
		public void ThrowsWhenReplacedWithAnother ()
		{
			GameState state = Factory.DefaultGameState;
			EnemyResolver resolver = new EnemyResolver (state.Enemies[0], state);
			state = state.WithEnemies (Factory.Enemy.Yield ());
			resolver.Update (state);

			Assert.Throws<CharacterNotFoundException> (() => resolver.Item);
		}

		[Fact]
		public void FindsUpdatedCharacter ()
		{
			GameState state = Factory.DefaultGameState;
			Character character = state.Party[0];
			PartyResolver resolver = new PartyResolver (character, state);

			state = state.WithParty (state.Party.ReplaceWithID (character.WithCT (Time.ActionAmount)));
			resolver.Update (state);

			Assert.Equal (Time.ActionAmount, resolver.Item.CT);
		}

		[Fact]
		public void ThrowsWhenStale ()
		{
			GameState state = Factory.DefaultGameState;
			GameEngine engine = Factory.CreateDefaultGameEngine (state);

			PartyResolver resolver = new PartyResolver (state.Party[0], state);
			engine.Process ();

			Assert.Throws<StaleReferenceException> (() => resolver.Item);
		}
	}
}
