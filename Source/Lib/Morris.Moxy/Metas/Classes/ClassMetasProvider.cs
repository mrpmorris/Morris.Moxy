﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Morris.Moxy.Extensions;
using Morris.Moxy.Metas.ProjectInformation;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Metas.Classes;

internal static class ClassMetasProvider
{
	public static IncrementalValuesProvider<ClassMeta> CreateClassMetasProvider
	(
		this IncrementalGeneratorInitializationContext context,
		IncrementalValueProvider<ProjectInformationMeta> projectInformationProvider
	)
	=>
		context
		.SyntaxProvider
		.CreateSyntaxProvider
		(
			predicate: static (syntaxNode, _) => syntaxNode is TypeDeclarationSyntax,
			transform: static (syntaxContext, _) => syntaxContext
		)
		.Select(CreateClassMeta)
		.Where(static(x) => x != ClassMeta.Empty)
		.Combine(projectInformationProvider)
		.Select(static(x, _) =>
			x.Left.Namespace != string.Empty
			? x.Left
			: x.Left.WithNamespace(x.Right.Namespace)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ClassMeta CreateClassMeta(GeneratorSyntaxContext context, CancellationToken cancellationToken)
	{
		var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;
		var typeSymbol = (INamedTypeSymbol?)context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken);
		if (typeSymbol is null) return ClassMeta.Empty;

		ImmutableArray<AttributeInstance> possibleTemplates = typeDeclarationSyntax
			.AttributeLists
			.SelectMany(x => x.Attributes)
			.Where(x => context.SemanticModel.GetDeclaredSymbol(x, cancellationToken) is null)
			.Select(x => 
				new AttributeInstance(
					name: x.Name.ToFullString(),
					arguments: x.GetArgumentKeyValuePairs(context.SemanticModel)))
			.ToImmutableArray();
		if (possibleTemplates.Length == 0) return ClassMeta.Empty;

		string className = typeDeclarationSyntax.Identifier.Text;
		string @namespace = typeSymbol.ContainingNamespace.ToFullString();
		var genericParameterNames = 
			(typeDeclarationSyntax.TypeParameterList?.Parameters.Count ?? 0) == 0
			? ImmutableArray<string>.Empty
			: typeDeclarationSyntax
				.TypeParameterList!
				.Parameters
				.Select(x => x.Identifier.Text).ToImmutableArray();

		return new ClassMeta(
			className: className,
			@namespace: @namespace,
			genericParameterNames: genericParameterNames,
			possibleTemplates: possibleTemplates);
	}


}
