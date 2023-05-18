using Microsoft.CodeAnalysis;

namespace Morris.Moxy.Providers;

internal static class ProjectInformationProvider
{
	public static IncrementalValueProvider<ProjectInformation> CreateProjectInformationProvider(this IncrementalGeneratorInitializationContext context) =>
		context
			.AnalyzerConfigOptionsProvider
			.Select(static (x, _) =>
			{
				x.GlobalOptions.TryGetValue("build_property.ProjectDir", out string? value);
				return value ?? "";
			})
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select(static (x, _) =>
			{
				x.Right.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? value);
				return new ProjectInformation(
					path: x.Left,
					@namespace: value ?? "");
			});
}
