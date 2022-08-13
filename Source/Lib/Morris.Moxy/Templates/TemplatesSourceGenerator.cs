using Microsoft.CodeAnalysis;
using Morris.Moxy.SourceGenerators;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.Templates;

public static class TemplatesSourceGenerator
{
	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		string assemblyName,
		string projectPath,
		IEnumerable<ValidatedResult<CompiledTemplate>> templateResults,
		out ImmutableDictionary<string, CompiledTemplate> nameToCompiledTemplateLookup)
	{
		using var stringWriter = new StringWriter();
		using var writer = new IndentedTextWriter(stringWriter);

		Dictionary<string, CompiledTemplate> nameToCompiledTemplateBuilder = new(StringComparer.OrdinalIgnoreCase);

		bool hasErrors = false;
		writer.WriteLine($"// Generated {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");
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
			if (!nameToCompiledTemplateBuilder.ContainsKey(compiledTemplate.Name))
				nameToCompiledTemplateBuilder.Add(compiledTemplate.Name, compiledTemplate);
			else
			{
				hasErrors = true;
				productionContext.AddCompilationError(
					compiledTemplate.FilePath,
					CompilationErrors.TemplateNamesMustBeUnique);
				continue;
			}

			TemplateAttributeSourceGenerator.Generate(
				writer: writer,
				assemblyName: assemblyName,
				projectPath: projectPath,
				compiledTemplate: templateResult.Value);
		}
		if (hasErrors)
		{
			nameToCompiledTemplateLookup = ImmutableDictionary<string, CompiledTemplate>.Empty;
			return false;
		}

		string source = stringWriter.ToString();
		productionContext.AddSource(
			hintName: $"{assemblyName}.Moxy.TemplateAttributes.g.cs",
			source: source);

		nameToCompiledTemplateLookup = nameToCompiledTemplateBuilder.ToImmutableDictionary();
		return true;
	}
}
