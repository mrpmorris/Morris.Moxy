namespace Morris.Moxy.Metas;

public readonly record struct CompilationError(int Line, int Column, string Id, string Message);

internal static class CompilationErrors
{
	public static readonly CompilationError NoClosingHeaderMarkerFound = new(0, 0, "MOXY0001", "No closing header marker found");
	public static readonly CompilationError UnknownOrMalformedHeader = new(0, 0, "MOXY0002", "Unknown or malformed header");
	public static readonly CompilationError ScriptCompilationError = new(0, 0, "MOXY0003", "");
	public static readonly CompilationError TemplateNamesMustBeUnique = new(0, 0, "MOXY0004", "Template names must be unique per project");
}
