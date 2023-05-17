using Morris.Moxy.Helpers;
using Scriban;

namespace Morris.Moxy.Templates;

public readonly struct CompiledTemplate : IEquatable<CompiledTemplate>
{
	public readonly string Name;
	public readonly string FilePath;
	public readonly string? TemplateSource;
	public readonly ParsedTemplate? Directives;
	public readonly Template? Template;

	public CompiledTemplate(
		string name,
		string filePath)
		: this(
			name: name,
			filePath: filePath,
			templateSource: null,
			directives: null,
			template: null)
	{
	}

	public CompiledTemplate(
		string name,
		string filePath,
		string? templateSource,
		ParsedTemplate? directives,
		Template? template)
	{
		Name = name;
		FilePath = filePath;
		TemplateSource = templateSource;
		Directives = directives;
		Template = template;
		CachedHashCode = HashCode.Combine(Name, FilePath, TemplateSource);
	}

	public override bool Equals(object obj) =>
		obj is CompiledTemplate other && Equals(other);

	public bool Equals(CompiledTemplate other) =>
		Name == other.Name
		&& FilePath == other.FilePath
		&& TemplateSource == other.TemplateSource;

	private readonly int CachedHashCode;
	public override int GetHashCode() => CachedHashCode;

	public static bool operator ==(CompiledTemplate left, CompiledTemplate right) => left.Equals(right);

	public static bool operator !=(CompiledTemplate left, CompiledTemplate right) => !(left == right);
}

