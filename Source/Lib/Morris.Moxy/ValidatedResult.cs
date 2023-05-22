using Morris.Moxy.Metas;
using System.Collections.Immutable;

namespace Morris.Moxy;

internal class ValidatedResult<T>
{
	public readonly string FilePath;
	public readonly bool Success;
	public readonly ImmutableArray<CompilationError> CompilationErrors;
	public readonly T Value;
	public bool Failure => !Success;

	private ValidatedResult(string filePath, T value)
	{
		Success = true;
		Value = value;
		FilePath = filePath;
		CompilationErrors = ImmutableArray<CompilationError>.Empty;
	}

	private ValidatedResult(string filePath, CompilationError compilationError)
		: this (filePath, ImmutableArray.Create(compilationError))
	{
	}

	private ValidatedResult(string filePath, ImmutableArray<CompilationError> compilationErrors)
	{
		FilePath = filePath;
		Success = false;
		Value = default!;
		CompilationErrors = compilationErrors;
	}

	public static implicit operator ValidatedResult<T>((string filePath, T value) args) =>
		new ValidatedResult<T>(args.filePath, args.value);

	public static implicit operator ValidatedResult<T>((string filePath, CompilationError error) args) =>
		new ValidatedResult<T>(args.filePath, args.error);

	public static implicit operator ValidatedResult<T>((string filePath, ImmutableArray<CompilationError> errors) args) =>
		new ValidatedResult<T>(args.filePath, args.errors);
}