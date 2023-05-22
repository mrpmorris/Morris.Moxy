using Scriban;

namespace Morris.Moxy.Metas.Templates;

internal class CompiledTemplate : IEquatable<CompiledTemplate>
{
	public static readonly CompiledTemplate Error = new CompiledTemplate();
	private readonly static Template ErrorTemplate = Template.Parse("// The mixin template for this script has errors");

	public readonly string Name;
	public readonly string FilePath;
	public readonly ParsedTemplate ParsedTemplate;
	public readonly Template CompiledScript;

	private readonly Lazy<int> CachedHashCode;

	public CompiledTemplate()
	{
		Name = "";
		FilePath = "";
		ParsedTemplate = null!;
		CompiledScript = ErrorTemplate;

		CachedHashCode = new Lazy<int>(() => typeof(CompiledTemplate).GetHashCode());
	}

	public CompiledTemplate(
		string name,
		string filePath,
		ParsedTemplate parsedTemplate,
		Template compiledScript)
	{
		Name = name;
		FilePath = filePath;
		ParsedTemplate = parsedTemplate;
		CompiledScript = compiledScript;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			name,
			filePath,
			parsedTemplate));
	}

	public static bool operator ==(CompiledTemplate left, CompiledTemplate right) => left.Equals(right);
	public static bool operator !=(CompiledTemplate left, CompiledTemplate right) => !(left == right);
	public override bool Equals(object obj) => obj is CompiledTemplate other && Equals(other);

	public bool Equals(CompiledTemplate other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& FilePath == other.FilePath
			&& ParsedTemplate == other.ParsedTemplate
		);

	public override int GetHashCode() => CachedHashCode.Value;
}
