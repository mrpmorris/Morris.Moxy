﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Morris.Moxy.Metas;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class SourceProductionContextAddCompilationErrorsExtensions
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

		var linePosition = new LinePosition(
			line: compilationError.Line,
			character: compilationError.Column);

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
