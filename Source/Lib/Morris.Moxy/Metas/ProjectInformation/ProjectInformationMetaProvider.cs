using Microsoft.CodeAnalysis;

namespace Morris.Moxy.Metas.ProjectInformation;

internal static class ProjectInformationMetaProvider
{
	public static IncrementalValueProvider<ProjectInformationMeta> CreateProjectInformationProvider(this IncrementalGeneratorInitializationContext context) =>
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
				return new ProjectInformationMeta(
					path: x.Left,
					@namespace: value ?? "");
			});
}
