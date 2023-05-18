using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Classes;

internal readonly struct ClassMeta : IEquatable<ClassMeta>
{
	public static readonly ClassMeta Empty = new ClassMeta();

	public readonly string ClassName = "";
	public readonly string Namespace = "";
	public readonly ImmutableArray<string> GenericParameterNames = ImmutableArray<string>.Empty;
	public readonly ImmutableArray<string> PossibleTemplates = ImmutableArray<string>.Empty;
	public readonly string GenericParametersSignature { get; } = "";

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
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& ClassName == other.ClassName
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

