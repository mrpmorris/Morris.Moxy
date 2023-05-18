using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Classes;

internal readonly struct ClassMeta : IEquatable<ClassMeta>
{
	public static readonly ClassMeta Empty = new ClassMeta();

	public string ClassName { get; } = "";
	public string Namespace { get; } = "";
	public ImmutableArray<string> GenericParameterNames { get; } = ImmutableArray<string>.Empty;
	public ImmutableArray<string> PossibleTemplates { get; } = ImmutableArray<string>.Empty;
	public string GenericParametersSignature { get; } = "";

	private readonly Lazy<int> CachedHashCode;

	public ClassMeta(
		string className,
		string @namespace,
		ImmutableArray<string> genericParameterNames,
		ImmutableArray<string> possibleTemplates)
	{
		ClassName = className;
		Namespace = @namespace;
		GenericParameterNames = genericParameterNames;
		PossibleTemplates = possibleTemplates;
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(className, @namespace, genericParameterNames, possibleTemplates));
	}

	public static bool operator ==(ClassMeta left, ClassMeta right) => left.Equals(right);
	public static bool operator !=(ClassMeta left, ClassMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is ClassMeta other && Equals(other);

	public bool Equals(ClassMeta other) =>
		ReferenceEquals(this, other)
		||
		(
			ClassName == other.ClassName
			&& Namespace == other.Namespace
			&& GenericParameterNames.SequenceEqual(other.GenericParameterNames)
			&& PossibleTemplates.SequenceEqual(other.PossibleTemplates)
		);

	public override int GetHashCode() => CachedHashCode.Value;

	public ClassMeta WithNamespace(string @namespace) =>
		new ClassMeta(
			className: ClassName,
			@namespace: @namespace,
			genericParameterNames: GenericParameterNames,
			possibleTemplates: PossibleTemplates);
}

