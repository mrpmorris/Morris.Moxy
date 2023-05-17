using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Morris.Moxy.Extensions;
using System.Collections.Immutable;

namespace Morris.Moxy.Templates;

internal static class TemplateAttributeSourceGenerator
{
	public static CompiledTemplateAndAttributeSource Generate(
		string rootNamespace,
		string projectPath,
		CompiledTemplate compiledTemplate)
	{
		using var sourceCode = new StringWriter();
		using var writer = new IndentedTextWriter(sourceCode);

		string templateFilePath = compiledTemplate.FilePath;
		if (templateFilePath.StartsWith(projectPath))
			templateFilePath = templateFilePath.Substring(projectPath.Length);

		writer.WriteLine($"// Generated from {templateFilePath} at {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");

		ParsedTemplate directives = compiledTemplate.Directives!.Value;
		writer.WriteLine($"namespace {rootNamespace}");
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
						writer.WriteLine($" = {property.DefaultValue};");
				}

				if (directives.AttributeRequiredProperties.Length > 0)
				{
					writer.WriteLine();
					writer.WriteLine($"public {compiledTemplate.Name}Attribute(");
					using (writer.Indent())
					{
						string comma = ",";
						int propertyIndex = 0;
						int lastPropertyIndex = directives.AttributeRequiredProperties.Length - 1;
						foreach (TemplateAttributeProperty property in directives.AttributeRequiredProperties)
						{
							if (propertyIndex == lastPropertyIndex)
								comma = "";
							propertyIndex++;

							writer.Write($"{property.TypeName} {property.Name}");
							if (property.DefaultValue is not null)
								writer.Write($" = {property.DefaultValue}");
							writer.WriteLine(comma);
						}
					}
					writer.WriteLine(")");
					using (writer.CodeBlock())
					{
						foreach (TemplateAttributeProperty property in directives.AttributeRequiredProperties)
						{
							writer.WriteLine($"this.{property.Name} = {property.Name};");
						}
					}
				}
			} //class
		} // namespace
		writer.WriteLine();

		return CreateResult(sourceCode.ToString(), compiledTemplate);
	}

	private static CompiledTemplateAndAttributeSource CreateResult(
		string sourceCode,
		CompiledTemplate compiledTemplate)
	{
		var options = new CSharpParseOptions(LanguageVersion.Preview, kind: SourceCodeKind.Regular);
		CompilationUnitSyntax syntaxTree = SyntaxFactory.ParseCompilationUnit(sourceCode, options: options);

		var namespaceDeclarationSyntax = syntaxTree.Members.OfType<NamespaceDeclarationSyntax>().Single();
		var classDeclarationSyntax = namespaceDeclarationSyntax.Members.OfType<ClassDeclarationSyntax>().First();

		var constructorDeclarationSyntax = classDeclarationSyntax
			.Members
			.OfType<ConstructorDeclarationSyntax>()
			.FirstOrDefault();

		ImmutableArray<string> attributeConstructorParameterNames =
			constructorDeclarationSyntax is null
			? ImmutableArray.Create<string>()
			: constructorDeclarationSyntax
				.ParameterList
				.Parameters
				.Select(x => x.Identifier.ValueText)
				.ToImmutableArray();

		var result = new CompiledTemplateAndAttributeSource(
			compiledTemplate: compiledTemplate,
			attributeSource: sourceCode,
			attributeConstructorParameterNames: attributeConstructorParameterNames);

		return result;
	}
}
