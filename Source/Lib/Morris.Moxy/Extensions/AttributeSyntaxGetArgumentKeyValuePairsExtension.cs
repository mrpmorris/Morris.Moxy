using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class AttributeSyntaxGetArgumentKeyValuePairsExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<KeyValuePair<string, object?>> GetArgumentKeyValuePairs(
		this AttributeSyntax attributeSyntax,
		SemanticModel semanticModel,
		ImmutableArray<Metas.Templates.TemplateInput> requiredInputs,
		ImmutableArray<Metas.Templates.TemplateInput> optionalInputs)
	{
		SeparatedSyntaxList<AttributeArgumentSyntax>? arguments = attributeSyntax.ArgumentList?.Arguments;
		if (arguments is null)
			return ImmutableArray<KeyValuePair<string, object?>>.Empty;

		ImmutableArray<Metas.Templates.TemplateInput> allInputs = requiredInputs.AddRange(optionalInputs);

		var nameToValueLookup = new Dictionary<string, object?>();
		foreach (var item in optionalInputs.Union(requiredInputs).Where(x => x.DefaultValue is not null))
			nameToValueLookup[item.Name] = GetValueFromStringRepresentation(item.DefaultValue!);

		for(int argumentIndex = 0; argumentIndex < arguments.Value.Count; argumentIndex++)
		{
			AttributeArgumentSyntax argument = arguments.Value[argumentIndex];

			string argumentName =
				argument.NameEquals is not null
				? argument.NameEquals.Name.Identifier.ValueText
				: argument.NameColon is not null
				? argument.NameColon.Name.Identifier.ValueText
				: allInputs[argumentIndex].Name;


			object? value = GetValueFromStringRepresentation(argument.ToString());
			nameToValueLookup[argumentName] = value;

		}
		return nameToValueLookup.ToImmutableArray();
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? GetValueFromStringRepresentation(string expressionStr)
	{
		var expression = SyntaxFactory.ParseExpression(expressionStr);
		return GetValueFromArgument(expression);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static object? GetValueFromArgument(ExpressionSyntax expression)
	{
		return expression switch {
			TypeOfExpressionSyntax x => x.Type.ToFullString(),
			LiteralExpressionSyntax lit when lit.Token.IsKind(SyntaxKind.TrueKeyword) => true,
			LiteralExpressionSyntax lit when lit.Token.IsKind(SyntaxKind.FalseKeyword) => false,
			LiteralExpressionSyntax lit when lit.Token.IsKind(SyntaxKind.NullKeyword) => null,
			_ => TrimQuotes(expression)
		};
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string TrimQuotes(ExpressionSyntax expression) =>
		expression.ToFullString() switch {
			string x when x.StartsWith("\"") => x.Substring(1, x.Length - 2),
			string x => x,
			_ => throw new NotImplementedException()
		};
}
