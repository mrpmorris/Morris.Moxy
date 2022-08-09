using Morris.Moxy.TemplatePreprocessing;
using System.Collections.Immutable;

namespace Morris.Moxy.DataStructures;

public readonly struct ParsedTemplate
{
	public readonly bool Success;
	public readonly string Name;
	public readonly string FilePath;
	public readonly string? TemplateSource;
	public readonly ImmutableArray<string> AttributeUsingClauses;
	public readonly ImmutableArray<string> ClassUsingClauses;
	public readonly ImmutableArray<TemplateAttributeProperty> AttributeRequiredProperties;
	public readonly ImmutableArray<TemplateAttributeProperty> AttributeOptionalProperties;

	public static readonly ParsedTemplate Empty = new();

	public ParsedTemplate(
		string name,
		string filePath,
		ImmutableArray<string> attributeUsingClauses,
		ImmutableArray<string> classUsingClauses,
		ImmutableArray<TemplateAttributeProperty> attributeRequiredProperties,
		ImmutableArray<TemplateAttributeProperty> attributeOptionalProperties,
		string templateSource)
	{
		Success = true;
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		AttributeUsingClauses = attributeUsingClauses;
		ClassUsingClauses = classUsingClauses;
		AttributeRequiredProperties = attributeRequiredProperties;
		AttributeOptionalProperties = attributeOptionalProperties;
		TemplateSource = templateSource ?? throw new ArgumentNullException(nameof(templateSource));
	}

	public ParsedTemplate(
		string name,
		string filePath)
	{
		Success = false;
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
		AttributeUsingClauses = ImmutableArray<string>.Empty;
		ClassUsingClauses = ImmutableArray<string>.Empty;
		AttributeRequiredProperties = ImmutableArray<TemplateAttributeProperty>.Empty;
		AttributeOptionalProperties = ImmutableArray<TemplateAttributeProperty>.Empty;
		TemplateSource = null;
	}
}
