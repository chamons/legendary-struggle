using System.Collections.Generic;
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

	public partial class GameState
	{
		internal void RegisterResolver (CharacterResolver resolver)
		{
			if (ActiveResolvers == null)
				ActiveResolvers = new List<CharacterResolver> ();
			ActiveResolvers.Add (resolver);
		}

		internal void InvalidateResolvers ()
		{
			if (ActiveResolvers != null)
			{
				foreach (var resolver in ActiveResolvers)
					resolver.Invalid = true;
				ActiveResolvers.Clear ();
			}
		}

		public GameState UpdateCharacter (Character newCharacter)
		{
			if (Enemies.Any (x => x.ID == newCharacter.ID))
				return WithEnemies (Enemies.ReplaceWithID (newCharacter));
			else 
				return WithParty (Party.ReplaceWithID (newCharacter));
		}
	}
}
