using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.Templates;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.SourceGenerators;

internal static class ClassSourceGenerator
{
	public static void Generate(SourceProductionContext productionContext, CompiledTemplate compiledTemplate, ImmutableArray<ClassMeta> classMetas)
	{
		for (int i = 0; i < classMetas.Length; i++)
		{
			ClassMeta classMeta = classMetas[i];
			GenerateForClassMeta(productionContext, compiledTemplate, classMeta);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void GenerateForClassMeta(SourceProductionContext productionContext, CompiledTemplate compiledTemplate, ClassMeta classMeta)
	{
		for (int i = 0; i < classMeta.PossibleTemplates.Length; i++)
		{
			string templateName = classMeta.PossibleTemplates[i];
			if (templateName == compiledTemplate.Name)
				GenerateForTemplate(productionContext, compiledTemplate, classMeta);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void GenerateForTemplate(
		SourceProductionContext productionContext,
		CompiledTemplate compiledTemplate,
		ClassMeta classMeta)
	{
		string classFileName = $"{classMeta.FullName}.{compiledTemplate.Name}.MixinCode.Moxy.g.cs"
			.Replace("<", "{")
			.Replace(">", "}");

		string generatedSource = GenerateSourceCode(productionContext, compiledTemplate, classMeta);

		productionContext.AddSource(
			hintName: classFileName,
			source: generatedSource);

	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string GenerateSourceCode(SourceProductionContext productionContext, CompiledTemplate compiledTemplate, ClassMeta classMeta)
	{
		using var stringWriter = new StringWriter();
		using var writer = new IndentedTextWriter(stringWriter);
		writer.WriteLine($"// Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

		return stringWriter.ToString();
	}
}