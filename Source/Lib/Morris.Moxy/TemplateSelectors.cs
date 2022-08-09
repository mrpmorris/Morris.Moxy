using Microsoft.CodeAnalysis;
using Morris.Moxy.DataStructures;

namespace Morris.Moxy.TemplateHandlers
{
	public static class TemplateSelectors
  {
	public static IncrementalValuesProvider<TemplateNameAndSource> SelectTemplateNamesAndSources(
	  IncrementalValuesProvider<AdditionalText> additionalTexts)
	=>
	  additionalTexts
		.Where(x => x.Path.ToLower().EndsWith(".mixin"))
		.Select(
		  static (file, cancellationToken) =>
			TemplateNameAndSource.Create(
			  name: Path.GetFileNameWithoutExtension(file.Path),
			  filePath: file.Path,
			  source: file.GetText(cancellationToken)!.ToString()));
  }
}
