using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class IndentedTextWriterIndentExtensions
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IDisposable Indent(this IndentedTextWriter writer)
	{
		writer.Indent++;
		return new DisposableAction(() => writer.Indent--);
	}
}
