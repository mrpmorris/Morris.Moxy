using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Morris.Moxy.TemplateHandlers;
using System.Collections.Immutable;

namespace Morris.Moxy
{
	[Generator]
	public class CodeGenerator : IIncrementalGenerator
	{
		public void Initialize(IncrementalGeneratorInitializationContext context)
		{
			IncrementalValuesProvider<ValidatedResult<CompiledTemplate>> parsedTemplates =
				TemplateSelectors.Select(context.AdditionalTextsProvider);

			var combined = context.CompilationProvider.Combine(parsedTemplates.Collect());

			context.RegisterSourceOutput(
				combined,
				static (productionContext, x) =>
				{
					ImmutableArray<ValidatedResult<CompiledTemplate>> templates = x.Right;
					foreach (ValidatedResult<CompiledTemplate> template in templates)
					{
						foreach (var error in template.CompilationErrors)
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
									filePath: template.Value.FilePath,
									textSpan: new TextSpan(0, 0),
									lineSpan: new LinePositionSpan(
										start: linePosition,
										end: linePosition)));

							productionContext.ReportDiagnostic(diagnostic);
						}
					}
					productionContext.AddSource("hahaha.g.cs", "");
				});
		}
	}
}
