using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Morris.Moxy.SourceGenerators;
using Morris.Moxy.TemplateHandlers;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace Morris.Moxy
{
	[Generator]
	public class SourceGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> parsedTemplates =
				TemplatesSelector.Select(context.AdditionalTextsProvider);

			var config = context.AnalyzerConfigOptionsProvider.Select((x, _) =>
			{
				if (x.GlobalOptions.TryGetValue("build_property.projectdir", out string? value))
					return value!;
				return "";
			});
			var generatorInput = context.CompilationProvider
				.Combine(parsedTemplates.Collect())
				.Select((x, _) => new
				{
					Compilation = x.Left,
					ParsedTemplateResults = x.Right
				})
				.Combine(config)
				.Select((x, _) => new
				{
					x.Left.Compilation,
					x.Left.ParsedTemplateResults,
					ProjectPath = x.Right
				});

			context.RegisterSourceOutput(
				generatorInput,
				static (productionContext, x) =>
				{
#if DEBUG
					if (!System.Diagnostics.Debugger.IsAttached)
					{
						//System.Diagnostics.Debugger.Launch();
					}
#endif
					Compilation compilation = x.Compilation;
					string assemblyName = compilation.AssemblyName!;
					ImmutableArray<ValidatedResult<CompiledTemplate>> templateResults = x.ParsedTemplateResults;

					using var stringWriter = new StringWriter();
					using var writer = new IndentedTextWriter(stringWriter);

					writer.WriteLine($"// Generated {DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")} UTC");
					foreach(var templateResult in templateResults)
					{
						if (templateResult.Success)
							TemplateAttributeSourceGenerator.Generate(
								writer: writer,
								assemblyName: assemblyName,
								projectPath: x.ProjectPath,
								compiledTemplate: templateResult.Value);
						else
							AddErrors(productionContext, templateResult.Value.FilePath, templateResult);
					}

					string source = stringWriter.ToString();
					productionContext.AddSource(
						hintName: $"{assemblyName}.Moxy.TemplateAttributes.g.cs",
						source: source);
				});
		}

		private static void AddErrors<T>(
			SourceProductionContext productionContext,
			string filePath,
			ValidatedResult<T> result)
		{
			foreach (var error in result.CompilationErrors)
			{
				var descriptor = new DiagnosticDescriptor(
					id: error.Id,
					title: error.Message,
					messageFormat: error.Message,
					category: "Moxy",
					defaultSeverity: DiagnosticSeverity.Error,
					isEnabledByDefault: true);

				var linePosition = new LinePosition(
					line: error.Line,
					character: error.Column);

				Diagnostic diagnostic = Diagnostic.Create(
					descriptor: descriptor,
					location: Location.Create(
						filePath: filePath,
						textSpan: new TextSpan(0, 0),
						lineSpan: new LinePositionSpan(
							start: linePosition,
							end: linePosition)));

				productionContext.ReportDiagnostic(diagnostic);
			}
		}
	}
}
