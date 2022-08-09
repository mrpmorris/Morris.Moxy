using Morris.Moxy.DataStructures;
using Morris.Moxy.TemplatePreprocessing;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace Morris.Moxy
{
	public static class TemplateParser
	{
		private readonly static Regex DirectivesMarkerRegex = new Regex(
			pattern: @"\s*\-{3}\s*",
			options: RegexOptions.Compiled);

		private readonly static Regex Regex = new Regex(
			pattern:
				@"^\s*@((attribute)\s+(using)\s+(.*))$" +
				@"|((attribute)\s+(optional|required)\s+(\w+[\w\.]*)\s+(\w+)(?:\s*\=\s*(.*))?)$" +
				@"|((class)\s+(using)\s+(.*))\s*$",
			options: RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

		public static ParsedTemplate Parse(string name, string filePath, string input)
		{
			if (input is null)
				throw new ArgumentNullException(nameof(input));

			if (!TrySeparateTemplateHeaderAndBody(
				input: input,
				out ImmutableArray<(int, string)> headerLines,
				out string body,
				out CompilationError? compilationError))
			{
				return new ParsedTemplate(
					name: name,
					filePath: filePath,
					compilationErrors: Enumerable.Repeat(compilationError!.Value, 1).ToImmutableArray());
			}

			var attributeUsingClausesBuilder = ImmutableArray.CreateBuilder<string>();
			var attributeRequiredPropertiesBuilder = ImmutableArray.CreateBuilder<TemplateAttributeProperty>();
			var attributeOptionalPropertiesBuilder = ImmutableArray.CreateBuilder<TemplateAttributeProperty>();
			var classUsingClausesBuilder = ImmutableArray.CreateBuilder<string>();
			var compilationErrorsBuilder = ImmutableArray.CreateBuilder<CompilationError>();

			foreach ((int Number, string Value) headerLine in headerLines)
			{
				string trimmedHeaderLine = headerLine.Value.Trim();
				var matches = Regex.Matches(trimmedHeaderLine);

				if (matches.Count == 0)
					compilationErrorsBuilder.Add(CompilationErrors.UnknownOrMalformedHeader with { Line = headerLine.Number });
				else
				{
					Match match = matches[0];

					if (match.Groups[2].Success) // attribute, using
						attributeUsingClausesBuilder.Add(match.Groups[4].Value);
					else if (match.Groups[6].Success) // attribute required/optional
					{
						var property = new TemplateAttributeProperty(
							Name: match.Groups[9].Value,
							TypeName: match.Groups[8].Value,
							DefaultValue: match.Groups[10].Success ? match.Groups[10].Value : null);

						if (string.Equals("required", match.Groups[7].Value, StringComparison.OrdinalIgnoreCase))
							attributeRequiredPropertiesBuilder.Add(property);
						else
							attributeOptionalPropertiesBuilder.Add(property);
					}
					else if (match.Groups[13].Success) //class
						classUsingClausesBuilder.Add(match.Groups[14].Value);
				}
			}

			if (compilationErrorsBuilder.Count != 0)
				return new ParsedTemplate(
					name: name,
					filePath: filePath,
					compilationErrors: compilationErrorsBuilder.ToImmutable());

			return new ParsedTemplate(
				name: name,
				filePath: filePath,
				attributeUsingClauses: attributeUsingClausesBuilder.ToImmutable(),
				classUsingClauses: classUsingClausesBuilder.ToImmutable(),
				attributeRequiredProperties: attributeRequiredPropertiesBuilder.ToImmutable(),
				attributeOptionalProperties: attributeOptionalPropertiesBuilder.ToImmutable(),
				templateSource: body);
		}

		public static bool TrySeparateTemplateHeaderAndBody(
			string input,
			out ImmutableArray<(int, string)> headerLines,
			out string body,
			out CompilationError? compilationError)
		{
			if (input is null)
				throw new ArgumentNullException(paramName: nameof(input));

			body = "";
			var headerLinesBuilder = ImmutableArray.CreateBuilder<(int, string)>();

			ParsingState parsingState = ParsingState.Unknown;
			using var reader = new StringReader(input);
			int lineNumber = 0;
			while (true)
			{
				string? line = reader.ReadLine();
				// End of input
				if (line is null)
				{
					if (parsingState == ParsingState.InHeader)
					{
						headerLines = ImmutableArray<(int, string)>.Empty;
						compilationError = CompilationErrors.NoClosingHeaderMarkerFound with { Line = lineNumber };
						return false;
					}
					headerLines = headerLinesBuilder.ToImmutable();
					compilationError = null;
					return true;
				}

				// Next line
				lineNumber++;
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
}
