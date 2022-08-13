using Microsoft.CodeAnalysis;
using Morris.Moxy.Templates;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Morris.Moxy.Extensions;
using Scriban;

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
			writer.WriteLine($"// Generated {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");
			bool hasWrittenNamespace = false;

			foreach (string possibleTemplateName in classInfo.PossibleTemplateNames)
			{
				if (!nameToCompiledTemplateLookup.TryGetValue(possibleTemplateName, out CompiledTemplate compiledTemplate))
					continue;
				if (!hasWrittenNamespace)
				{
					hasWrittenNamespace = true;
					writer.WriteLine($"namespace {classInfo.Namespace}");
					writer.WriteLine("{");
					writer.Indent++;
					foreach (string classUsing in compiledTemplate.Directives!.Value.ClassUsingClauses)
						writer.WriteLine($"using {classUsing};");
					
					writer.WriteLine();
					writer.WriteLine($"partial class {classInfo.Name}");
					writer.WriteLine("{");
					writer.Indent++;
				}

				var scribanTemplateContext = new TemplateContext();
				scribanTemplateContext.MemberRenamer = m => m.Name;
				scribanTemplateContext.AutoIndent = true;
				scribanTemplateContext.CurrentIndent = "        ";

				string generatedSource = compiledTemplate.Template!.Render(scribanTemplateContext);
				writer.WriteLine(generatedSource);
				writer.WriteLine();
			}
			if (hasWrittenNamespace)
			{
				writer.Indent--;
				writer.WriteLine($"}} // {classInfo.Name}");
				writer.Indent--;
				writer.WriteLine("}");

				string source = stringWriter.ToString();
				string fullGeneratedClassName = classInfo.Namespace == ""
					? $"{classInfo.Name}"
					: $"{classInfo.Namespace}.{classInfo.Name}";

				productionContext.AddSource(
					hintName: $"{fullGeneratedClassName}.Moxy.g.cs",
					source: source);
			}
		}
		return true;
	}
}
