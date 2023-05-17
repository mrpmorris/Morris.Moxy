using Morris.Moxy.Helpers;

namespace Morris.Moxy.Templates;

public readonly struct TemplateAttributeProperty : IEquatable<TemplateAttributeProperty>
{
	public string Name { get; }
	public string TypeName { get; }
	public string? DefaultValue { get; }
	private readonly Lazy<int> CachedHashCode;

	public TemplateAttributeProperty(string name, string typeName, string? defaultValue)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
		DefaultValue = defaultValue;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, typeName, defaultValue));
	}

	public override bool Equals(object obj) =>
		obj is TemplateAttributeProperty other && Equals(other);

	public bool Equals(TemplateAttributeProperty other) =>
		Name == other.Name
		&& TypeName == other.TypeName
		&& DefaultValue == other.DefaultValue;

	public static bool operator ==(TemplateAttributeProperty left, TemplateAttributeProperty right) => left.Equals(right);

	public static bool operator !=(TemplateAttributeProperty left, TemplateAttributeProperty right) => !(left == right);

	public override int GetHashCode() => CachedHashCode.Value;
}

