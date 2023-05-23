using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class SyntaxNodeGetUsingClausesExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<string> GetUsingClauses(this SyntaxNode node)
	{
		var usingClausesBuilder = ImmutableArray.CreateBuilder<string>();
		SyntaxNode? current = node;
		while (current is not null)
		{
			if (current is CompilationUnitSyntax compilationUnitSyntax)
				usingClausesBuilder.AddRange(GetUsingClauses(compilationUnitSyntax.Usings));

			if (current is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclarationSyntax)
				usingClausesBuilder.AddRange(GetUsingClauses(fileScopedNamespaceDeclarationSyntax.Usings));

			if (current is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
				usingClausesBuilder.AddRange(GetUsingClauses(namespaceDeclarationSyntax.Usings));

			current = current.Parent;
		}
		return usingClausesBuilder.ToImmutableArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static IEnumerable<string> GetUsingClauses(SyntaxList<UsingDirectiveSyntax> usingClauses) =>
		usingClauses
			.Where(x => x?.Name is not null)
			.Select(x => x.ToFullString().Replace("\r", "").Replace("\n", ""));
}
