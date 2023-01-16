using Microsoft.CodeAnalysis;
using Morris.Moxy.Classes;
using Morris.Moxy.Templates;
using Roslyn.Reflection;
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

			var rootNamespace = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
			{
				if (x.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? value))
					return value!;
				return "";
			});

			var generatorInput = context.CompilationProvider
				.Combine(rootNamespace)
				.Select(static (x, _) => (
					Compilation: x.Left,
					RootNamespace: x.Right
				))
				.Combine(parsedTemplates.Collect())
				.Select(static (x, _) => (
					x.Left.Compilation,
					x.Left.RootNamespace,
					ParsedTemplateResults: x.Right
				))
				.Combine(config)
				.Select(static (x, _) => (
					x.Left.Compilation,
					x.Left.RootNamespace,
					x.Left.ParsedTemplateResults,
					ProjectPath: x.Right
				))
				.Combine(classInfos.Collect())
				.Select(static (x, _) => (
					x.Left.Compilation,
					x.Left.RootNamespace,
					x.Left.ParsedTemplateResults,
					x.Left.ProjectPath,
					Classes: x.Right
				));

			context.RegisterSourceOutput(
				generatorInput,
				static (productionContext, x) =>
				{
					Compilation compilation = x.Compilation;
					var reflection = new MetadataLoadContext(compilation);
					string assemblyName = compilation.AssemblyName!;
					ImmutableArray<ValidatedResult<CompiledTemplate>> templateResults = x.ParsedTemplateResults;

					if (!TemplatesSourceGenerator.TryGenerateSource(
						productionContext,
						rootNamespace: x.RootNamespace,
						projectPath: x.ProjectPath,
						templateResults,
						out ImmutableDictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateLookup))
					{
						return;
					}

					if (!ClassesSourceGenerator.TryGenerateSource(
						productionContext,
						compilation,
						reflection,
						projectPath: x.ProjectPath,
						x.Classes,
						nameToCompiledTemplateLookup))
					{
						return;
					}
				});
		}
	}
}
