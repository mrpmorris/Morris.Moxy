using Microsoft.CodeAnalysis;

namespace Morris.Moxy.ProjectPaths;

internal static class ProjectPathAndRootNamespaceProviderExtension
{
	public static IncrementalValueProvider<ProjectPathAndRootNamespace> CreateProjectPathAndRootNamespaceProvider(
		this IncrementalGeneratorInitializationContext context)
	{
		var projectPathProvider = context.AnalyzerConfigOptionsProvider.Select(static(x, _) =>
		{
			if (x.GlobalOptions.TryGetValue("build_property.projectdir", out string? value))
				return value!;
			return "";
		});

		var rootNamespaceProvider = context.AnalyzerConfigOptionsProvider.Select(static(x, _) =>
		{
			if (x.GlobalOptions.TryGetValue("build_property.RootNamespace", out string? value))
				return value!;
			return "";
		});

		IncrementalValueProvider<ProjectPathAndRootNamespace> pathsProvider = projectPathProvider
			.Combine(rootNamespaceProvider)
			.Select(static (x, _) => new ProjectPathAndRootNamespace(projectPath: x.Left, rootNamespace: x.Right));

		return pathsProvider;
	}
}
