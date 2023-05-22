namespace Morris.Moxy.Metas.Templates;

internal class TemplateInput : IEquatable<TemplateInput>
{
	public readonly string Name;
	public readonly string TypeName;
	public readonly string? DefaultValue;

	public static readonly TemplateInput Empty = new TemplateInput();

	private readonly Lazy<int> CachedHashCode;

	public TemplateInput()
	{
		Name = "";
		TypeName = "";
		DefaultValue = null;
		CachedHashCode = new Lazy<int>(() => typeof(TemplateInput).GetHashCode());
	}

	public TemplateInput(
		string name,
		string typeName,
		string? defaultValue)
	{
		Name = name;
		TypeName = typeName;
		DefaultValue = defaultValue;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, typeName, defaultValue));
	}

	public static bool operator ==(TemplateInput left, TemplateInput right) => left.Equals(right);
	public static bool operator !=(TemplateInput left, TemplateInput right) => !(left == right);
	public override bool Equals(object obj) => obj is TemplateInput other && Equals(other);

	public bool Equals(TemplateInput other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& TypeName == other.TypeName
			&& DefaultValue == other.DefaultValue
		);

	public override int GetHashCode() => CachedHashCode.Value;
}
