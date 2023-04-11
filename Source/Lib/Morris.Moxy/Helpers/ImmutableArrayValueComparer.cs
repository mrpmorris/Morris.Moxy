using System.Collections.Immutable;

namespace Morris.Moxy.Helpers
{
	internal sealed class ImmutableArrayValueComparer<T> : IEqualityComparer<ImmutableArray<T>>
	{
		public static readonly IEqualityComparer<ImmutableArray<T>> Default = new ImmutableArrayValueComparer<T>();

		public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y) => x.SequenceEqual(y, EqualityComparer<T>.Default);

		public int GetHashCode(ImmutableArray<T> array)
		{
			var hashCode = 0;
			foreach (T item in array)
				hashCode = Hash.Combine(hashCode, EqualityComparer<T>.Default.GetHashCode(item!));

			return hashCode;
		}
	}
}
