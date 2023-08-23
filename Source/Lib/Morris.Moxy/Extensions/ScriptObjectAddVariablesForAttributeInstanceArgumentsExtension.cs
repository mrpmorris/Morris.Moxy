using Morris.Moxy.Metas.Classes;
using Scriban.Runtime;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class ScriptObjectAddVariablesForAttributeInstanceArgumentsExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AddVariablesForAttributeInstanceArguments(
		this ScriptObject scriptObject,
		AttributeInstance attributeInstance)
	{
		for (int i = 0; i < attributeInstance.Arguments.Length; i++)
		{
			KeyValuePair<string, object?> argument = attributeInstance.Arguments[i];
			scriptObject.Add(argument.Key, argument.Value);
		}
	}
}
