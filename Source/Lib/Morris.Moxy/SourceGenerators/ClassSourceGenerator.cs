using Microsoft.CodeAnalysis;
using Morris.Moxy.Extensions;
using Morris.Moxy.Metas;
using Morris.Moxy.Metas.Classes;
using Morris.Moxy.Metas.ScriptVariables;
using Morris.Moxy.Metas.Templates;
using Scriban;
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

		var classVariable = new ClassVariable(
			name: classMeta.ClassName,
			@namespace: classMeta.Namespace);
		var moxyVariable = new MoxyVariable(@class: classVariable);

		var scribanScriptObject = ScribanTemplateContext.GetDefaultBuiltinObject();
		scribanScriptObject.Add(
			key: "moxy",
			value: moxyVariable);

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
				Message = ex.Message
			};

			productionContext.AddCompilationError(
				filePath: compiledTemplate.FilePath,
				compilationError: compilationError);
			stringWriter.WriteLine("/* An error occurred executing your mixin script");
			stringWriter.WriteLine(ex.ToString());
			stringWriter.WriteLine("*/");
		}

		return stringWriter.ToString();
	}
}