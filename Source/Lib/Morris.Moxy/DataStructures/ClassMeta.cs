using Morris.Moxy.Helpers;

namespace Morris.Moxy.DataStructures;

public readonly struct ClassMeta : IEquatable<ClassMeta>
{
	public readonly Type Type;
	public readonly string Namespace;
	public readonly string Name;

	private readonly Lazy<int> CachedHashCode;

	public ClassMeta(Type type, string @namespace, string name)
	{
		Type = type ?? throw new ArgumentNullException(nameof(type));
		Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		Name = name ?? throw new ArgumentNullException(nameof(name));
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(type, @namespace, @name));
	}

	public override int GetHashCode() => CachedHashCode.Value;

	public override bool Equals(object obj) =>
		obj is ClassMeta other && Equals(other);

	public bool Equals(ClassMeta other) =>
		Type == other.Type
		&& Namespace == other.Namespace
		&& Name == other.Name;

	public static bool operator ==(ClassMeta left, ClassMeta right) => left.Equals(right);

	public static bool operator !=(ClassMeta left, ClassMeta right) => !(left == right);
}

