using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Classes;

internal class PropertyMeta : IEquatable<PropertyMeta>
{
	public readonly string Name;
	public readonly string TypeName;
	public readonly string Accessibility;
	public readonly bool HasGetter;
	public readonly bool HasSetter;

	private readonly Lazy<int> CachedHashCode;

	public PropertyMeta()
	{
		Name = "";
		TypeName = "";
		Accessibility = "";
		HasGetter = false;
		HasSetter = false;
		CachedHashCode = new Lazy<int>(() => typeof(PropertyMeta).GetHashCode());
	}

	public PropertyMeta(
		string name,
		string typeName,
		string accessibility,
		bool hasGetter,
		bool hasSetter)
	{
		Name = name;
		TypeName = typeName;
		Accessibility = accessibility;
		HasGetter = hasGetter;
		HasSetter = hasSetter;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			name,
			typeName,
			accessibility,
			hasGetter,
			hasSetter));
	}

	public static bool operator ==(PropertyMeta left, PropertyMeta right) => left.Equals(right);
	public static bool operator !=(PropertyMeta left, PropertyMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is PropertyMeta other && Equals(other);

	public bool Equals(PropertyMeta other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
				  && Name == other.Name
				  && TypeName == other.TypeName
				  && Accessibility == other.Accessibility
				  && HasGetter == other.HasGetter
				  && HasSetter == other.HasSetter
		);

	public override int GetHashCode() => CachedHashCode.Value;
}