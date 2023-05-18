using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.ProjectInformation;

namespace Morris.Moxy;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<ProjectInformationMeta> projectInformationProvider = context.CreateProjectInformationProvider();
		IncrementalValuesProvider<ClassMeta> classMetaProvider = context.CreateClassMetaProvider(projectInformationProvider);

		context.RegisterSourceOutput(
			source: classMetaProvider,
			static (production, classMeta) =>
			{

			});
	}
}
