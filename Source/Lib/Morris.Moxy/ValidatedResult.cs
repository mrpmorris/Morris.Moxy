using System.Collections.Immutable;

namespace Morris.Moxy;

internal readonly struct ValidatedResult<T>
{
	public readonly bool Success;
	public readonly ImmutableArray<CompilationError> CompilationErrors = ImmutableArray<CompilationError>.Empty;
	public readonly T Value;

	public ValidatedResult(T value)
	{
		Success = true;
		Value = value;
	}

	public ValidatedResult(IEnumerable<CompilationError> compilationErrors)
	{
		Success = false;
		Value = default!;
		CompilationErrors = compilationErrors.ToImmutableArray();
	}
}