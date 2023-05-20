using Microsoft.CodeAnalysis;
using Morris.Moxy.Extensions;
using Morris.Moxy.Metas;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.ScriptVariables;
using Morris.Moxy.Metas.Templates;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using ScribanTemplateContext = Scriban.TemplateContext;

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
		var groupedTemplates = classMeta.PossibleTemplates.GroupBy(t => t.Name).ToArray();

		for (int groupIndex = 0; groupIndex < groupedTemplates.Length; groupIndex++)
		{
			var grouped = groupedTemplates[groupIndex];
			if (grouped.Key == compiledTemplate.Name)
				GenerateForGroupedTemplates(productionContext, compiledTemplate, classMeta, grouped.ToImmutableArray());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void GenerateForGroupedTemplates(
		SourceProductionContext productionContext,
		CompiledTemplate compiledTemplate,
		ClassMeta classMeta,
		ImmutableArray<AttributeInstance> attributeInstances)
	{
		int fileIndex = 1;
		for (int i = 0; i < attributeInstances.Length; i++)
		{
			AttributeInstance attributeInstance = attributeInstances[i];
			GenerateForTemplate(productionContext, compiledTemplate, classMeta, attributeInstance, fileIndex++);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void GenerateForTemplate(
		SourceProductionContext productionContext,
		CompiledTemplate compiledTemplate,
		ClassMeta classMeta,
		AttributeInstance attributeInstance,
		int index)
	{
		string classFileName = $"{classMeta.FullName}.{compiledTemplate.Name}.Instance{index}.MixinCode.Moxy.g.cs"
			.Replace("<", "{")
			.Replace(">", "}");

		string? generatedSource = GenerateSourceCodeForAttributeInstance(productionContext, compiledTemplate, classMeta, attributeInstance);
		if (generatedSource is not null)
			productionContext.AddSource(
				hintName: classFileName,
				source: generatedSource);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string? GenerateSourceCodeForAttributeInstance(
		SourceProductionContext productionContext,
		CompiledTemplate compiledTemplate,
		ClassMeta classMeta,
		AttributeInstance attributeInstance)
	{
		using var stringWriter = new StringWriter();
		using var writer = new IndentedTextWriter(stringWriter);
		writer.WriteLine($"// Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

		var classVariable = new ClassVariable(
			name: classMeta.ClassName,
			@namespace: classMeta.Namespace);
		var moxyVariable = new MoxyVariable(@class: classVariable);

		ScriptObject scribanScriptObject = ScribanTemplateContext.GetDefaultBuiltinObject();
		scribanScriptObject.Add(
			key: "moxy",
			value: moxyVariable);
		scribanScriptObject.AddVariablesForAttributeInstanceArguments(attributeInstance);

		var scribanTemplateContext = new ScribanTemplateContext(scribanScriptObject) {
			MemberRenamer = static m => m.Name
		};

		try
		{
			string templateResult = compiledTemplate.CompiledScript.Render(scribanTemplateContext);
			writer.WriteLine(templateResult);
		}
		catch (ScriptRuntimeException ex)
		{
			CompilationError compilationError = CompilationErrors.ScriptCompilationError with {
				StartLine = ex.Span.Start.Line + compiledTemplate.ParsedTemplate.TemplateBodyLineIndex + 1,
				StartColumn = ex.Span.Start.Column,
				EndLine = ex.Span.End.Line + compiledTemplate.ParsedTemplate.TemplateBodyLineIndex + 1,
				EndColumn = ex.Span.End.Column,
				Message = ex.Message
			};

			productionContext.AddCompilationError(
				filePath: compiledTemplate.FilePath,
				compilationError: compilationError);
			return null;
		}

		stringWriter.Flush();
		return stringWriter.ToString();
	}
}