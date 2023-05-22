using Scriban;
using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Templates;

internal static class TemplateCompiler
{
	public static ValidatedResult<CompiledTemplate> Compile(ValidatedResult<ParsedTemplate> parsedTemplateResult) =>
		parsedTemplateResult.Success switch {
			false => (parsedTemplateResult.FilePath, ImmutableArray<CompilationError>.Empty),
			true => Compile(parsedTemplateResult.Value)
		};

	private static (string FilePath, ImmutableArray<CompilationError> Empty) Compile(ParsedTemplate parsedTemplate)
	{
		var scribanTemplate = Template.Parse(
			text: parsedTemplate.TemplateSource,
			sourceFilePath: parsedTemplate.FilePath);
		throw new NotImplementedException();
	}
}
