using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Morris.Moxy.Metas;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class SourceProductionContextAddCompilationErrorsExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	public static void AddCompilationError(
		this SourceProductionContext productionContext,
		string filePath,
		CompilationError compilationError)
	{
		var descriptor = new DiagnosticDescriptor(
			id: compilationError.Id,
			title: compilationError.Message,
			messageFormat: compilationError.Message,
			category: "Moxy",
			defaultSeverity: DiagnosticSeverity.Error,
			isEnabledByDefault: true);

		var startPosition = new LinePosition(
			line: compilationError.StartLine,
			character: compilationError.StartColumn);

		var endPosition = new LinePosition(
			line: compilationError.EndLine,
			character: compilationError.EndColumn);

		var diagnostic = Diagnostic.Create(
			descriptor: descriptor,
			location: Location.Create(
				filePath: filePath,
				textSpan: new TextSpan(0, 0),
				lineSpan: new LinePositionSpan(
					start: startPosition,
					end: endPosition)));

		productionContext.ReportDiagnostic(diagnostic);
	}
}
