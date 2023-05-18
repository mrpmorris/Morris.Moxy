using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class INamespaceSymbolToFullStringExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ToFullString(this INamespaceSymbol source) =>
		source.ContainingNamespace.IsGlobalNamespace
		? string.Empty
		: source.ContainingNamespace.ToString();
}
