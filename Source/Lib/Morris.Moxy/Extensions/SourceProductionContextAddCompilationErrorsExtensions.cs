using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Morris.Moxy.Metas;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class SourceProductionContextAddCompilationErrorsExtensions
{

	public static void AddCompilationErrors(
		this SourceProductionContext productionContext,
		string filePath,
		ImmutableArray<CompilationError> compilationErrors)
	{
		for (int i = 0; i < compilationErrors.Length; i++)
		{
			CompilationError compilationError = compilationErrors[i];
			AddCompilationError(productionContext, filePath, compilationError);
		}
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void AddCompilationError(
		SourceProductionContext productionContext,
		string filePath,
		CompilationError error)
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
