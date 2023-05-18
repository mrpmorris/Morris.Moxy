using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.ProjectInformation;

namespace Morris.Moxy;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<ProjectInformationMeta> projectInformationProvider = ProjectInformationMetaProvider.CreateProjectInformationProvider(context);
	}
}
