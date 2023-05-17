using Microsoft.CodeAnalysis;
using Morris.Moxy.Classes;
using Morris.Moxy.Templates;

namespace Morris.Moxy
{
	[Generator]
	public class SourceGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> parsedTemplatesProvider =
				TemplatesSelector.Select(context.AdditionalTextsProvider);
			IncrementalValuesProvider<ClassInfo> classInfosProvider =
				ClassesSelector.Select(context.SyntaxProvider);

			var projectPathProvider = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
			{
				if (x.GlobalOptions.TryGetValue("build_property.projectdir", out string? value))
					return value!;
				return "";
			});

			var rootNamespaceProvider = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
			{
				if (x.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? value))
					return value!;
				return "";
			});

			var pathsProvider = projectPathProvider
				.Select(static(x, _) => new GeneratorInput { ProjectPath = x })
				.Combine(rootNamespaceProvider)
				.Select(static(x, _) => new GeneratorInput(x.Left) { RootNamespace = x.Right });

			context.RegisterSourceOutput(
				source: parsedTemplatesProvider,
				action: static (productionContext, x) =>
				{

				}
			);

			var attributesInput = pathsProvider;
			context.RegisterSourceOutput(
				source: attributesInput,
				action: static (productionContext, x) =>
				{

				});


			//var generatorInputOld = context.CompilationProvider
			//	.Combine(rootNamespace)
			//	.Select(static (x, _) => (
			//		Compilation: x.Left,
			//		RootNamespace: x.Right
			//	))
			//	.Combine(parsedTemplates.Collect())
			//	.Select(static (x, _) => (
			//		x.Left.Compilation,
			//		x.Left.RootNamespace,
			//		ParsedTemplateResults: x.Right
			//	))
			//	.Combine(config)
			//	.Select(static (x, _) => (
			//		x.Left.Compilation,
			//		x.Left.RootNamespace,
			//		x.Left.ParsedTemplateResults,
			//		ProjectPath: x.Right
			//	))
			//	.Combine(classInfos.Collect())
			//	.Select(static (x, _) => (
			//		x.Left.Compilation,
			//		x.Left.RootNamespace,
			//		x.Left.ParsedTemplateResults,
			//		x.Left.ProjectPath,
			//		Classes: x.Right
			//	));

			//context.RegisterSourceOutput(
			//	generatorInputOld,
			//	static (productionContext, x) =>
			//	{
			//		Compilation compilation = x.Compilation;
			//		var reflection = new MetadataLoadContext(compilation);
			//		string assemblyName = compilation.AssemblyName!;
			//		ImmutableArray<ValidatedResult<CompiledTemplate>> templateResults = x.ParsedTemplateResults;

			//		if (!TemplatesSourceGenerator.TryGenerateSource(
			//			productionContext,
			//			rootNamespace: x.RootNamespace,
			//			projectPath: x.ProjectPath,
			//			templateResults,
			//			out ImmutableDictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateLookup))
			//		{
			//			return;
			//		}

			//		if (!ClassesSourceGenerator.TryGenerateSource(
			//			productionContext,
			//			compilation,
			//			reflection,
			//			projectPath: x.ProjectPath,
			//			x.Classes,
			//			nameToCompiledTemplateLookup))
			//		{
			//			return;
			//		}
			//	});
		}
	}
}
