using Microsoft.CodeAnalysis;
namespace Morris.Moxy.Metas.Templates;

internal static class TemplateSourceProvider
{
	public static IncrementalValuesProvider<TemplateSource> CreateTemplateSourceProvider(this IncrementalGeneratorInitializationContext context) =>
		context
		.AdditionalTextsProvider
		.Where(static (x) => x.Path.ToLower().EndsWith(".mixin"))
		.Select(static (file, cancellationToken) =>
			new TemplateSource
			(
				name: Path.GetFileNameWithoutExtension(file.Path),
				filePath: file.Path,
				source: file.GetText(cancellationToken)!.ToString()
			));
}

