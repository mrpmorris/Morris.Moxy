using Microsoft.CodeAnalysis;
using Morris.Moxy.Classes;
using Morris.Moxy.Templates;
using System.Collections.Immutable;

namespace Morris.Moxy
{
	[Generator]
	public class SourceGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> parsedTemplates =
				TemplatesSelector.Select(context.AdditionalTextsProvider);
			IncrementalValuesProvider<ClassInfo> classInfos =
				ClassesSelector.Select(context.SyntaxProvider);

			var config = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
			{
				if (x.GlobalOptions.TryGetValue("build_property.projectdir", out string? value))
					return value!;
				return "";
			});

			var generatorInput = context.CompilationProvider
				.Combine(parsedTemplates.Collect())
				.Select(static (x, _) => new
				{
					Compilation = x.Left,
					ParsedTemplateResults = x.Right
				})
				.Combine(config)
				.Select(static (x, _) => new
				{
					x.Left.Compilation,
					x.Left.ParsedTemplateResults,
					ProjectPath = x.Right
				})
				.Combine(classInfos.Collect())
				.Select(static (x, _) => new
				{
					x.Left.Compilation,
					x.Left.ParsedTemplateResults,
					x.Left.ProjectPath,
					Classes = x.Right
				});


			context.RegisterSourceOutput(
				generatorInput,
				static (productionContext, x) =>
				{
					Compilation compilation = x.Compilation;
					string assemblyName = compilation.AssemblyName!;
					ImmutableArray<ValidatedResult<CompiledTemplate>> templateResults = x.ParsedTemplateResults;

					if (!TemplatesSourceGenerator.TryGenerateSource(
						productionContext,
						assemblyName: assemblyName,
						projectPath: x.ProjectPath,
						templateResults,
						out ImmutableDictionary<string, CompiledTemplate> nameToCompiledTemplateLookup))
					{
						return;
					}
				});
		}
	}
}
