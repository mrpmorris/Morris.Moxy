using Morris.Moxy.TemplatePreprocessing;
using System.Collections.Immutable;

namespace Morris.Moxy.DataStructures;

public readonly struct ParsedTemplate
{
	public readonly ImmutableArray<string> AttributeUsingClauses;
	public readonly ImmutableArray<string> ClassUsingClauses;
	public readonly ImmutableArray<TemplateAttributeProperty> AttributeRequiredProperties;
	public readonly ImmutableArray<TemplateAttributeProperty> AttributeOptionalProperties;
	public readonly ImmutableArray<CompilationError> CompilationErrors;
	public readonly string? TemplateSource;

	public static readonly ParsedTemplate Empty = new();

	public ParsedTemplate(
		ImmutableArray<string> attributeUsingClauses,
		ImmutableArray<string> classUsingClauses,
		ImmutableArray<TemplateAttributeProperty> attributeRequiredProperties,
		ImmutableArray<TemplateAttributeProperty> attributeOptionalProperties,
		string templateSource)
	{
		AttributeUsingClauses = attributeUsingClauses;
		ClassUsingClauses = classUsingClauses;
		AttributeRequiredProperties = attributeRequiredProperties;
		AttributeOptionalProperties = attributeOptionalProperties;
		TemplateSource = templateSource ?? throw new ArgumentNullException(nameof(templateSource));
		CompilationErrors = ImmutableArray<CompilationError>.Empty;
	}

	public ParsedTemplate(ImmutableArray<CompilationError> compilationErrors)
	{
		if (compilationErrors.Length == 0)
			throw new ArgumentException(paramName: nameof(compilationErrors), message: "Cannot be empty");

		AttributeUsingClauses = ImmutableArray<string>.Empty;
		ClassUsingClauses = ImmutableArray<string>.Empty;
		AttributeRequiredProperties = ImmutableArray<TemplateAttributeProperty>.Empty;
		AttributeOptionalProperties = ImmutableArray<TemplateAttributeProperty>.Empty;
		TemplateSource = null;
		CompilationErrors = compilationErrors;
	}
}
