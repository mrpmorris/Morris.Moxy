using Morris.Moxy.Helpers;

namespace Morris.Moxy.Templates;

public readonly struct TemplateNameAndSource : IEquatable<TemplateNameAndSource>
{
	public string Name { get; }
	public string FilePath { get; }
	public string Source { get; }
	private readonly Lazy<int> CachedHashCode;

	public TemplateNameAndSource(string name, string filePath, string source)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		Source = source ?? throw new ArgumentNullException(nameof(source));
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, filePath, source));
	}

	public override bool Equals(object obj) =>
		obj is TemplateNameAndSource other && Equals(other);

	public bool Equals(TemplateNameAndSource other) =>
		Name == other.Name
		&& FilePath == other.FilePath
		&& Source == other.Source;

	public static bool operator ==(TemplateNameAndSource left, TemplateNameAndSource right) => left.Equals(right);

	public static bool operator !=(TemplateNameAndSource left, TemplateNameAndSource right) => !(left == right);

	public override int GetHashCode() => CachedHashCode.Value;
}
