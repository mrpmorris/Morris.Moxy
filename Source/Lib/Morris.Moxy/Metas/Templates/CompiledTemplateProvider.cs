using Microsoft.CodeAnalysis;
using Morris.Moxy.Extensions;
using System.Runtime.CompilerServices;
using ScribanCompiledTemplate = Scriban.Template;

namespace Morris.Moxy.Metas.Templates;

internal static class CompiledTemplateProvider
{
	public static IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> CreateCompiledTemplateProvider(
		this IncrementalValuesProvider<ValidatedResult<ParsedTemplate>> parsedTemplateResults)
	=>
		parsedTemplateResults
		.Select((x, _) =>
			x.Success switch
			{
				false => (x.FilePath, CompiledTemplate.Error), // TODO: PeteM - Get name from somewhere
				true => CreateCompiledTemplateResult(x.Value)
			});

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ValidatedResult<CompiledTemplate> CreateCompiledTemplateResult(ParsedTemplate parsedTemplate)
	{
		var compiledScriban = ScribanCompiledTemplate.Parse(
			text: parsedTemplate.TemplateSource,
			sourceFilePath: parsedTemplate.FilePath);

		return compiledScriban.HasErrors switch {
			false =>
				(
					parsedTemplate.FilePath,
					new CompiledTemplate(
						name: parsedTemplate.Name,
						filePath: parsedTemplate.FilePath,
						parsedTemplate: parsedTemplate,
						compiledScript: compiledScriban)
				),
			true =>
				(
					parsedTemplate.FilePath,
					compiledScriban.Messages.ToCompilationErrors(parsedTemplate.TemplateBodyLineIndex)
				)
		};
	}
}
