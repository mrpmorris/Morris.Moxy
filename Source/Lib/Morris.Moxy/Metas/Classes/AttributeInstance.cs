using System.Collections.Immutable;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.Metas.Classes;

internal class AttributeInstance : IEquatable<AttributeInstance>
{
	public readonly string Name;
	public readonly ImmutableArray<KeyValuePair<string, object?>> Arguments;

	private readonly Lazy<int> CachedHashCode;

	public AttributeInstance()
	{
		Name = "";
		Arguments = ImmutableArray<KeyValuePair<string, object?>>.Empty;
		CachedHashCode = new Lazy<int>(() => typeof(AttributeInstance).GetHashCode());
	}

	public AttributeInstance(
		string name,
		ImmutableArray<KeyValuePair<string, object?>> arguments)
	{
		if (name.EndsWith("Attribute"))
			name = name.Substring(0, name.Length - 10);

		int dotIndex = name.LastIndexOf('.');
		if (dotIndex > 0)
			name = name.Substring(dotIndex + 1);

		Name = name;
		Arguments = arguments;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, arguments.GetContentsHashCode()));
	}

	public static bool operator ==(AttributeInstance left, AttributeInstance right) => left.Equals(right);
	public static bool operator !=(AttributeInstance left, AttributeInstance right) => !(left == right);
	public override bool Equals(object obj) => obj is AttributeInstance other && Equals(other);

	public bool Equals(AttributeInstance other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& Arguments.SequenceEqual(other.Arguments)
		);

	public override int GetHashCode() => CachedHashCode.Value;
}
