using Morris.Moxy.Metas;
using Scriban;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Morris.Moxy.Extensions;

internal static class ScribanLogMessageBagToCompilationErrorsExtension
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ImmutableArray<CompilationError> ToCompilationErrors(this LogMessageBag scribanErrors, int templateBodyLineIndex) =>
		scribanErrors
		.Select(x =>
			CompilationErrors.ScriptCompilationError with {
				StartLine = x.Span.Start.Line + templateBodyLineIndex + 1,
				StartColumn = x.Span.Start.Column,
				EndLine = x.Span.End.Line + templateBodyLineIndex + 1,
				EndColumn = x.Span.End.Column,
				Message = x.Message
			})
		.ToImmutableArray();
}
