using System.CodeDom.Compiler;

namespace Morris.Moxy.Extensions;

internal static class IndentedTextWriterExtensions
{
	public static IDisposable Indent(this IndentedTextWriter writer)
	{
		writer.Indent++;
		return new DisposableAction(() => writer.Indent--);
	}

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
