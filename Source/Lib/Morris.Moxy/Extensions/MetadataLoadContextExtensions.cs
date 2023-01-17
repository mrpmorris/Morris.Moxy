using Roslyn.Reflection;

namespace Morris.Moxy.Extensions;

internal static class MetadataLoadContextExtensions
{
	public static Type ResolveType(
		this MetadataLoadContext instance,
		string fullyQualifiedMetadataName,
		int numberOfGenericParameters)
	=>
		numberOfGenericParameters == 0
		? instance.ResolveType(fullyQualifiedMetadataName)
		: instance.ResolveType($"{fullyQualifiedMetadataName}`{numberOfGenericParameters}");
}
