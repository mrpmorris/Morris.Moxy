using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Morris.Moxy.Extensions;

internal static class SyntaxNodeGetUsingClausesExtension
{
	public static ImmutableArray<string> GetUsingClauses(this SyntaxNode node)
	{
		var usingClausesBuilder = ImmutableArray.CreateBuilder<string>();
		SyntaxNode? current = node;
		while (current is not null)
		{
			if (current is CompilationUnitSyntax compilationUnitSyntax)
				usingClausesBuilder.AddRange
				(
					compilationUnitSyntax
					.Usings
					.Where(x => x.Name is not null)
					.Select(x => x.Name!.ToString())
				);

			if (current is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclarationSyntax)
				usingClausesBuilder.AddRange
				(
					fileScopedNamespaceDeclarationSyntax
					.Usings
					.Where(x => x.Name is not null)
					.Select(x => x.Name!.ToString()));

			current = current.Parent;
		}
		return usingClausesBuilder.ToImmutableArray();
	}
}
