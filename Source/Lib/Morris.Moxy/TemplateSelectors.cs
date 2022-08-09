using Microsoft.CodeAnalysis;
using Morris.Moxy.DataStructures;

namespace Morris.Moxy.TemplateHandlers
{
	public static class TemplateSelectors
	{
		public static IncrementalValuesProvider<ValidatedResult<ParsedTemplate>> Select(
		  IncrementalValuesProvider<AdditionalText> additionalTexts)
		=>
		  additionalTexts
				.Where(x => x.Path.ToLower().EndsWith(".mixin"))
				.Select(static (file, cancellationToken) =>
					TemplateNameAndSource.Create(
					  name: Path.GetFileNameWithoutExtension(file.Path),
					  filePath: file.Path,
					  source: file.GetText(cancellationToken)!.ToString()))
				.Select(static (templateNameAndSource, cancellationToken) =>
					TemplateParser.Parse(
						name: templateNameAndSource.Name,
						filePath: templateNameAndSource.FilePath,
						input: templateNameAndSource.Source));
	}
}
