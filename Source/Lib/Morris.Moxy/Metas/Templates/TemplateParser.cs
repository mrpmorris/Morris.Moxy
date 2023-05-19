using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Morris.Moxy.Metas.Templates;

internal static class TemplateParser
{
	private readonly static Regex Regex = new Regex(
		pattern:
			@"^\s*@((attribute)\s+(using)\s+(.*))$" +
			@"|((attribute)\s+(optional|required)\s+(\w+[\w\.]*)\s+(\w+)(?:\s*\=\s*(.*))?)$",
		options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

	public static ValidatedResult<ParsedTemplate> Parse(string name, string filePath, string source)
	{
		if (!TrySeparateTemplateHeaderAndBody(
			input: source,
			out ImmutableArray<(int, string)> headerLines,
			out string body,
			out int templateBodyLineNumber,
			out CompilationError? compilationError))
		{
			return (filePath, compilationError!.Value);
		}

		var attributeUsingClausesBuilder = ImmutableArray.CreateBuilder<string>();
		var attributeRequiredPropertiesBuilder = ImmutableArray.CreateBuilder<TemplateInput>();
		var attributeOptionalPropertiesBuilder = ImmutableArray.CreateBuilder<TemplateInput>();
		var compilationErrorsBuilder = ImmutableArray.CreateBuilder<CompilationError>();

		foreach ((int Number, string Value) headerLine in headerLines)
		{
			var matches = Regex.Matches(headerLine.Value);

			if (matches.Count == 0)
				compilationErrorsBuilder.Add(CompilationErrors.UnknownOrMalformedHeader with { 
					StartLine = headerLine.Number, 
					StartColumn = 1,
					EndLine = headerLine.Number,
					EndColumn = headerLine.Value.Length
				});
			else
			{
				Match match = matches[0];

				if (match.Groups[2].Success) // attribute, using
					attributeUsingClausesBuilder.Add(match.Groups[4].Value);
				else if (match.Groups[6].Success) // attribute required/optional
				{
					var property = new TemplateInput(
						name: match.Groups[9].Value,
						typeName: match.Groups[8].Value,
						defaultValue: match.Groups[10].Success ? match.Groups[10].Value : null);

					if (string.Equals("required", match.Groups[7].Value, StringComparison.OrdinalIgnoreCase))
						attributeRequiredPropertiesBuilder.Add(property);
					else
						attributeOptionalPropertiesBuilder.Add(property);
				}
			}
		}

		if (compilationErrorsBuilder.Count != 0)
			return (filePath, compilationErrorsBuilder.ToImmutable());

		var parsedTemplate = new ParsedTemplate(
			name: name,
			filePath: filePath,
			templateBodyLineIndex: templateBodyLineNumber,
			attributeUsingClauses: attributeUsingClausesBuilder.ToImmutable(),
			requiredInputs: attributeRequiredPropertiesBuilder.ToImmutable(),
			optionalInputs: attributeOptionalPropertiesBuilder.ToImmutable(),
			templateSource: body);

		return (filePath, parsedTemplate);
	}

	public static bool TrySeparateTemplateHeaderAndBody(
		string input,
		out ImmutableArray<(int, string)> headerLines,
		out string body,
		out int templateBodyLineNumber,
		out CompilationError? compilationError)
	{
		if (input is null)
			throw new ArgumentNullException(paramName: nameof(input));

		body = "";
		var headerLinesBuilder = ImmutableArray.CreateBuilder<(int, string)>();

		ParsingState parsingState = ParsingState.Unknown;
		using var reader = new StringReader(input);
		int lineNumber = -1;
		templateBodyLineNumber = 0;
		while (true)
		{
			// Next line
			lineNumber++;

			string? line = reader.ReadLine();
			// End of input
			if (line is null)
			{
				if (parsingState == ParsingState.InHeader)
				{
					headerLines = ImmutableArray.Create<(int, string)>();
					compilationError = CompilationErrors.NoClosingHeaderMarkerFound with { StartLine = lineNumber, EndLine = lineNumber };
					return false;
				}
				headerLines = headerLinesBuilder.ToImmutable();
				compilationError = null;
				return true;
			}

			switch (parsingState)
			{
				case ParsingState.Unknown:
					if (line.TrimEnd() == "@moxy")
						parsingState = ParsingState.InHeader;
					else
					{
						// An input with headers cannot have any non-whitespace before @moxy
						// If we get anything that is not whitespace before we find @moxy
						// then we return the input
						if (!string.IsNullOrWhiteSpace(line))
						{
							body = input;
							templateBodyLineNumber = 0;
							reader.ReadToEnd(); // Go to end of input
							parsingState = ParsingState.Finished;
						}
					}
					break;

				case ParsingState.InHeader:
					if (line.TrimEnd() == "@moxy")
					{
						// If we are in the header and find a closing @moxy then the rest of the input
						// is the body
						templateBodyLineNumber = lineNumber;
						body = reader.ReadToEnd();
						parsingState = ParsingState.Finished;
					}
					else
					{
						// Ignore comments in the header
						if (line.TrimStart().StartsWith("//")) continue;

						// Keep track of the line number + header value
						headerLinesBuilder.Add((lineNumber, line));
					}
					break;

				default: throw new NotImplementedException(parsingState.ToString());
			}
		}
	}

	private enum ParsingState
	{
		Unknown,
		InHeader,
		Finished
	}
}

