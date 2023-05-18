using Morris.Moxy.Extensions;
using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Templates;

internal readonly struct ParsedTemplate : IEquatable<ParsedTemplate>
{
	public readonly string Name;
	public readonly string FilePath;
	public readonly string TemplateSource;
	public readonly int TemplateBodyLineNumber;
	public readonly ImmutableArray<string> AttributeUsingClauses;
	public readonly ImmutableArray<TemplateInput> RequiredInputs;
	public readonly ImmutableArray<TemplateInput> OptionalInputs;

	private readonly Lazy<int> CachedHashCode;

	public ParsedTemplate()
	{
		Name = "";
		FilePath = "";
		TemplateSource = "";
		TemplateBodyLineNumber = 0;
		AttributeUsingClauses = ImmutableArray<string>.Empty;
		RequiredInputs = ImmutableArray<TemplateInput>.Empty;
		OptionalInputs = ImmutableArray<TemplateInput>.Empty;

		CachedHashCode = new Lazy<int>(() => typeof(ParsedTemplate).GetHashCode());
	}

	public ParsedTemplate(
		string name,
		string filePath,
		string templateSource,
		int templateBodyLineNumber,
		ImmutableArray<string> attributeUsingClauses,
		ImmutableArray<TemplateInput> requiredInputs,
		ImmutableArray<TemplateInput> optionalInputs)
	{
		Name = name;
		FilePath = filePath;
		TemplateSource = templateSource;
		TemplateBodyLineNumber = templateBodyLineNumber;
		AttributeUsingClauses = attributeUsingClauses;
		RequiredInputs = requiredInputs;
		OptionalInputs = optionalInputs;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			name,
			filePath,
			templateSource,
			templateBodyLineNumber,
			attributeUsingClauses.GetContentsHashCode(),
			requiredInputs.GetContentsHashCode(),
			optionalInputs.GetContentsHashCode()));
	}

	public static bool operator ==(ParsedTemplate left, ParsedTemplate right) => left.Equals(right);
	public static bool operator !=(ParsedTemplate left, ParsedTemplate right) => !(left == right);
	public override bool Equals(object obj) => obj is ParsedTemplate other && Equals(other);

	public bool Equals(ParsedTemplate other) =>
		CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
			? CachedHashCode.Value == other.CachedHashCode.Value
			: true
		&& TemplateBodyLineNumber == other.TemplateBodyLineNumber
		&& Name == other.Name
		&& FilePath == other.FilePath
		&& TemplateSource == other.TemplateSource
		&& AttributeUsingClauses.SequenceEqual(other.AttributeUsingClauses)
		&& RequiredInputs.SequenceEqual(other.RequiredInputs)
		&& OptionalInputs.SequenceEqual(other.OptionalInputs);

	public override int GetHashCode() => CachedHashCode.Value;
}
