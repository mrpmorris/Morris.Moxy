using Scriban;
using Scriban.Parsing;
using System.Collections.Immutable;

namespace Morris.Moxy.Templates;

internal static class TemplateCompiler
{
	public static ValidatedResult<CompiledTemplate> Compile(ValidatedResult<ParsedTemplate> parsedTemplateResult)
	{
		ParsedTemplate parsedTemplate = parsedTemplateResult.Value;

		if (!parsedTemplateResult.Success)
			return new ValidatedResult<CompiledTemplate>(
				value: new CompiledTemplate(name: parsedTemplate.Name, filePath: parsedTemplate.FilePath),
				compilationErrors: parsedTemplateResult.CompilationErrors);

		Console.Beep();
		var scribanTemplate = Template.Parse(
			text: parsedTemplate.TemplateSource,
			sourceFilePath: parsedTemplate.FilePath);

		if (!scribanTemplate.HasErrors)
			return new ValidatedResult<CompiledTemplate>(
				value: new CompiledTemplate(
					name: parsedTemplate.Name,
					filePath: parsedTemplate.FilePath,
					directives: parsedTemplate,
					template: scribanTemplate)
				);

		var errorBuilder = ImmutableArray.CreateBuilder<CompilationError>();
		foreach (LogMessage scribanError in scribanTemplate.Messages)
		{
			var compilationError = CompilationErrors.ScriptCompilationError with {
				Line = scribanError.Span.Start.Line + parsedTemplate.TemplateBodyLineNumber + 1,
				Column = scribanError.Span.Start.Column,
				Message = scribanError.Message
			};
			errorBuilder.Add(compilationError);
		}

		return new ValidatedResult<CompiledTemplate>(
			value: new CompiledTemplate(name: parsedTemplate.Name, filePath: parsedTemplate.FilePath),
			compilationErrors: errorBuilder.ToImmutable());
	}
}
