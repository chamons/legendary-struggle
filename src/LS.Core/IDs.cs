public static class IDs
{
	static long CurrentID = 100;

	public static long Next ()
	{
		CurrentID += 1;
		return CurrentID;
	}
}
