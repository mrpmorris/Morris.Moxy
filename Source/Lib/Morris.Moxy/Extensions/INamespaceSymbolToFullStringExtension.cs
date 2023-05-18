using Microsoft.CodeAnalysis;

namespace Morris.Moxy.Extensions;

internal static class INamespaceSymbolToFullStringExtension
{
	public static string ToFullString(this INamespaceSymbol source) =>
		source.ContainingNamespace.IsGlobalNamespace
		? string.Empty
		: source.ContainingNamespace.ToString();
}
