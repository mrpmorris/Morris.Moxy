using System.Collections.Immutable;

namespace Morris.Moxy;

internal readonly struct ValidatedResult<T>
{
	public readonly bool Success;
	public readonly ImmutableArray<CompilationError> CompilationErrors = ImmutableArray<CompilationError>.Empty;
	public readonly T Value;

	private ValidatedResult(T value)
	{
		Success = true;
		Value = value;
	}

	private ValidatedResult(CompilationError compilationError)
		: this(ImmutableArray.Create(compilationError))
	{
	}

	private ValidatedResult(ImmutableArray<CompilationError> compilationErrors)
	{
		Success = false;
		Value = default!;
		CompilationErrors = compilationErrors;
	}

	public static implicit operator ValidatedResult<T>(T value) => new ValidatedResult<T>(value);
	public static implicit operator ValidatedResult<T>(CompilationError error) => new ValidatedResult<T>(error);
	public static implicit operator ValidatedResult<T>(ImmutableArray<CompilationError> errors) => new ValidatedResult<T>(errors);
}