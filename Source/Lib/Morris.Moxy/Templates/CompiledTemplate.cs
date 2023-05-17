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
	private readonly Lazy<int> CachedHashCode;

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
		CachedHashCode = new Lazy<int>(() => typeof(CompiledTemplate).GetHashCode());
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
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, filePath, templateSource, directives));
	}

	public override bool Equals(object obj) =>
		obj is CompiledTemplate other && Equals(other);

	public bool Equals(CompiledTemplate other) =>
		Name == other.Name
		&& FilePath == other.FilePath
		&& TemplateSource == other.TemplateSource
		&& Comparer<ParsedTemplate?>.Equals(Template, other.Template);

	public override int GetHashCode() => CachedHashCode.Value;

	public static bool operator ==(CompiledTemplate left, CompiledTemplate right) => left.Equals(right);

	public static bool operator !=(CompiledTemplate left, CompiledTemplate right) => !(left == right);
}


