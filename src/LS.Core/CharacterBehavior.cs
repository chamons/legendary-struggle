namespace LS.Core
{
	public interface ICharacterBehavior
	{
		GameState Act (GameState state, ItemResolver<Character> c);
	}

	public class DefaultCharacterBehavior : ICharacterBehavior
	{
		public GameState Act (GameState state, ItemResolver<Character> c)
		{
			return state;
		}
	}
}

