using System.Collections.Immutable;

namespace Morris.Moxy.Classes;

public readonly struct ClassInfo : IEquatable<ClassInfo>
{
	public static readonly ClassInfo Empty = new();
	public readonly string ClassTypeId;
	public readonly string ClassName;
	public readonly string Namespace;
	public readonly ImmutableArray<string> GenericParameterNames;
	public readonly ImmutableArray<AttributeNameAndSyntaxTree> PossibleTemplates;
	public readonly string GenericParametersSignature;

	public ClassInfo()
	{
		ClassTypeId = "";
		ClassName = "";
		Namespace = "";
		GenericParameterNames = ImmutableArray.Create<string>();
		GenericParametersSignature = "";
		PossibleTemplates = ImmutableArray.Create<AttributeNameAndSyntaxTree>();
	}

	public ClassInfo(
		string className,
		string @namespace,
		ImmutableArray<string> genericParameterNames,
		ImmutableArray<AttributeNameAndSyntaxTree> possibleTemplates)
	{
		if (className is null)
			throw new ArgumentNullException(nameof(className));

		Namespace = @namespace ?? throw new ArgumentNullException(nameof(@namespace));
		GenericParameterNames = genericParameterNames;
		PossibleTemplates = possibleTemplates;
		GenericParametersSignature =
			genericParameterNames.IsDefault || genericParameterNames.Length == 0
			? ""
			: "<" + string.Join(", ", GenericParameterNames) + ">";
		ClassName = $"{className}{GenericParametersSignature}";
		ClassTypeId =
			genericParameterNames.IsDefault || genericParameterNames.Length == 0
			? className
			: $"{className}`{genericParameterNames.Length}";
	}

	public bool Equals(ClassInfo other) =>
		other.ClassName == ClassName
		&& other.Namespace == Namespace
		&& Enumerable.SequenceEqual(other.PossibleTemplates, PossibleTemplates);
}
