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
	}

	public ParsedTemplate(
		string name,
		string filePath,
		int templateBodyLineNumber)
	{
		Success = false;
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		TemplateBodyLineNumber = templateBodyLineNumber;
		AttributeUsingClauses = ImmutableArray<string>.Empty;
		AttributeRequiredProperties = ImmutableArray<TemplateAttributeProperty>.Empty;
		AttributeOptionalProperties = ImmutableArray<TemplateAttributeProperty>.Empty;
		TemplateSource = null;
	}

	public override bool Equals(object? obj) => obj is ParsedTemplate other && Equals(other);

	public bool Equals(ParsedTemplate other) =>
		Success == other.Success
		&& Name == other.Name
		&& FilePath == other.FilePath
		&& TemplateSource == other.TemplateSource
		&& TemplateBodyLineNumber == other.TemplateBodyLineNumber
		&& AttributeUsingClauses.SequenceEqual(other.AttributeUsingClauses)
		&& AttributeRequiredProperties.SequenceEqual(other.AttributeRequiredProperties)
		&& AttributeOptionalProperties.SequenceEqual(other.AttributeOptionalProperties);

	public override int GetHashCode()
	{
		unchecked
		{
			int hashCode = Success.GetHashCode();
			hashCode = (hashCode * 397) ^ Name.GetHashCode();
			hashCode = (hashCode * 397) ^ FilePath.GetHashCode();
			hashCode = (hashCode * 397) ^ (TemplateSource?.GetHashCode() ?? 0);
			hashCode = (hashCode * 397) ^ TemplateBodyLineNumber;
			hashCode = (hashCode * 397) ^ AttributeUsingClauses.GetHashCode();
			hashCode = (hashCode * 397) ^ AttributeRequiredProperties.GetHashCode();
			hashCode = (hashCode * 397) ^ AttributeOptionalProperties.GetHashCode();
			return hashCode;
		}
	}

	public static bool operator ==(ParsedTemplate left, ParsedTemplate right) => left.Equals(right);
	public static bool operator !=(ParsedTemplate left, ParsedTemplate right) => !(left == right);
}
