using System.Collections.Immutable;

namespace Morris.Moxy
{
	public readonly struct ValidatedResult<T> : IEquatable<ValidatedResult<T>>
	{
		public readonly bool Success;
		public readonly ImmutableArray<CompilationError> CompilationErrors;
		public readonly T Value;

		public ValidatedResult(T value)
			: this(value, ImmutableArray<CompilationError>.Empty)
		{
		}

		public ValidatedResult(T value, CompilationError compilationError)
			: this(value, Enumerable.Repeat(compilationError, 1).ToImmutableArray())
		{
		}

		public ValidatedResult(T value, ImmutableArray<CompilationError> compilationErrors)
		{
			Success = compilationErrors.Length == 0;
			Value = value;
			CompilationErrors = compilationErrors;
		}

		public override bool Equals(object? obj) => obj is ValidatedResult<T> other && Equals(other);

		public bool Equals(ValidatedResult<T> other) => 
			Success == other.Success
			&& EqualityComparer<T>.Default.Equals(Value, other.Value)
			&& CompilationErrors.SequenceEqual(other.CompilationErrors);

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Success.GetHashCode();
				hashCode = (hashCode * 397) ^ EqualityComparer<T>.Default.GetHashCode(Value);
				hashCode = (hashCode * 397) ^ CompilationErrors.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(ValidatedResult<T> left, ValidatedResult<T> right) => left.Equals(right);
		public static bool operator !=(ValidatedResult<T> left, ValidatedResult<T> right) => !(left == right);
	}
}
