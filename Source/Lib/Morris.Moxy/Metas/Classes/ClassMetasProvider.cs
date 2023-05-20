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
		IncrementalValueProvider<ProjectInformationMeta> projectInformationProvider,
		IncrementalValuesProvider<ValidatedResult<Templates.ParsedTemplate>> parsedTemplatesProvider)
	=>
		context
		.SyntaxProvider
		.CreateSyntaxProvider
		(
			predicate: static (syntaxNode, _) => syntaxNode is TypeDeclarationSyntax,
			transform: static (syntaxContext, _) => syntaxContext
		)
		.Combine(parsedTemplatesProvider.Collect())
		.Select((x, cancellationToken) => CreateClassMeta(x.Left, x.Right, cancellationToken))
		.Where(static(x) => x != ClassMeta.Empty)
		.Combine(projectInformationProvider)
		.Select(static(x, _) =>
			x.Left.Namespace != string.Empty
			? x.Left
			: x.Left.WithNamespace(x.Right.Namespace)
		);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ClassMeta CreateClassMeta(
		GeneratorSyntaxContext context,
		ImmutableArray<ValidatedResult<Templates.ParsedTemplate>> parsedTemplates,
		CancellationToken cancellationToken)
	{
		var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;
		var typeSymbol = (INamedTypeSymbol?)context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken);
		if (typeSymbol is null)
			return ClassMeta.Empty;

		ImmutableArray<AttributeInstance> possibleTemplates =
		(
			from x in typeDeclarationSyntax.AttributeLists.SelectMany(x => x.Attributes)
			let attributeFullName = x.Name.ToFullString()
			let parsedTemplate = parsedTemplates.FirstOrDefault(x => x.Success && x.Value.Name == attributeFullName)
			where parsedTemplate.Value != Templates.ParsedTemplate.Empty
			where context.SemanticModel.GetDeclaredSymbol(x, cancellationToken) == null
			select new AttributeInstance(
				name: attributeFullName,
				arguments: x.GetArgumentKeyValuePairs(context.SemanticModel, parsedTemplate.Value.RequiredInputs)
			)
		)
		.ToImmutableArray();

		if (possibleTemplates.Length == 0)
			return ClassMeta.Empty;

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
