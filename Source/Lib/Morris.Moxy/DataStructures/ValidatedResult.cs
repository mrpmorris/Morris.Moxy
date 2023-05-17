using Morris.Moxy.Extensions;
using Morris.Moxy.Helpers;
using System.Collections.Immutable;

namespace Morris.Moxy;

public readonly struct ValidatedResult<T> : IEquatable<ValidatedResult<T>>
	where T: IEquatable<T>
{
	public readonly bool Success;
	public readonly ImmutableArray<CompilationError> CompilationErrors;
	public readonly T Value;

	private readonly Lazy<int> CachedHashCode;

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
		bool success = compilationErrors.Length == 0;
		Success = success;
		Value = value;
		CompilationErrors = compilationErrors;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(success, value, compilationErrors.GetContentHashCode()));
	}

	public static bool operator ==(ValidatedResult<T> left, ValidatedResult<T> right) => left.Equals(right);
	public static bool operator !=(ValidatedResult<T> left, ValidatedResult<T> right) => !(left == right);

	public override bool Equals(object obj) =>
		obj is ValidatedResult<T> other && Equals(other);

	public bool Equals(ValidatedResult<T> other) =>
		other.Success == Success
		&& EqualityComparer<T?>.Default.Equals(Value, other.Value);

	public override int GetHashCode() => CachedHashCode.Value;

	private int CalculateHashCode() => 
		HashCode.Combine(Success, Value, CompilationErrors.GetContentHashCode());
}
