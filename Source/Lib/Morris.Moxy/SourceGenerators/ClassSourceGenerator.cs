using Microsoft.CodeAnalysis;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.Templates;
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
			Generate(productionContext, compiledTemplate, classMeta);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Generate(SourceProductionContext productionContext, CompiledTemplate compiledTemplate, ClassMeta classMeta)
	{
		string classFullName = $"{classMeta.FullName}.{compiledTemplate.Name}.Moxy.g.cs"
			.Replace("<", "{")
			.Replace(">", "}");

		for (int i = 0; i < classMeta.PossibleTemplates.Length; i++)
		{
			string templateName = classMeta.PossibleTemplates[i];
			if (templateName == compiledTemplate.Name)
				Generate(productionContext, compiledTemplate, classMeta, templateName);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Generate(
		SourceProductionContext productionContext,
		CompiledTemplate compiledTemplate,
		ClassMeta classMeta,
		string templateName)
	{
	}
}