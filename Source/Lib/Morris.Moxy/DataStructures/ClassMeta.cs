using System.Collections.Immutable;

namespace Morris.Moxy.DataStructures;

public readonly struct ClassMeta
{
	public readonly Type Type;
	public readonly string Namespace;
	public readonly string Name;
	public readonly ImmutableArray<string> Usings;

	public ClassMeta(Type type, string @namespace, string name, ImmutableArray<string> usings)
	{
		Type = type ?? throw new ArgumentNullException(nameof(type));
		Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Usings = usings;
	}
}
