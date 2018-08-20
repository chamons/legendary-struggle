namespace LS.Core
{
	public interface ICharacterBehavior
	{
		GameState Act (GameState state, CharacterResolver c);
	}

	public class DefaultCharacterBehavior : ICharacterBehavior
	{
		public GameState Act (GameState state, CharacterResolver c)
		{
			return state;
		}
	}
}

