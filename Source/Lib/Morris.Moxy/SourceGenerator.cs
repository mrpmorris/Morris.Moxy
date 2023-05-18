using Microsoft.CodeAnalysis;
using Morris.Moxy.Providers;

namespace Morris.Moxy;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<ProjectInformation> projectInformationProvider = context.CreateProjectInformationProvider();
	}
}
