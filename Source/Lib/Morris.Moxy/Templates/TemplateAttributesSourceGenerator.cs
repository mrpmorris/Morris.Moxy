using Microsoft.CodeAnalysis;
using Morris.Moxy.SourceGenerators;
using System.Collections.Immutable;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.Templates;

public static class TemplateAttributesSourceGenerator
{
	private static Dictionary<CompiledTemplate, CompiledTemplateAndAttributeSource> Cache = new();

	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		string rootNamespace,
		string projectPath,
		ImmutableArray<ValidatedResult<CompiledTemplate>> templateResults,
		out ImmutableDictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateLookup)
	{
		Dictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateBuilder =
			new (StringComparer.OrdinalIgnoreCase);

		var nextCache = new Dictionary<CompiledTemplate, CompiledTemplateAndAttributeSource>();

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

			if (!Cache.TryGetValue(compiledTemplate, out CompiledTemplateAndAttributeSource generated))
			{
				generated =
					TemplateAttributeSourceGenerator.Generate(
						rootNamespace: rootNamespace,
						projectPath: projectPath,
						compiledTemplate: templateResult.Value);
			}
			nameToCompiledTemplateBuilder.Add(compiledTemplate.Name, generated);
			nextCache.Add(compiledTemplate, generated);


			productionContext.AddSource(
				hintName: $"{templateResult.Value!.Name}.TemplateAttribute.Moxy.g.cs",
				source: generated.AttributeSource);
		}

		Cache = nextCache;

		if (hasErrors)
		{
			nameToCompiledTemplateLookup = ImmutableDictionary<string, CompiledTemplateAndAttributeSource>.Empty;
			return false;
		}

		nameToCompiledTemplateLookup = nameToCompiledTemplateBuilder.ToImmutableDictionary();
		return true;
	}
}
