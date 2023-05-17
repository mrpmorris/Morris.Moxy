using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.Templates;

public static class TemplatesSourceGenerator
{
	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		string rootNamespace,
		string projectPath,
		ValidatedResult<CompiledTemplate> validatedTemplate)
	{
		if (!validatedTemplate.Success)
		{
			productionContext.AddCompilationErrors(
				validatedTemplate.Value.FilePath,
				validatedTemplate.CompilationErrors);
			return false;
		}

		CompiledTemplate compiledTemplate = validatedTemplate.Value;
		CompiledTemplateAndAttributeSource generated =
			TemplateAttributeSourceGenerator.Generate(
				rootNamespace: rootNamespace,
				projectPath: projectPath,
				compiledTemplate: validatedTemplate.Value);

		productionContext.AddSource(
			hintName: $"{validatedTemplate.Value!.Name}.TemplateAttribute.Moxy.g.cs",
			source: generated.AttributeSource);
		return true;
	}
}
