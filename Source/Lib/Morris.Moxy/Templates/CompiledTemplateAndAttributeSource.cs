using System.Collections.Immutable;

namespace Morris.Moxy.Templates;

public readonly struct CompiledTemplateAndAttributeSource
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
	}
}
