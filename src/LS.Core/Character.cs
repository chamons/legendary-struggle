using System;

namespace LS.Core
{
	public partial struct Character
	{
		public static Character Create () => new Character (IDs.Next ()); 
	}
}
