using System.Collections.Immutable;

namespace Morris.Moxy;

public readonly struct ValidatedResult<T>
{
	public readonly bool Success;
	public readonly ImmutableArray<CompilationError> CompilationErrors;
	public readonly T Value;

	public ValidatedResult(T value)
		: this(value, ImmutableArray.Create<CompilationError>())
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
}
