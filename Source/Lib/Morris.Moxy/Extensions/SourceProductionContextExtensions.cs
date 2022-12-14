using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;

namespace Morris.Moxy.Extensions
{
	public static class SourceProductionContextExtensions
	{
		public static void AddCompilationError(
			this SourceProductionContext productionContext,
			string filePath,
			CompilationError error)
		{
			productionContext.AddCompilationErrors(filePath, Enumerable.Repeat(error, 1));
		}

		public static void AddCompilationErrors(
			this SourceProductionContext productionContext,
			string filePath,
			IEnumerable<CompilationError> errors)
		{
			foreach (var error in errors)
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

				var diagnostic = Diagnostic.Create(
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
