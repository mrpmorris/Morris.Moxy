using Microsoft.CodeAnalysis;

namespace Morris.Moxy.TemplateHandlers
{
	public static class TemplatesSelector
	{
		public static IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> Select(
			IncrementalValuesProvider<AdditionalText> additionalTexts)
		=>
			additionalTexts
				.Where(x => x.Path.ToLower().EndsWith(".mixin"))
				.Select(static (file, cancellationToken) =>
					new TemplateNameAndSource(
					  Name: Path.GetFileNameWithoutExtension(file.Path),
					  FilePath: file.Path,
					  Source: file.GetText(cancellationToken)!.ToString()))
				.Select(static (templateNameAndSource, cancellationToken) =>
					TemplateParser.Parse(
						name: templateNameAndSource.Name,
						filePath: templateNameAndSource.FilePath,
						input: templateNameAndSource.Source))
				.Select(static (parsedTemplateResult, cancellationToken) =>
					TemplateCompiler.Compile(parsedTemplateResult));
	}
}
