using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class AttributeSyntaxGetArgumentKeyValuePairsExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<KeyValuePair<string, string>> GetArgumentKeyValuePairs(
		this AttributeSyntax attributeSyntax,
		SemanticModel semanticModel,
		ImmutableArray<Metas.Templates.TemplateInput> requiredInputs,
		ImmutableArray<Metas.Templates.TemplateInput> optionalInputs)
	{
		SeparatedSyntaxList<AttributeArgumentSyntax>? arguments = attributeSyntax.ArgumentList?.Arguments;
		if (arguments is null)
			return ImmutableArray<KeyValuePair<string, string>>.Empty;

		ImmutableArray<Metas.Templates.TemplateInput> allInputs = requiredInputs.AddRange(optionalInputs);

		var resultBuilder = ImmutableArray.CreateBuilder<KeyValuePair<string, string>>();

		for(int argumentIndex = 0; argumentIndex < arguments.Value.Count; argumentIndex++)
		{
			AttributeArgumentSyntax argument = arguments.Value[argumentIndex];

			string argumentName =
				argument.NameEquals is not null
				? argument.NameEquals.Name.Identifier.ValueText
				: argument.NameColon is not null
				? argument.NameColon.Name.Identifier.ValueText
				: allInputs[argumentIndex].Name;

			string value = argument.Expression switch {
				TypeOfExpressionSyntax x => x.ToFullString(),
				_ => TrimQuotes(argument)
			};

			resultBuilder.Add(new KeyValuePair<string, string>(argumentName, value));
		}

		return resultBuilder.ToImmutableArray();
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string TrimQuotes(AttributeArgumentSyntax argument) =>
		argument.Expression.ToFullString() switch {
			string x when x.StartsWith("\"") => x.Substring(1, x.Length - 2),
			string x => x,
			_ => throw new NotImplementedException()
		};
}
