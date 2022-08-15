using System.Collections.Immutable;

namespace Morris.Moxy.Classes;

public readonly struct ClassInfo : IEquatable<ClassInfo>
{
	public static readonly ClassInfo Empty = new();
	public readonly string Name;
	public readonly string Namespace;
	public readonly ImmutableArray<AttributeNameAndSyntaxTree> PossibleTemplates;

	public ClassInfo(
		string name,
		string @namespace,
		ImmutableArray<AttributeNameAndSyntaxTree> possibleTemplates)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		PossibleTemplates = possibleTemplates;
	}

	public bool Equals(ClassInfo other) =>
		other.Name == Name
		&& other.Namespace == Namespace
	   && Enumerable.SequenceEqual(other.PossibleTemplates, PossibleTemplates);
}
