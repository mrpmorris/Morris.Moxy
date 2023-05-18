using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.ProjectInformation;
using Morris.Moxy.Metas.Templates;

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
			source: parsedTemplatesProvider,
			static (production, parsedTemplate) =>
			{

			});
	}
}
