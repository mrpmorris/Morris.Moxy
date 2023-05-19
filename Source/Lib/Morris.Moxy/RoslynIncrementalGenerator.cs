using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.ProjectInformation;
using Morris.Moxy.Metas.Templates;
using Morris.Moxy.SourceGenerators;
using Morris.Moxy.Extensions;

namespace Morris.Moxy;

[Generator]
public class RoslynIncrementalGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<ProjectInformationMeta> projectInformationProvider = context.CreateProjectInformationProvider();
		IncrementalValuesProvider<ClassMeta> classMetaProvider = context.CreateClassMetasProvider(projectInformationProvider);
		IncrementalValuesProvider<ValidatedResult<ParsedTemplate>> parsedTemplatesProvider = context.CreateParsedTemplatesProvider();

		context.RegisterSourceOutput(
			source: parsedTemplatesProvider.Combine(projectInformationProvider),
			static (production, input) =>
			{
				ValidatedResult<ParsedTemplate> parsedTemplate = input.Left;
				if (parsedTemplate.Failure)
					production.AddCompilationErrors(parsedTemplate.FilePath, parsedTemplate.CompilationErrors);
				else
				{
					ProjectInformationMeta projectInfo = input.Right;
					string generatedSourceCode = TemplateAttributeSourceGenerator.Generate(
						rootNamespace: projectInfo.Namespace,
						projectPath: projectInfo.Path,
						parsedTemplate: parsedTemplate.Value);
					production.AddSource(
						hintName: $"{parsedTemplate.Value.Name}.TemplateAttribute.Moxy.g.cs",
						source: generatedSourceCode);
				}
			});
	}

}
