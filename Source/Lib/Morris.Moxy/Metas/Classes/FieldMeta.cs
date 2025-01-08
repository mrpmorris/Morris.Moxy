using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Classes;

internal class FieldMeta : IEquatable<FieldMeta>
{
	public readonly string Name;
	public readonly string TypeName;
	public readonly string Accessibility;  // public, private, protected, etc.

	private readonly Lazy<int> CachedHashCode;

	public FieldMeta()
	{
		Name = "";
		TypeName = "";
		Accessibility = "";
		CachedHashCode = new Lazy<int>(() => typeof(FieldMeta).GetHashCode());
	}

	public FieldMeta(
		string name,
		string typeName,
		string accessibility)
	{
		Name = name;
		TypeName = typeName;
		Accessibility = accessibility;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			name,
			typeName,
			accessibility));
	}

	public static bool operator ==(FieldMeta left, FieldMeta right) => left.Equals(right);
	public static bool operator !=(FieldMeta left, FieldMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is FieldMeta other && Equals(other);

	public bool Equals(FieldMeta other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& TypeName == other.TypeName
			&& Accessibility == other.Accessibility
		);

	public override int GetHashCode() => CachedHashCode.Value;
}