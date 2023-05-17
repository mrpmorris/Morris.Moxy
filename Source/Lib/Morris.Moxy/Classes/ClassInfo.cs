using Morris.Moxy.DataStructures;
using Morris.Moxy.Extensions;
using Morris.Moxy.Helpers;
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
	private readonly Lazy<int> CachedHashCode;

	public ClassInfo()
	{
		ClassTypeId = "";
		ClassName = "";
		Namespace = "";
		GenericParameterNames = ImmutableArray.Create<string>();
		GenericParametersSignature = "";
		PossibleTemplates = ImmutableArray.Create<AttributeNameAndSyntaxTree>();
		CachedHashCode = new Lazy<int>(() => typeof(ClassInfo).GetHashCode());
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

		CachedHashCode = new Lazy<int>(() => 
			HashCode.Combine(
				className, 
				@namespace, 
				genericParameterNames.GetContentHashCode(),
				possibleTemplates.GetContentHashCode()));
	}

	public bool Equals(ClassInfo other) =>
		other.ClassName == ClassName
		&& other.Namespace == Namespace
		&& other.ClassTypeId == ClassTypeId
		&& other.PossibleTemplates.SequenceEqual(PossibleTemplates)
		&& other.GenericParameterNames.SequenceEqual(GenericParameterNames)
		&& other.GenericParametersSignature == GenericParametersSignature;

	public override bool Equals(object obj) =>
		obj is ClassInfo other && Equals(other);

	public override int GetHashCode() => CachedHashCode.Value;

	public static bool operator ==(ClassInfo left, ClassInfo right) => left.Equals(right);

	public static bool operator !=(ClassInfo left, ClassInfo right) => !(left == right);
}
