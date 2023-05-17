using Microsoft.CodeAnalysis.CSharp.Syntax;
using Morris.Moxy.Helpers;

namespace Morris.Moxy.DataStructures;

public readonly struct AttributeNameAndSyntaxTree : IEquatable<AttributeNameAndSyntaxTree>
{
	public readonly string Name;
	public readonly AttributeSyntax AttributeSyntaxTree;
	private readonly Lazy<int> CachedHashCode;

	public AttributeNameAndSyntaxTree(string name, AttributeSyntax attributeSyntaxTree)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		AttributeSyntaxTree = attributeSyntaxTree ?? throw new ArgumentNullException(nameof(attributeSyntaxTree));
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(name, attributeSyntaxTree));
	}

	public bool Equals(AttributeNameAndSyntaxTree other) =>
		other.Name == Name
		&& other.AttributeSyntaxTree.Equals(AttributeSyntaxTree);

	public override bool Equals(object obj) =>
		obj is AttributeNameAndSyntaxTree other && Equals(other);

	public override int GetHashCode() => CachedHashCode.Value;

	public static bool operator ==(AttributeNameAndSyntaxTree left, AttributeNameAndSyntaxTree right) => left.Equals(right);

	public static bool operator !=(AttributeNameAndSyntaxTree left, AttributeNameAndSyntaxTree right) => !(left == right);
}
