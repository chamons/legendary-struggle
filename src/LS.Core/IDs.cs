// This is a deep dark corner of state in a mostly stateless system
// However, users should never care about the ID they get as long as 
// it is unique, and keeping it tied to GameState is a nightmare
public class IDs
{
	static object Lock = new object ();
	static long CurrentID = 100;

	public static long Next ()
	{
		lock (Lock)
		{
			CurrentID += 1;
			return CurrentID;
		}
	}
}
