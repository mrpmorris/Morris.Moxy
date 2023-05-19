using System.CodeDom.Compiler;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class IndentedTextWriterCodeBlockExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IDisposable CodeBlock(this IndentedTextWriter writer)
	{
		writer.WriteLine("{");
		writer.Indent++;
		return new DisposableAction(() =>
		{
			writer.Indent--;
			writer.WriteLine("}");
		});
	}
}
