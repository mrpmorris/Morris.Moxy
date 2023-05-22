using Microsoft.CodeAnalysis;

namespace Morris.Moxy.Metas.ProjectInformation;

internal static class ProjectInformationMetaProvider
{
	public static IncrementalValueProvider<ProjectInformationMeta> CreateProjectInformationProvider(this IncrementalGeneratorInitializationContext context) =>
		context
			.AnalyzerConfigOptionsProvider
			.Select(static (x, _) => x.GlobalOptions.TryGetValue("build_property.ProjectDir", out string? value) ? value : "")
			.Combine(context.AnalyzerConfigOptionsProvider)
			.Select(static (x, _) =>
				x.Right.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? value)
				? new ProjectInformationMeta(
					path: x.Left,
					@namespace: value)
				: new ProjectInformationMeta(
					path: x.Left,
					@namespace: "")
			);
}
