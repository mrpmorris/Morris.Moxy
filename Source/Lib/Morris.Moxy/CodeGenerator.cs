using Microsoft.CodeAnalysis;
using Morris.Moxy.DataStructures;
using Morris.Moxy.TemplateHandlers;

namespace Morris.Moxy
{
	[Generator]
	public class CodeGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<ValidatedResult<ParsedTemplate>> parsedTemplates =
				TemplateSelectors.Select(context.AdditionalTextsProvider);

			var combined = context.CompilationProvider.Combine(parsedTemplates.Collect());

			context.RegisterSourceOutput(
				combined,
				static (productionContext, x) =>
				{
					productionContext.AddSource("hahaha.g.cs", "");
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
