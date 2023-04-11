namespace Morris.Moxy.Helpers
{
	internal static class Hash
	{
		internal static int Combine(int newKey, int currentKey)
		{
			return unchecked((currentKey * (int)0xA5555529) + newKey);
		}
	}
}
