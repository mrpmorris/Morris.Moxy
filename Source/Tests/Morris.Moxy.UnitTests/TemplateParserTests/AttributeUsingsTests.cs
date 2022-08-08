using Morris.Moxy.DataStructures;
using Morris.Moxy.TemplatePreprocessing;

namespace Morris.Moxy.UnitTests.TemplateParserTests;

public class AttributeUsingsTests
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

	const string ClassUsingClauses =
		"@class using Newtonsoft.Json\r\n" +
		"@class using System.Text.Json\r\n";


	[Fact]
	public void WhenSourceContainsAttributeUsingClauses_ThenResultShouldContainsAttributeUsingClauses()
	{
		string source = $"---\r\n{AttributeUsingClauses}---\r\n";
		bool success = TemplateParser.TryParse(source, out ParsedTemplate parsedTemplate);

		Assert.True(success);
		Assert.Empty(parsedTemplate.ClassUsingClauses);
		Assert.Empty(parsedTemplate.AttributeOptionalProperties);
		Assert.Empty(parsedTemplate.AttributeRequiredProperties);
		Assert.Empty(parsedTemplate.TemplateSource);
		Assert.True(
			Enumerable.SequenceEqual(
				parsedTemplate.AttributeUsingClauses,
				new string[] { "System", "System.Text" }));
	}

	[Fact]
	public void WhenSourceContainsRequiredPropertiesWithoutDefaultValues_ThenResultShouldContainRequiredPropertiesWithoutDefaultValues()
	{
		string source = $"---\r\n{AttributeRequiredPropertiesWithoutDefaultValues}---\r\n";
		bool success = TemplateParser.TryParse(source, out ParsedTemplate parsedTemplate);

		Assert.True(success);
		Assert.Empty(parsedTemplate.ClassUsingClauses);
		Assert.Empty(parsedTemplate.AttributeUsingClauses);
		Assert.Empty(parsedTemplate.AttributeOptionalProperties);
		Assert.Empty(parsedTemplate.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "Age",
				TypeName: "int",
				DefaultValue: null),
			parsedTemplate.AttributeRequiredProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "Name",
				TypeName: "string",
				DefaultValue: null),
			parsedTemplate.AttributeRequiredProperties[1]);
	}

	[Fact]
	public void WhenSourceContainsRequiredPropertiesWithDefaultValues_ThenResultShouldContainRequiredPropertiesWithDefaultValues()
	{
		string source = $"---\r\n{AttributeRequiredPropertiesWithDefaultValues}---\r\n";
		bool success = TemplateParser.TryParse(source, out ParsedTemplate parsedTemplate);

		Assert.True(success);

		Assert.Empty(parsedTemplate.ClassUsingClauses);
		Assert.Empty(parsedTemplate.AttributeUsingClauses);
		Assert.Empty(parsedTemplate.AttributeOptionalProperties);
		Assert.Empty(parsedTemplate.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "GivenName",
				TypeName: "string",
				DefaultValue: "\"Peter\""),
			parsedTemplate.AttributeRequiredProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "FamilyName",
				TypeName: "string",
				DefaultValue: "\"Morris\""),
			parsedTemplate.AttributeRequiredProperties[1]);
	}

	[Fact]
	public void WhenSourceContainsOptionalPropertiesWithoutDefaultValues_ThenResultShouldContainOptionalPropertiesWithoutDefaultValues()
	{
		string source = $"---\r\n{AttributeOptionalPropertiesWithoutDefaultValues}---\r\n";
		bool success = TemplateParser.TryParse(source, out ParsedTemplate parsedTemplate);

		Assert.True(success);
		Assert.Empty(parsedTemplate.ClassUsingClauses);
		Assert.Empty(parsedTemplate.AttributeUsingClauses);
		Assert.Empty(parsedTemplate.AttributeRequiredProperties);
		Assert.Empty(parsedTemplate.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalIntNoDefault",
				TypeName: "int",
				DefaultValue: null),
			parsedTemplate.AttributeOptionalProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalStringNoDefault",
				TypeName: "string",
				DefaultValue: null),
			parsedTemplate.AttributeOptionalProperties[1]);
	}

	[Fact]
	public void WhenSourceContainsOptionalPropertiesWithDefaultValues_ThenResultShouldContainOptionalPropertiesWithDefaultValues()
	{
		string source = $"---\r\n{AttributeOptionalPropertiesWithDefaultValues}---\r\n";
		bool success = TemplateParser.TryParse(source, out ParsedTemplate parsedTemplate);

		Assert.True(success);

		Assert.Empty(parsedTemplate.ClassUsingClauses);
		Assert.Empty(parsedTemplate.AttributeUsingClauses);
		Assert.Empty(parsedTemplate.AttributeRequiredProperties);
		Assert.Empty(parsedTemplate.TemplateSource);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalIntWithDefault",
				TypeName: "int",
				DefaultValue: "42"),
			parsedTemplate.AttributeOptionalProperties[0]);

		Assert.Equal(
			new TemplateAttributeProperty(
				Name: "OptionalStringWithDefault",
				TypeName: "string",
				DefaultValue: "\"Eggs are nice\""),
			parsedTemplate.AttributeOptionalProperties[1]);
	}

	[Fact]
	public void WhenSourceContainsClassUsingClauses_ThenResultShouldContainsClassUsingClauses()
	{
		string source = $"---\r\n{ClassUsingClauses}---\r\n";
		bool success = TemplateParser.TryParse(source, out ParsedTemplate parsedTemplate);

		Assert.True(success);
		Assert.Empty(parsedTemplate.AttributeUsingClauses);
		Assert.Empty(parsedTemplate.AttributeOptionalProperties);
		Assert.Empty(parsedTemplate.AttributeRequiredProperties);
		Assert.Empty(parsedTemplate.TemplateSource);
		Assert.True(
			Enumerable.SequenceEqual(
				parsedTemplate.ClassUsingClauses,
				new string[] { "Newtonsoft.Json", "System.Text.Json" }));
	}
}
