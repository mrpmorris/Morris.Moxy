using System.Runtime.CompilerServices;

namespace Morris.Moxy;

internal static class HashCode
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int Combine(params object[] values)
	{
		if (values?.Length < 1)
			return 0;
		
		int result = 17;

		unchecked
		{
			for (int i = 0; i < values!.Length; i++)
				result = result * 31 + (values[i]?.GetHashCode() ?? 0);
		}
		return result;
	}
}
