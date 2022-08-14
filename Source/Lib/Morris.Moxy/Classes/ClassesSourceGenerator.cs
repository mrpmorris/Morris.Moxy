using Microsoft.CodeAnalysis;
using Morris.Moxy.Templates;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Scriban;
using Morris.Moxy.DataStructures;

namespace Morris.Moxy.Classes;

public static class ClassesSourceGenerator
{

	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		string assemblyName,
		string projectPath,
		IEnumerable<ClassInfo> classInfos,
		ImmutableDictionary<string, CompiledTemplate> nameToCompiledTemplateLookup)
	{
		foreach (var classInfo in classInfos)
		{
			using var stringWriter = new StringWriter();
			using var writer = new IndentedTextWriter(stringWriter);

			foreach (string possibleTemplateName in classInfo.PossibleTemplateNames)
			{
				if (!nameToCompiledTemplateLookup.TryGetValue(possibleTemplateName, out CompiledTemplate compiledTemplate))
					continue;

				string templateFilePath = compiledTemplate.FilePath;
				if (templateFilePath.StartsWith(projectPath))
					templateFilePath = templateFilePath.Substring(projectPath.Length);

				writer.WriteLine($"// Generated from {templateFilePath} at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");

				var classMeta = new ClassMeta(
					@namespace: classInfo.Namespace,
					name: classInfo.Name,
					usings: compiledTemplate.Directives!.Value.ClassUsingClauses);

				var moxyMeta = new MoxyMeta {
					Class = classMeta
				};

				var scribanScriptObject = new Scriban.Runtime.ScriptObject();
				scribanScriptObject.Add("moxy", moxyMeta);
				
				var scribanTemplateContext = new TemplateContext(scribanScriptObject);
				scribanTemplateContext.MemberRenamer = m => m.Name;

				string generatedSource = compiledTemplate.Template!.Render(scribanTemplateContext);
				writer.WriteLine(generatedSource);

				string source = stringWriter.ToString();
				string fullGeneratedClassName = classInfo.Namespace == ""
					? $"{classInfo.Name}"
					: $"{classInfo.Namespace}.{classInfo.Name}";

				productionContext.AddSource(
					hintName: $"{fullGeneratedClassName}.{compiledTemplate.Name}.Moxy.g.cs",
					source: source);
			}
		}
		return true;
	}
}
