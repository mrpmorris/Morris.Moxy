using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Morris.Moxy.DataStructures;
using System.Collections.Immutable;

namespace Morris.Moxy.Classes;

public static class ClassesSelector
{
	public static IncrementalValuesProvider<ClassInfo> Select(
		SyntaxValueProvider syntaxProvider)
	=>
		syntaxProvider
			.CreateSyntaxProvider(
				predicate: static (typeDeclaration, _) => typeDeclaration is TypeDeclarationSyntax,
				transform: static (syntaxContext, _) => syntaxContext)
			.Select(static (syntaxContext, cancellationToken) =>
			{
				var typeDeclarationSyntax = (TypeDeclarationSyntax)syntaxContext.Node;
				var typeSymbol = (INamedTypeSymbol?)syntaxContext.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken);
				if (typeSymbol is null)
					return ClassInfo.Empty;

				var possibleTemplates =
					typeDeclarationSyntax
						.AttributeLists
						.SelectMany(x => x.Attributes)
						.OfType<AttributeSyntax>()
						.Where(x => syntaxContext.SemanticModel.GetSymbolInfo(x).Symbol is null)
						.Select(x => new AttributeNameAndSyntaxTree(
							name: x.Name.ToFullString(),
							attributeSyntaxTree: x))
						.ToImmutableArray();
				if (possibleTemplates.Length == 0)
					return ClassInfo.Empty;

				string className = typeDeclarationSyntax.Identifier.Text;
				string @namespace = typeSymbol.ContainingNamespace.IsGlobalNamespace
					? string.Empty
					: typeSymbol.ContainingNamespace.ToString();
				var genericParameterNames =
					typeDeclarationSyntax.TypeParameterList?.Parameters.Count > 0
					? typeDeclarationSyntax.TypeParameterList.Parameters.Select(x => x.Identifier.Text).ToImmutableArray()
					: ImmutableArray<string>.Empty;

				return new ClassInfo(
					className: className,
					@namespace: @namespace,
					genericParameterNames: genericParameterNames,
					possibleTemplates: possibleTemplates);
			})
			.Where(x => !x.Equals(ClassInfo.Empty));
}
