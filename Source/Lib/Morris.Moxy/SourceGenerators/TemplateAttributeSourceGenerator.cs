using System.CodeDom.Compiler;
using Morris.Moxy.Extensions;
using Morris.Moxy.TemplatePreprocessing;

namespace Morris.Moxy.SourceGenerators;

internal static class TemplateAttributeSourceGenerator
{
	public static void Generate(
		IndentedTextWriter writer,
		string assemblyName,
		string projectPath,
		CompiledTemplate compiledTemplate)
	{
		string templateFilePath = compiledTemplate.FilePath;
		if (templateFilePath.StartsWith(projectPath))
			templateFilePath = templateFilePath.Substring(projectPath.Length);

		writer.WriteLine($"// Generated from {templateFilePath}");

		ParsedTemplate directives = compiledTemplate.Directives!.Value;
		writer.WriteLine($"namespace {assemblyName}");
		using (writer.CodeBlock())
		{
			foreach (string attributeUsingClause in compiledTemplate.Directives!.Value.AttributeUsingClauses)
				writer.WriteLine($"using {attributeUsingClause};");
			writer.WriteLine();

			writer.WriteLine($"internal class {compiledTemplate.Name}Attribute : Attribute");
			using (writer.CodeBlock())
			{
				var allProperties = directives.AttributeRequiredProperties.Union(directives.AttributeOptionalProperties);
				foreach (TemplateAttributeProperty property in allProperties)
				{
					writer.Write($"public {property.TypeName} {property.Name} {{ get; set; }}");
					if (property.DefaultValue is null)
						writer.WriteLine("");
					else
						writer.WriteLine($" = {property.DefaultValue}");
				}

				if (directives.AttributeRequiredProperties.Length > 0)
				{
					writer.WriteLine();
					writer.WriteLine($"public {compiledTemplate.Name}Attribute(");
					using (writer.Indent())
					{
						foreach (TemplateAttributeProperty property in directives.AttributeRequiredProperties)
						{
							writer.Write($"{property.TypeName} {property.Name}");
							if (property.DefaultValue is null)
								writer.WriteLine("");
							else
								writer.WriteLine($" = {property.DefaultValue}");
						}
					}
					writer.WriteLine(")");
					using (writer.CodeBlock())
					{
						foreach(TemplateAttributeProperty property in directives.AttributeRequiredProperties)
						{
							writer.WriteLine($"this.{property.Name} = {property.Name};");
						}
					}
				}
			} //class
		} // namespace
		writer.WriteLine();
	}
}
