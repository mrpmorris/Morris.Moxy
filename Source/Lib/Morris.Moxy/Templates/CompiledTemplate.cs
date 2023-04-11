using Scriban;

namespace Morris.Moxy.Templates;

public readonly struct CompiledTemplate : IEquatable<CompiledTemplate>
{
	public readonly string Name;
	public readonly string FilePath;
	public readonly ParsedTemplate? Directives;
	public readonly Template? Template;

	public CompiledTemplate(
		string name,
		string filePath)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		Directives = null;
		Template = null;
	}

	public CompiledTemplate(
		string name,
		string filePath,
		ParsedTemplate? directives,
		Template? template)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		Directives = directives;
		Template = template;
	}

	public override bool Equals(object? obj) => obj is CompiledTemplate other && Equals(other);

	public bool Equals(CompiledTemplate other) =>
		Name == other.Name
		&& FilePath == other.FilePath
		&& EqualityComparer<ParsedTemplate?>.Default.Equals(Directives, other.Directives);

	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = Name.GetHashCode();
			hashCode = (hashCode * 397) ^ FilePath.GetHashCode();
			hashCode = (hashCode * 397) ^ (Directives?.GetHashCode() ?? 0);
			return hashCode;
		}
	}

	public static bool operator ==(CompiledTemplate left, CompiledTemplate right) => left.Equals(right);
	public static bool operator !=(CompiledTemplate left, CompiledTemplate right) => !(left == right);
}
