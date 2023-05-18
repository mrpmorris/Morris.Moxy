using Morris.Moxy.Metas.Templates;
using System.CodeDom.Compiler;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.SourceGenerators;

internal static class TemplateAttributeSourceGenerator
{
	public static string Generate(
		string rootNamespace,
		string projectPath,
		ParsedTemplate parsedTemplate)
	{
		using var sourceCode = new StringWriter();
		using var writer = new IndentedTextWriter(sourceCode);

		string templateFilePath = parsedTemplate.FilePath;
		if (templateFilePath.StartsWith(projectPath))
			templateFilePath = templateFilePath.Substring(projectPath.Length);

		writer.WriteLine($"// Generated from {templateFilePath} at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");

		writer.WriteLine($"namespace {rootNamespace}");
		using (writer.CodeBlock())
		{
			foreach (string attributeUsingClause in parsedTemplate.AttributeUsingClauses)
				writer.WriteLine($"using {attributeUsingClause};");
			writer.WriteLine();

			writer.WriteLine("[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]");
			writer.WriteLine($"internal class {parsedTemplate.Name}Attribute : Attribute");
			using (writer.CodeBlock())
			{
				var allTemplateInputs = parsedTemplate.RequiredInputs.Union(parsedTemplate.OptionalInputs);
				foreach (TemplateInput templateInput in allTemplateInputs)
				{
					writer.Write($"public {templateInput.TypeName} {templateInput.Name} {{ get; set; }}");
					if (templateInput.DefaultValue is null)
						writer.WriteLine("");
					else
						writer.WriteLine($" = {templateInput.DefaultValue};");
				}

				if (parsedTemplate.RequiredInputs.Length > 0)
				{
					writer.WriteLine();
					writer.WriteLine($"public {parsedTemplate.Name}Attribute(");
					using (IndentedTextWriterIndentExtensions.IndentedBlock(writer))
					{
						int propertyIndex = 0;
						int finalPropertyIndex = parsedTemplate.RequiredInputs.Length - 1;
						foreach (TemplateInput requiredTemplateInput in parsedTemplate.RequiredInputs)
						{
							writer.Write($"{requiredTemplateInput.TypeName} {requiredTemplateInput.Name}");
							
							if (requiredTemplateInput.DefaultValue is not null)
								writer.Write($" = {requiredTemplateInput.DefaultValue}");

							if (propertyIndex != finalPropertyIndex)
								writer.WriteLine(",");

							propertyIndex++;
						}
					}
					writer.WriteLine(")");

					using (writer.CodeBlock())
					{
						foreach (TemplateInput requiredTemplateInput in parsedTemplate.RequiredInputs)
						{
							writer.WriteLine($"this.{requiredTemplateInput.Name} = {requiredTemplateInput.Name};");
						}
					}
				}
			} //class
		} // namespace
		writer.WriteLine();

		writer.Flush();
		return sourceCode.ToString();
	}
}

