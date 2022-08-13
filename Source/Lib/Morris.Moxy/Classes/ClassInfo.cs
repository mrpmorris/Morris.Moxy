using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Morris.Moxy.Classes;

public readonly struct ClassInfo : IEquatable<ClassInfo>
{
	public static readonly ClassInfo Empty = new();
	public readonly string Name;
	public readonly string Namespace;
	public readonly ImmutableArray<string> PossibleTemplateNames;

	public ClassInfo(string name, string @namespace, ImmutableArray<string> possibleTemplateNames)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		PossibleTemplateNames = possibleTemplateNames;
	}

	public bool Equals(ClassInfo other) =>
		other.Name == Name
		&& other.Namespace == Namespace
	   && Enumerable.SequenceEqual(other.PossibleTemplateNames, PossibleTemplateNames);
}
