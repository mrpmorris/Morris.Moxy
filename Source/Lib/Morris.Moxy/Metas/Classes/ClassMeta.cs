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
	public readonly ImmutableArray<FieldMeta> Fields;
	public readonly ImmutableArray<PropertyMeta> Properties;
	public readonly ImmutableArray<MethodMeta> Methods;
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
		Fields = ImmutableArray<FieldMeta>.Empty;
		Properties = ImmutableArray<PropertyMeta>.Empty;
		Methods = ImmutableArray<MethodMeta>.Empty;
		GenericParametersSignature = "";
		CachedHashCode = new Lazy<int>(() => typeof(ClassMeta).GetHashCode());
	}

	public ClassMeta(
		string className,
		string @namespace,
		string? declaringTypeName,
		ImmutableArray<string> genericParameterNames,
		ImmutableArray<AttributeInstance> possibleTemplates,
		ImmutableArray<FieldMeta> fields,
		ImmutableArray<PropertyMeta> properties,
		ImmutableArray<MethodMeta> methods)
	{
		GenericParametersSignature = GetGenericParametersSignature(genericParameterNames);
		ClassName = className + GenericParametersSignature;
		Namespace = @namespace;
		DeclaringTypeName = declaringTypeName;
		GenericParameterNames = genericParameterNames;
		PossibleTemplates = possibleTemplates;
		Fields = fields;
		Properties = properties;
		Methods = methods;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			className,
			@namespace,
			declaringTypeName,
			genericParameterNames.GetContentsHashCode(),
			possibleTemplates.GetContentsHashCode(),
			fields.GetContentsHashCode(),
			properties.GetContentsHashCode(),
			methods.GetContentsHashCode()));
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
			&& Fields.SequenceEqual(other.Fields)
			&& Properties.SequenceEqual(other.Properties)
			&& Methods.SequenceEqual(other.Methods)
		);

	public override int GetHashCode() => CachedHashCode.Value;

	public ClassMeta WithNamespace(string @namespace) =>
		new ClassMeta(
			className: ClassName,
			declaringTypeName: DeclaringTypeName,
			@namespace: @namespace,
			genericParameterNames: GenericParameterNames,
			possibleTemplates: PossibleTemplates,
			fields: Fields,
			properties: Properties,
			methods: Methods);

	private static string GetGenericParametersSignature(ImmutableArray<string> genericParameterNames) =>
		genericParameterNames.IsDefaultOrEmpty
		? ""
		: "<" + string.Join(", ", genericParameterNames) + ">";
}