using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.ProjectInformation;
using Morris.Moxy.Metas.Templates;
using Morris.Moxy.SourceGenerators;
using Morris.Moxy.Extensions;
using System.Collections.Immutable;

namespace Morris.Moxy;

[Generator]
public class RoslynIncrementalGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<ProjectInformationMeta> projectInformationProvider = context.CreateProjectInformationProvider();
		IncrementalValuesProvider<ClassMeta> classMetaProvider = context.CreateClassMetasProvider(projectInformationProvider);
		IncrementalValuesProvider<ValidatedResult<ParsedTemplate>> parsedTemplatesProvider = context.CreateParsedTemplatesProvider();
		IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> compiledTemplatesProvider = parsedTemplatesProvider.CreateCompiledTemplateProvider();

		context.RegisterSourceOutput(
			source: parsedTemplatesProvider.Combine(projectInformationProvider),
			static (productionContext, input) =>
			{
				ValidatedResult<ParsedTemplate> parsedTemplate = input.Left;
				if (parsedTemplate.Failure)
					productionContext.AddCompilationErrors(parsedTemplate.FilePath, parsedTemplate.CompilationErrors);
				else
				{
					ProjectInformationMeta projectInfo = input.Right;
					string generatedSourceCode = TemplateAttributeSourceGenerator.Generate(
						rootNamespace: projectInfo.Namespace,
						projectPath: projectInfo.Path,
						parsedTemplate: parsedTemplate.Value);
					productionContext.AddSource(
						hintName: $"{parsedTemplate.Value.Name}.TemplateAttribute.Moxy.g.cs",
						source: generatedSourceCode);
				}
			});

		context.RegisterSourceOutput(
			source: compiledTemplatesProvider.Combine(classMetaProvider.Collect()),
			static (productionContext, input) =>
			{
				ValidatedResult<CompiledTemplate> compiledTemplateResult = input.Left;
				ImmutableArray<ClassMeta> classMetas = input.Right;
				if (compiledTemplateResult.Failure)
					productionContext.AddCompilationErrors(compiledTemplateResult.FilePath, compiledTemplateResult.CompilationErrors);
				else
				{
					ClassSourceGenerator.Generate(productionContext, compiledTemplateResult.Value, classMetas);
				}
			});

	}

}
