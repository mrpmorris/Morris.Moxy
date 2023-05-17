using System.Runtime.CompilerServices;

namespace Morris.Moxy.Helpers;

internal static class NamespaceHelper
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string Combine(string @namespace, string className) =>
		@namespace is null
		? className
		: @namespace + "." + className;
}
