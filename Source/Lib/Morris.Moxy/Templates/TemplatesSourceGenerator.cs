using Microsoft.CodeAnalysis;
using Morris.Moxy.SourceGenerators;
using System.Collections.Immutable;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.Templates;

public static class TemplatesSourceGenerator
{
	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		string rootNamespace,
		string projectPath,
		IEnumerable<ValidatedResult<CompiledTemplate>> templateResults,
		out ImmutableDictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateLookup)
	{
		Dictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateBuilder =
			new (StringComparer.OrdinalIgnoreCase);

		bool hasErrors = false;
		foreach (var templateResult in templateResults)
		{
			if (!templateResult.Success)
			{
				hasErrors = true;
				productionContext.AddCompilationErrors(
					templateResult.Value.FilePath,
					templateResult.CompilationErrors);
				continue;
			}

			CompiledTemplate compiledTemplate = templateResult.Value;
			if (nameToCompiledTemplateBuilder.ContainsKey(compiledTemplate.Name))
			{
				hasErrors = true;
				productionContext.AddCompilationError(
					compiledTemplate.FilePath,
					CompilationErrors.TemplateNamesMustBeUnique);
				continue;
			}

			CompiledTemplateAndAttributeSource generated =
				TemplateAttributeSourceGenerator.Generate(
					rootNamespace: rootNamespace,
					projectPath: projectPath,
					compiledTemplate: templateResult.Value);
			nameToCompiledTemplateBuilder.Add(compiledTemplate.Name, generated);

			productionContext.AddSource(
				hintName: $"{templateResult.Value!.Name}.TemplateAttribute.Moxy.g.cs",
				source: generated.AttributeSource);
		}

		if (hasErrors)
		{
			nameToCompiledTemplateLookup = ImmutableDictionary<string, CompiledTemplateAndAttributeSource>.Empty;
			return false;
		}

		nameToCompiledTemplateLookup = nameToCompiledTemplateBuilder.ToImmutableDictionary();
		return true;
	}
}
