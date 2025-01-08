using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
			.Where(static(x) => x is not null)
			.Combine(projectInformationProvider)
			.Select(static(x, _) =>
				x.Left!.Namespace != string.Empty
				? x.Left!
				: x.Left!.WithNamespace(x.Right.Namespace)
			);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ClassMeta? CreateClassMeta(
		GeneratorSyntaxContext context,
		ImmutableArray<ValidatedResult<Templates.ParsedTemplate>> parsedTemplates,
		CancellationToken cancellationToken)
	{
		var typeDeclarationSyntax = (TypeDeclarationSyntax)context.Node;
		var typeSymbol = (INamedTypeSymbol?)context.SemanticModel.GetDeclaredSymbol(typeDeclarationSyntax, cancellationToken);
		if (typeSymbol is null)
			return null;

		ImmutableArray<AttributeInstance> possibleTemplates =
		(
			from x in typeDeclarationSyntax.AttributeLists.SelectMany(x => x.Attributes)
			let attributeFullName = x.Name.ToFullString()
			let parsedTemplate = parsedTemplates.FirstOrDefault(x => x.Success && x.Value.Name == attributeFullName)
			where parsedTemplate?.Value is not null
			where context.SemanticModel.GetDeclaredSymbol(x, cancellationToken) is null
			select new AttributeInstance(
				name: attributeFullName,
				arguments: x.GetArgumentKeyValuePairs
				(
					semanticModel: context.SemanticModel,
					requiredInputs: parsedTemplate.Value.RequiredInputs,
					optionalInputs: parsedTemplate.Value.OptionalInputs
				)
			)
		)
		.ToImmutableArray();

		if (possibleTemplates.Length == 0)
			return null;

		string className = typeDeclarationSyntax.Identifier.Text;
		string @namespace = typeSymbol.ContainingNamespace.ToDisplayString();
		string? declaringTypeName = typeSymbol.ContainingType?.Name; 
		var genericParameterNames = 
			(typeDeclarationSyntax.TypeParameterList?.Parameters.Count ?? 0) == 0
			? ImmutableArray<string>.Empty
			: typeDeclarationSyntax
				.TypeParameterList!
				.Parameters
				.Select(x => x.Identifier.Text).ToImmutableArray();
		// Get members
		var fields = GetFields(typeDeclarationSyntax).ToImmutableArray();
		var properties = GetProperties(typeDeclarationSyntax).ToImmutableArray();
		var methods = GetMethods(typeDeclarationSyntax).ToImmutableArray();

		return new ClassMeta(
			className: className,
			@namespace: @namespace,
			declaringTypeName: declaringTypeName,
			genericParameterNames: genericParameterNames,
			possibleTemplates: possibleTemplates,
			fields: fields,
			properties: properties,
			methods: methods);
	}

	private static IEnumerable<FieldMeta> GetFields(TypeDeclarationSyntax typeDeclaration)
	{
		return typeDeclaration.Members
			.OfType<FieldDeclarationSyntax>()
			.SelectMany(fieldDecl => fieldDecl.Declaration.Variables.Select(variable =>
				new FieldMeta(
					name: variable.Identifier.Text,
					typeName: fieldDecl.Declaration.Type.ToString(),
					accessibility: GetAccessibilityString(fieldDecl.Modifiers))));
	}

	private static IEnumerable<PropertyMeta> GetProperties(TypeDeclarationSyntax typeDeclaration)
	{
		return typeDeclaration.Members
			.OfType<PropertyDeclarationSyntax>()
			.Select(prop => new PropertyMeta(
				name: prop.Identifier.Text,
				typeName: prop.Type.ToString(),
				accessibility: GetAccessibilityString(prop.Modifiers),
				hasGetter: prop.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.GetAccessorDeclaration)) ?? false,
				hasSetter: prop.AccessorList?.Accessors.Any(a => a.IsKind(SyntaxKind.SetAccessorDeclaration)) ?? false));
	}

	private static IEnumerable<MethodMeta> GetMethods(TypeDeclarationSyntax typeDeclaration)
	{
		return typeDeclaration.Members
			.OfType<MethodDeclarationSyntax>()
			.Select(method => new MethodMeta(
				name: method.Identifier.Text,
				returnTypeName: method.ReturnType.ToString(),
				accessibility: GetAccessibilityString(method.Modifiers),
				parameters: GetParameters(method.ParameterList).ToImmutableArray()));
	}

	private static IEnumerable<ParameterMeta> GetParameters(ParameterListSyntax parameterList)
	{
		return parameterList.Parameters.Select(param =>
			new ParameterMeta(
				name: param.Identifier.Text,
				typeName: param.Type?.ToString() ?? "object",
				defaultValue: param.Default?.Value.ToString()));
	}

	private static string GetAccessibilityString(SyntaxTokenList modifiers)
	{
		if (modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword))) return "public";
		if (modifiers.Any(m => m.IsKind(SyntaxKind.PrivateKeyword))) return "private";
		if (modifiers.Any(m => m.IsKind(SyntaxKind.ProtectedKeyword))) return "protected";
		if (modifiers.Any(m => m.IsKind(SyntaxKind.InternalKeyword))) return "internal";
		return "private"; // default accessibility
	}
}
