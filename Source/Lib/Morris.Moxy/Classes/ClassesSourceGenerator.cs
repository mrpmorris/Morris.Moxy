using Microsoft.CodeAnalysis;
using Morris.Moxy.Templates;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using Scriban;
using Morris.Moxy.DataStructures;
using Scriban.Runtime;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Reflection;
using Morris.Moxy.Extensions;

namespace Morris.Moxy.Classes;

public static class ClassesSourceGenerator
{
	public static bool TryGenerateSource(
		SourceProductionContext productionContext,
		Compilation compilation,
		MetadataLoadContext reflection,
		string projectPath,
		IEnumerable<ClassInfo> classInfos,
		ImmutableDictionary<string, CompiledTemplateAndAttributeSource> nameToCompiledTemplateLookup)
	{
		foreach (var classInfo in classInfos)
		{
			using var stringWriter = new StringWriter();
			using var writer = new IndentedTextWriter(stringWriter);
			writer.WriteLine($"// Generated at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");

			Type classType =
				string.IsNullOrWhiteSpace(classInfo.Namespace)
				? reflection.ResolveType(classInfo.ClassTypeId)
				: reflection.ResolveType($"{classInfo.Namespace}.{classInfo.ClassTypeId}");

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

				writer.WriteLine($"\r\n// Generated from mixin: {templateFilePath}");

				var classMeta = new ClassMeta(
					classType,
					@namespace: classInfo.Namespace,
					name: classInfo.ClassName,
					usings: compiledTemplateAndAttributeSource.CompiledTemplate.Directives!.Value.ClassUsingClauses);

				var moxyMeta = new MoxyMeta {
					Class = classMeta
				};

				var scribanScriptObject = new ScriptObject {
					{ "moxy", moxyMeta }
				};
				AddScriptVariablesFromAttribute(
					scribanScriptObject,
					compilation,
					reflection,
					compiledTemplateAndAttributeSource,
					possibleTemplate);

				var scribanTemplateContext = new TemplateContext(scribanScriptObject) {
					MemberRenamer = m => m.Name
				};

				string generatedSource =
					compiledTemplateAndAttributeSource.CompiledTemplate.Template!.Render(scribanTemplateContext);
				writer.WriteLine(generatedSource);

			} // Template

			string source = stringWriter.ToString();
			string fullGeneratedClassName =
				classInfo.Namespace == ""
				? $"{classInfo.ClassName}"
				: $"{classInfo.Namespace}.{classInfo.ClassName}";

			string filename = $"{fullGeneratedClassName}.Moxy.g.cs"
				.Replace('<', '{')
				.Replace('>', '}');
			productionContext.AddSource(
				hintName: filename,
				source: source);
		} // class
		return true;
	}

	private static void AddScriptVariablesFromAttribute(
		ScriptObject scribanScriptObject,
		Compilation compilation,
		MetadataLoadContext reflection,
		CompiledTemplateAndAttributeSource compiledTemplateAndAttributeSource,
		AttributeNameAndSyntaxTree possibleTemplate)
	{
		SeparatedSyntaxList<AttributeArgumentSyntax>? arguments =
			possibleTemplate.AttributeSyntaxTree.ArgumentList?.Arguments;

		if (arguments is null)
			return;

		for (int argumentIndex = 0; argumentIndex < arguments.Value.Count; argumentIndex++)
		{
			AttributeArgumentSyntax argument = arguments.Value[argumentIndex];

			string argumentName =
				argument.NameEquals is not null
				? argument.NameEquals.Name.Identifier.ValueText
				: argument.NameColon is not null
				? argument.NameColon.Name.Identifier.ValueText
				: compiledTemplateAndAttributeSource.AttributeConstructorParameterNames[argumentIndex];

			if (argument.Expression.Kind() != Microsoft.CodeAnalysis.CSharp.SyntaxKind.TypeOfExpression)
			{
				string argumentValue = GetArgumentValueAsString(argument);
				scribanScriptObject.Add(argumentName, argumentValue);
			}
			else
			{
				Type argumentValue = GetArgumentValueAsType(argument, compilation, reflection);
				scribanScriptObject.Add(argumentName, argumentValue);
			}
		}
	}

	private static string GetArgumentValueAsString(AttributeArgumentSyntax argument)
	{
		string argumentValue = argument.Expression.ToFullString();

		if (argumentValue.StartsWith("\""))
			argumentValue = argumentValue.Substring(1, argumentValue.Length - 2);
		return argumentValue;
	}

	private static Type GetArgumentValueAsType(
		AttributeArgumentSyntax argument,
		Compilation compilation,
		MetadataLoadContext reflection)
	{
		var typeOfExpression = (TypeOfExpressionSyntax)argument.Expression;
		SemanticModel symbolModel = compilation.GetSemanticModel(typeOfExpression.Type.SyntaxTree)!;
		SymbolInfo symbolInfo = symbolModel.GetSymbolInfo(typeOfExpression.Type);
		var symbol = (INamedTypeSymbol)symbolInfo.Symbol!;

		string className = symbol.Name;
		string @namespace = symbol.ContainingNamespace.IsGlobalNamespace
			? string.Empty
			: $"{symbol.ContainingNamespace}.";

		Type resolvedType = reflection.ResolveType($"{@namespace}{className}");
		return resolvedType;
	}
}
