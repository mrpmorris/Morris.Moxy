using Microsoft.CodeAnalysis;
using Morris.Moxy.Providers;

namespace Morris.Moxy;

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		IncrementalValueProvider<Models.ProjectInformation> projectInformationProvider = context.CreateProjectInformationProvider();

		context.RegisterImplementationSourceOutput(
			projectInformationProvider,
			(production, source) =>
			{
				production.AddSource("pete.cs", $"Namespace={source.Namespace}\r\nPath={source.Path}");
			});
	}
}
