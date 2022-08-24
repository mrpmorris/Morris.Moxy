using Microsoft.CodeAnalysis;
using Morris.Moxy.Templates;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Scriban;
using Morris.Moxy.DataStructures;
using Scriban.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Morris.Moxy.Classes;

public static class ClassesSourceGenerator
{

	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		string projectPath,
		IEnumerable<ClassInfo> classInfos,
		ImmutableDictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateLookup)
	{
		foreach (var classInfo in classInfos)
		{
			using var stringWriter = new StringWriter();
			using var writer = new IndentedTextWriter(stringWriter);

			foreach (AttributeNameAndSyntaxTree possibleTemplate in classInfo.PossibleTemplates)
			{
				if (!nameToCompiledTemplateLookup.TryGetValue(
					possibleTemplate.Name,
					out CompiledTemplateAndAttributeSource compiledTemplateAndAttributeSource))
				{
					continue;
				}

				string templateFilePath = compiledTemplateAndAttributeSource.CompiledTemplate.FilePath;
				if (templateFilePath.StartsWith(projectPath))
					templateFilePath = templateFilePath.Substring(projectPath.Length);

				writer.WriteLine($"// Generated from {templateFilePath} at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");

				var classMeta = new ClassMeta(
					@namespace: classInfo.Namespace,
					name: classInfo.Name,
					usings: compiledTemplateAndAttributeSource.CompiledTemplate.Directives!.Value.ClassUsingClauses);

				var moxyMeta = new MoxyMeta {
					Class = classMeta
				};

				var scribanScriptObject = new Scriban.Runtime.ScriptObject();
				scribanScriptObject.Add("moxy", moxyMeta);
				AddScriptVariablesFromAttribute(
					scribanScriptObject,
					compiledTemplateAndAttributeSource,
					possibleTemplate);
				
				var scribanTemplateContext = new TemplateContext(scribanScriptObject);
				scribanTemplateContext.MemberRenamer = m => m.Name;

				string generatedSource =
					compiledTemplateAndAttributeSource.CompiledTemplate.Template!.Render(scribanTemplateContext);
				writer.WriteLine(generatedSource);

				string source = stringWriter.ToString();
				string fullGeneratedClassName = classInfo.Namespace == ""
					? $"{classInfo.Name}"
					: $"{classInfo.Namespace}.{classInfo.Name}";

				productionContext.AddSource(
					hintName: $"{fullGeneratedClassName}.{compiledTemplateAndAttributeSource.CompiledTemplate.Name}.Moxy.g.cs",
					source: source);
			}
		}
		return true;
	}

	private static void AddScriptVariablesFromAttribute(
		ScriptObject scribanScriptObject,
		CompiledTemplateAndAttributeSource compiledTemplateAndAttributeSource,
		AttributeNameAndSyntaxTree possibleTemplate)
	{
		SeparatedSyntaxList<AttributeArgumentSyntax>? arguments =
			possibleTemplate.AttributeSyntaxTree.ArgumentList?.Arguments;

		if (arguments is null)
			return;

#if DEBUG
		if (!System.Diagnostics.Debugger.IsAttached)
		{
			//System.Diagnostics.Debugger.Launch();
		}
#endif
		for(int argumentIndex = 0; argumentIndex < arguments.Value.Count; argumentIndex++)
		{
			AttributeArgumentSyntax argument = arguments.Value[argumentIndex];
			string argumentValue = argument.Expression.ToFullString();
			if (argumentValue.StartsWith("\""))
				argumentValue = argumentValue.Substring(1, argumentValue.Length - 2);
			string argumentName =
				argument.NameEquals is not null
				? argument.NameEquals.Name.Identifier.ValueText
				: argument.NameColon is not null
				? argument.NameColon.Name.Identifier.ValueText
				: compiledTemplateAndAttributeSource.AttributeConstructorParameterNames[argumentIndex];
			scribanScriptObject.Add(argumentName, argumentValue);
		}
	}
}
