using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Morris.Moxy.Classes;

public readonly struct AttributeNameAndSyntaxTree : IEquatable<AttributeNameAndSyntaxTree>
{
	public readonly string Name;
	public readonly AttributeSyntax AttributeSyntaxTree;

	public AttributeNameAndSyntaxTree(string name, AttributeSyntax attributeSyntaxTree)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		AttributeSyntaxTree = attributeSyntaxTree ?? throw new ArgumentNullException(nameof(attributeSyntaxTree));
	}

	public bool Equals(AttributeNameAndSyntaxTree other) =>
		other.Name == Name
		&& other.AttributeSyntaxTree == AttributeSyntaxTree;
}
