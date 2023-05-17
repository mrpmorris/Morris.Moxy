using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class ImmutableArrayExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetContentHashCode<T>(this ImmutableArray<T> instance)
	{
		if (instance.IsDefaultOrEmpty)
			return 0;

		int result = 0;
		unchecked
		{
			for (int i = 0; i < instance.Length; i++)
			{
				object? obj = instance[i];
				int objHashCode = obj?.GetHashCode() ?? 0;
				result = result * 397 ^ objHashCode;
			}
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool SequenceEquals<T>(this ImmutableArray<T> instance, ImmutableArray<T> other) =>
		instance.IsDefaultOrEmpty == other.IsDefaultOrEmpty == true
		|| Enumerable.SequenceEqual(instance, other);
}
