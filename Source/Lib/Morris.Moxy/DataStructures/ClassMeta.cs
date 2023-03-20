using System.Collections.Immutable;

namespace Morris.Moxy.DataStructures;

public readonly struct ClassMeta
{
	public readonly Type Type;
	public readonly string Namespace;
	public readonly string Name;

	public ClassMeta(Type type, string @namespace, string name)
	{
		Type = type ?? throw new ArgumentNullException(nameof(type));
		Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		Name = name ?? throw new ArgumentNullException(nameof(name));
	}
}
