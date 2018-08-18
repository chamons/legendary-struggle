using System.Linq;

namespace LS.Core
{
	public partial struct Character
	{
		public static Character Create () => new Character (IDs.Next ()); 

		public Character WithIncrementedCT ()
		{
			return WithCT (CT + 1);
		}
	}

	public partial struct GameState
	{
		public GameState UpdateCharacter (Character newCharacter)
		{
			if (Enemies.Any (x => x.ID == newCharacter.ID))
				return WithEnemies (Enemies.ReplaceWithID (newCharacter));
			else 
				return WithParty (Party.ReplaceWithID (newCharacter));
		}
	}
}
