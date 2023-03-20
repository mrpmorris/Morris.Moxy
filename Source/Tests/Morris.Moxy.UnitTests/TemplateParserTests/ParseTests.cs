using Morris.Moxy.Templates;

namespace Morris.Moxy.UnitTests.TemplateParserTests;

public class ParseTests
{
	const string AttributeUsingClauses =
		"@attribute using System\r\n" +
		"@attribute using System.Text\r\n";

	const string AttributeRequiredPropertiesWithoutDefaultValues =
		"@attribute required int Age\r\n" +
		"@attribute required string Name\r\n";

	const string AttributeRequiredPropertiesWithDefaultValues =
		"@attribute required string GivenName = \"Peter\"\r\n" +
		"@attribute required string FamilyName = \"Morris\"\r\n";

	const string AttributeOptionalPropertiesWithoutDefaultValues =
		"@attribute optional int OptionalIntNoDefault\r\n" +
		"@attribute optional string OptionalStringNoDefault\r\n";

	const string AttributeOptionalPropertiesWithDefaultValues =
		"@attribute optional int OptionalIntWithDefault = 42\r\n" +
		"@attribute optional string OptionalStringWithDefault = \"Eggs are nice\"\r\n";

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceContainsAttributeUsingClauses_ThenResultShouldContainsAttributeUsingClauses(string inputPadding)
	{
		string source = $"{inputPadding}@moxy\r\n{AttributeUsingClauses}@moxy\r\n";
		ValidatedResult<ParsedTemplate> parsedTemplate = TemplateParser.Parse("", "", source);

		Assert.True(parsedTemplate.Success);
		Assert.Empty(parsedTemplate.Value.AttributeOptionalProperties);
		Assert.Empty(parsedTemplate.Value.AttributeRequiredProperties);
		Assert.Equal("", parsedTemplate.Value.TemplateSource);
		Assert.True(
			Enumerable.SequenceEqual(
				parsedTemplate.Value.AttributeUsingClauses,
				new string[] { "System", "System.Text" }));
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceContainsRequiredPropertiesWithoutDefaultValues_ThenResultShouldContainRequiredPropertiesWithoutDefaultValues(string inputPadding)
	{
		string source = $"{inputPadding}@moxy\r\n{AttributeRequiredPropertiesWithoutDefaultValues}@moxy\r\n";
		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", source);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeOptionalProperties);
		Assert.Equal("", result.Value.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "Age",
				TypeName: "int",
				DefaultValue: null),
			result.Value.AttributeRequiredProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "Name",
				TypeName: "string",
				DefaultValue: null),
			result.Value.AttributeRequiredProperties[1]);
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceContainsRequiredPropertiesWithDefaultValues_ThenResultShouldContainRequiredPropertiesWithDefaultValues(string inputPadding)
	{
		string source = $"{inputPadding}@moxy\r\n{AttributeRequiredPropertiesWithDefaultValues}@moxy\r\n";
		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", source);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeOptionalProperties);
		Assert.Equal("", result.Value.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "GivenName",
				TypeName: "string",
				DefaultValue: "\"Peter\""),
			result.Value.AttributeRequiredProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "FamilyName",
				TypeName: "string",
				DefaultValue: "\"Morris\""),
			result.Value.AttributeRequiredProperties[1]);
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceContainsOptionalPropertiesWithoutDefaultValues_ThenResultShouldContainOptionalPropertiesWithoutDefaultValues(string inputPadding)
	{
		string source = $"{inputPadding}@moxy\r\n{AttributeOptionalPropertiesWithoutDefaultValues}@moxy\r\n";
		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", source);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeRequiredProperties);
		Assert.Equal("", result.Value.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalIntNoDefault",
				TypeName: "int",
				DefaultValue: null),
			result.Value.AttributeOptionalProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalStringNoDefault",
				TypeName: "string",
				DefaultValue: null),
			result.Value.AttributeOptionalProperties[1]);
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceContainsOptionalPropertiesWithDefaultValues_ThenResultShouldContainOptionalPropertiesWithDefaultValues(string inputPadding)
	{
		string source = $"{inputPadding}@moxy\r\n{AttributeOptionalPropertiesWithDefaultValues}@moxy\r\n";
		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", source);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeRequiredProperties);
		Assert.Equal("", result.Value.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalIntWithDefault",
				TypeName: "int",
				DefaultValue: "42"),
			result.Value.AttributeOptionalProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalStringWithDefault",
				TypeName: "string",
				DefaultValue: "\"Eggs are nice\""),
			result.Value.AttributeOptionalProperties[1]);
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
<<<<<<< Updated upstream
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceContainsClassUsingClauses_ThenResultShouldContainsClassUsingClauses(string inputPadding)
	{
		string source = $"{inputPadding}@moxy\r\n{ClassUsingClauses}@moxy\r\n";
		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", source);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeOptionalProperties);
		Assert.Empty(result.Value.AttributeRequiredProperties);
		Assert.Equal("", result.Value.TemplateSource);
		Assert.True(
			Enumerable.SequenceEqual(
				result.Value.ClassUsingClauses,
				new string[] { "Newtonsoft.Json", "System.Text.Json" }));
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
=======
>>>>>>> Stashed changes
	[InlineData("\r")]
	[InlineData("\n")]
	public void WhenSourceHasNoHeaderMarker_ThenResultTemplateSourceShouldEqualTheInputString(string inputTerminator)
	{
		string expected = $"1\r\n2\r\n3${inputTerminator}";
		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", expected);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeOptionalProperties);
		Assert.Empty(result.Value.AttributeRequiredProperties);
		Assert.Equal(result.Value.TemplateSource, expected);
	}

	[Fact]
	public void WhenSourceHasNonWhitespaceBeforeHeaderMarker_ThenResultTemplateSourceShouldEqualTheInputString()
	{
		string expected = "hello\r\n@moxy\r\n@attribute using System\r\n@moxy\r\nHello";

		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", expected);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeOptionalProperties);
		Assert.Empty(result.Value.AttributeRequiredProperties);
		Assert.Equal(result.Value.TemplateSource, expected);
	}

	/// <summary>
	/// If there is non-whitespace before the first @moxy then this is all just template body,
	/// so a missing @moxy is not an error
	/// </summary>
	[Fact]
	public void WhenSourceHasNonWhitespaceBeforeHeaderMarker_ThenMissingHeaderMarkerIsIgnored()
	{
		string expected = "hello\r\n@moxy\r\n@attribute using System\r\nHello";

		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", expected);

		Assert.True(result.Success);
		Assert.Empty(result.Value.AttributeUsingClauses);
		Assert.Empty(result.Value.AttributeOptionalProperties);
		Assert.Empty(result.Value.AttributeRequiredProperties);
		Assert.Equal(result.Value.TemplateSource, expected);
	}

	[Theory]
	[InlineData("")]
	[InlineData("\r\n")]
	[InlineData("  \r\n")]
	[InlineData("  \r\n  \r\n")]
	public void WhenSourceHasHeaders_ThenResultTemplateSourceShouldEqualTheInputStringAfterTheClosingHeaderMarker(string inputPadding)
	{
		string source = $"{inputPadding}" +
			$"@moxy\r\n" +
			$"{AttributeUsingClauses}" +
			$"{AttributeRequiredPropertiesWithoutDefaultValues}" +
			$"{AttributeRequiredPropertiesWithDefaultValues}" +
			$"{AttributeOptionalPropertiesWithoutDefaultValues}" +
			$"{AttributeOptionalPropertiesWithDefaultValues}" +
			$"@moxy\r\n" +
			$"Hello!";

		ValidatedResult<ParsedTemplate> result = TemplateParser.Parse("", "", source);

		Assert.True(result.Success);
		Assert.Equal(2, result.Value.AttributeUsingClauses.Length);
		Assert.Equal(4, result.Value.AttributeOptionalProperties.Length);
		Assert.Equal(4, result.Value.AttributeRequiredProperties.Length);
		Assert.Equal("Hello!", result.Value.TemplateSource);
	}
}
