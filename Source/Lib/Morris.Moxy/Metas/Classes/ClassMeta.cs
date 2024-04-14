using Morris.Moxy.Extensions;
using Morris.Moxy.Helpers;
using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Classes;

internal class ClassMeta : IEquatable<ClassMeta>
{
	public readonly string ClassName;
	public readonly string Namespace;
	public readonly string? DeclaringTypeName;
	public readonly ImmutableArray<string> GenericParameterNames;
	public readonly ImmutableArray<AttributeInstance> PossibleTemplates;
	public readonly string GenericParametersSignature;

	public string FullName => NamespaceHelper.Combine(Namespace, ClassName);

	private readonly Lazy<int> CachedHashCode;

	public ClassMeta()
	{
		ClassName = "";
		Namespace = "";
		DeclaringTypeName = string.Empty;
		GenericParameterNames = ImmutableArray<string>.Empty;
		PossibleTemplates = ImmutableArray<AttributeInstance>.Empty;
		GenericParametersSignature = "";
		CachedHashCode = new Lazy<int>(() => typeof(ClassMeta).GetHashCode());
	}

	public ClassMeta(
		string className,
		string @namespace,
		string? declaringTypeName,
		ImmutableArray<string> genericParameterNames,
		ImmutableArray<AttributeInstance> possibleTemplates)
	{
		GenericParametersSignature = GetGenericParametersSignature(genericParameterNames);
		ClassName = className + GenericParametersSignature;
		Namespace = @namespace;
		DeclaringTypeName = declaringTypeName;
		GenericParameterNames = genericParameterNames;
		PossibleTemplates = possibleTemplates;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			className,
			@namespace,
			declaringTypeName,
			genericParameterNames.GetContentsHashCode(),
			possibleTemplates.GetContentsHashCode()));
	}

	public static bool operator ==(ClassMeta left, ClassMeta right) => left.Equals(right);
	public static bool operator !=(ClassMeta left, ClassMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is ClassMeta other && Equals(other);

	public bool Equals(ClassMeta other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& ClassName == other.ClassName
			&& Namespace == other.Namespace
			&& DeclaringTypeName == other.DeclaringTypeName
			&& GenericParameterNames.SequenceEqual(other.GenericParameterNames)
			&& PossibleTemplates.SequenceEqual(other.PossibleTemplates)
		);

	public override int GetHashCode() => CachedHashCode.Value;

	public ClassMeta WithNamespace(string @namespace) =>
		new ClassMeta(
			className: ClassName,
			declaringTypeName: DeclaringTypeName,
			@namespace: @namespace,
			genericParameterNames: GenericParameterNames,
			possibleTemplates: PossibleTemplates);

	private static string GetGenericParametersSignature(ImmutableArray<string> genericParameterNames) =>
		genericParameterNames.IsDefaultOrEmpty
		? ""
		: "<" + string.Join(", ", genericParameterNames) + ">";
}

