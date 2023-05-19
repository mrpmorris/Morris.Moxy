using Morris.Moxy.Metas;
using Scriban;
using System.Collections.Immutable;

namespace Morris.Moxy.Extensions;

internal static class ScribanLogMessageBagToCompilationErrorsExtensions
{
	public static ImmutableArray<CompilationError> ToCompilationErrors(this LogMessageBag scribanErrors, int templateBodyLineIndex) =>
		scribanErrors
		.Select(x =>
			CompilationErrors.ScriptCompilationError with {
				Line = x.Span.Start.Line + templateBodyLineIndex + 1,
				Column = x.Span.Start.Column,
				Message = x.Message
			})
		.ToImmutableArray();
}
