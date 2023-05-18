namespace Morris.Moxy.Metas.Templates;

internal sealed class TemplateSource : IEquatable<TemplateSource>
{
	public readonly string Name = "";
	public readonly string FilePath = "";
	public readonly string Source = "";

	private readonly Lazy<int> CachedHashCode;

	public TemplateSource()
	{
		Name = "";
		FilePath = "";
		Source = "";
		CachedHashCode = new Lazy<int>(() => typeof(TemplateSource).GetHashCode());
	}

	public TemplateSource(string name, string filePath, string source)
	{
		Name = name;
		FilePath = filePath;
		Source = source;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, filePath, source));
	}

	public static bool operator ==(TemplateSource left, TemplateSource right) => left.Equals(right);
	public static bool operator !=(TemplateSource left, TemplateSource right) => !(left == right);
	public override bool Equals(object obj) => obj is TemplateSource other && Equals(other);

	public bool Equals(TemplateSource other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& FilePath == other.FilePath
			&& Source == other.Source
		);

	public override int GetHashCode() => CachedHashCode.Value;
}
