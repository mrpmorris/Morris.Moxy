using Morris.Moxy.Extensions;
using Morris.Moxy.Helpers;
using System.Collections.Immutable;

namespace Morris.Moxy.Templates;

public readonly struct ParsedTemplate : IEquatable<ParsedTemplate>
{
	public readonly bool Success;
	public readonly string Name;
	public readonly string FilePath;
	public readonly string? TemplateSource;
	public readonly int TemplateBodyLineNumber;
	public readonly ImmutableArray<string> AttributeUsingClauses;
	public readonly ImmutableArray<TemplateAttributeProperty> AttributeRequiredProperties;
	public readonly ImmutableArray<TemplateAttributeProperty> AttributeOptionalProperties;

	public static readonly ParsedTemplate Empty = new();

	private readonly Lazy<int> CachedHashCode;

	public ParsedTemplate(
		string name,
		string filePath,
		int templateBodyLineNumber,
		ImmutableArray<string> attributeUsingClauses,
		ImmutableArray<TemplateAttributeProperty> attributeRequiredProperties,
		ImmutableArray<TemplateAttributeProperty> attributeOptionalProperties,
		string templateSource)
	{
		Success = true;
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		TemplateBodyLineNumber = templateBodyLineNumber;
		AttributeUsingClauses = attributeUsingClauses;
		AttributeRequiredProperties = attributeRequiredProperties;
		AttributeOptionalProperties = attributeOptionalProperties;
		TemplateSource = templateSource ?? throw new ArgumentNullException(nameof(templateSource));

		CachedHashCode = new Lazy<int>(() => 
			HashCode.Combine(
				true,
				name,
				filePath,
				templateBodyLineNumber,
				attributeUsingClauses.GetContentHashCode(),
				attributeRequiredProperties.GetContentHashCode(),
				attributeOptionalProperties.GetContentHashCode())); 
	}

	public ParsedTemplate(
		string name,
		string filePath,
		int templateBodyLineNumber,
		string templateSource)
	{
		Success = false;
		Name = name;
		FilePath = filePath;
		TemplateSource = templateSource;
		TemplateBodyLineNumber = templateBodyLineNumber;
		AttributeUsingClauses = ImmutableArray.Create<string>();
		AttributeRequiredProperties = ImmutableArray.Create<TemplateAttributeProperty>();
		AttributeOptionalProperties = ImmutableArray.Create<TemplateAttributeProperty>();
		TemplateSource = null;

		CachedHashCode = new Lazy<int>(() =>
			HashCode.Combine(
				false,
				name,
				filePath,
				templateBodyLineNumber,
				templateSource));

	}

	public override bool Equals(object obj) => obj is ParsedTemplate parsedTemplate && Equals(parsedTemplate);

	public bool Equals(ParsedTemplate other) =>
		other.Success == Success
		&& other.Name == Name
		&& other.FilePath == FilePath
		&& other.TemplateBodyLineNumber == TemplateBodyLineNumber
		&& other.TemplateSource == TemplateSource
		&& other.AttributeUsingClauses.SequenceEqual(AttributeUsingClauses)
		&& other.AttributeRequiredProperties.SequenceEqual(AttributeRequiredProperties)
		&& other.AttributeOptionalProperties.SequenceEqual(AttributeOptionalProperties);

	public override int GetHashCode() => CachedHashCode.Value;
		

	public static bool operator ==(ParsedTemplate left, ParsedTemplate right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(ParsedTemplate left, ParsedTemplate right)
	{
		return !(left == right);
	}

	private int CalculateHashCode() =>
		HashCode.Combine(
			Success,
			Name,
			FilePath,
			TemplateBodyLineNumber,
			TemplateSource,
			AttributeUsingClauses.GetContentHashCode(),
			AttributeRequiredProperties.GetContentHashCode(),
			AttributeOptionalProperties.GetContentHashCode());
}
