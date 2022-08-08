using Morris.Moxy.DataStructures;
using Morris.Moxy.TemplatePreprocessing;
using System.Collections.Immutable;
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

		public static bool TryParse(string input, out ParsedTemplate parsedTemplate)
		{
			if (input is null)
				throw new ArgumentNullException(nameof(input));

			var attributeUsings = new List<string>();
			var attributeRequiredProperties = new List<TemplateAttributeProperty>();
			var attributeOptionalProperties = new List<TemplateAttributeProperty>();
			var classUsings = new List<string>();

			bool headerMarkerDetected = false;
			using var reader = new StringReader(input);
			bool finished = false;
			while (true)
			{
				string? trimmedLine = reader.ReadLine()?.Trim();
				if (!headerMarkerDetected && trimmedLine == "") continue;

				finished = trimmedLine is null;
				if (!finished && headerMarkerDetected && trimmedLine == "---")
					finished = true;

				if (finished)
				{
					string remainingSource =
					headerMarkerDetected
					? reader.ReadToEnd()
					: input;

					parsedTemplate = new ParsedTemplate(
						attributeUsingClauses: attributeUsings.ToImmutableArray(),
						classUsingClauses: classUsings.ToImmutableArray(),
						attributeRequiredProperties: attributeRequiredProperties.ToImmutableArray(),
						attributeOptionalProperties: attributeOptionalProperties.ToImmutableArray(),
						templateSource: reader.ReadToEnd());
					return true;
				}


				if (!headerMarkerDetected)
				{
					if (trimmedLine != "---")
					{
						parsedTemplate = ParsedTemplate.Empty;
						return false;
					}
					headerMarkerDetected = true;
					continue;
				}

				if (trimmedLine == "---")
				{
				}

				var matches = Regex.Matches(trimmedLine);

				if (matches.Count == 0)
					throw new InvalidOperationException(trimmedLine);

				Match match = matches[0];
				if (match.Groups[2].Success) // attribute, using
					attributeUsings.Add(match.Groups[4].Value);
				else
				if (match.Groups[6].Success) // attribute required/optional
				{
					var property = new TemplateAttributeProperty(
						Name: match.Groups[9].Value,
						TypeName: match.Groups[8].Value,
						DefaultValue: match.Groups[10].Success ? match.Groups[10].Value : null);

					if (string.Equals("required", match.Groups[7].Value, StringComparison.OrdinalIgnoreCase))
						attributeRequiredProperties.Add(property);
					else
						attributeOptionalProperties.Add(property);
				}
				else
				if (match.Groups[13].Success) //class
					classUsings.Add(match.Groups[14].Value);
				else
					throw new NotImplementedException();
			}
		}
	}
}
