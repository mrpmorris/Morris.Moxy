using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class ImmutableArrayTGetContentsHashCodeExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetContentsHashCode<T>(this ImmutableArray<T> source)
	{
		if (source.IsDefaultOrEmpty)
			return 0;

		int result = 17;

		unchecked
		{
			for (int i = 0; i < source.Length; i++)
				result = result * 31 + (source[i]?.GetHashCode() ?? 0);
		}
		return result;
	}
}
