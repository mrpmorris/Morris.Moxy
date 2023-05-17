using Morris.Moxy.Helpers;
using System.Collections.Immutable;

namespace Morris.Moxy.Templates;

public readonly struct CompiledTemplateAndAttributeSource : IEquatable<CompiledTemplateAndAttributeSource>
{
	public readonly CompiledTemplate CompiledTemplate;
	public readonly string AttributeSource;
	public readonly ImmutableArray<string> AttributeConstructorParameterNames;

	public CompiledTemplateAndAttributeSource(
		CompiledTemplate compiledTemplate,
		string attributeSource,
		ImmutableArray<string> attributeConstructorParameterNames)
	{
		CompiledTemplate = compiledTemplate;
		AttributeSource = attributeSource ?? "";
		AttributeConstructorParameterNames = attributeConstructorParameterNames;
		CachedHashCode = HashCode.Combine(CompiledTemplate, AttributeSource, AttributeConstructorParameterNames);
	}

	private readonly int CachedHashCode;

	public override int GetHashCode() => CachedHashCode;

	public override bool Equals(object obj) =>
		obj is CompiledTemplateAndAttributeSource other && Equals(other);

	public bool Equals(CompiledTemplateAndAttributeSource other) =>
		CompiledTemplate == other.CompiledTemplate
		&& AttributeSource == other.AttributeSource
		&& AttributeConstructorParameterNames.SequenceEqual(other.AttributeConstructorParameterNames);

	public static bool operator ==(CompiledTemplateAndAttributeSource left, CompiledTemplateAndAttributeSource right) => left.Equals(right);

	public static bool operator !=(CompiledTemplateAndAttributeSource left, CompiledTemplateAndAttributeSource right) => !(left == right);
}

