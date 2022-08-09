using Microsoft.CodeAnalysis;
using Morris.Moxy.DataStructures;
using Morris.Moxy.TemplateHandlers;
using System.Text;

namespace Morris.Moxy
{
	[Generator]
	public class CodeGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<TemplateNameAndSource> templateNamesAndSources =
				TemplateSelectors.SelectTemplateNamesAndSources(context.AdditionalTextsProvider);

			var combined = context.CompilationProvider.Combine(templateNamesAndSources.Collect());

			context.RegisterSourceOutput(
				combined,
				static (productionContext, x) =>
				{
					var sb = new StringBuilder();
					sb.AppendLine("public class Testercles {");
					foreach (var item in x.Right)
						sb.AppendLine(item.Source);
					sb.AppendLine("}");
					string sourceText = sb.ToString();
					productionContext.AddSource("hahaha.g.cs", sourceText!);
				});
		}
	}

	public readonly record struct SomethingFake(string Name, string FilePath, string Source)
	{
		public static SomethingFake Create(string name, string filePath, string source)
		{
			Console.Beep();
			return new SomethingFake(name, filePath, source);
		}
	}
}
