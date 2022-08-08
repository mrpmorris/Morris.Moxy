namespace Morris.Moxy;

public readonly record struct CompilationError(int Line, string Code, string Message);

internal static class CompilationErrors
{
	public static readonly CompilationError NoClosingHeaderMarkerFound = new(0, "MOXY0001", "No closing header marker found");
	public static readonly CompilationError UnknownOrMalformedHeader = new(0, "MOXY0002", "Unknown or malformed header");
}
