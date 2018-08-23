namespace LS.Core
{
	public interface ICharacterBehavior
	{
		TargettedSkill Act (GameState state, ItemResolver<Character> c);
	}

	public class DefaultCharacterBehavior : ICharacterBehavior
	{
		public TargettedSkill Act (GameState state, ItemResolver<Character> c)
		{
			return null;
		}
	}
}

