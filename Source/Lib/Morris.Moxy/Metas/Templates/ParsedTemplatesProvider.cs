using Microsoft.CodeAnalysis;

namespace Morris.Moxy.Metas.Templates;

internal static class ParsedTemplatesProvider
{
	public static IncrementalValuesProvider<ValidatedResult<ParsedTemplate>> CreateParsedTemplatesProvider(this IncrementalGeneratorInitializationContext context) =>
		context
		.AdditionalTextsProvider
		.Where(static (x) => x.Path.ToLower().EndsWith(".mixin"))
		.Select(static (file, cancellationToken) =>
			new TemplateSource
			(
				name: Path.GetFileNameWithoutExtension(file.Path),
				filePath: file.Path,
				source: file.GetText(cancellationToken)!.ToString()
			))
		.Select(CreateParsedTemplate);

	private static ValidatedResult<ParsedTemplate> CreateParsedTemplate(TemplateSource templateSource, CancellationToken token) =>
		TemplateParser.Parse(
			name: templateSource.Name,
			filePath: templateSource.FilePath,
			source: templateSource.Source);
}

