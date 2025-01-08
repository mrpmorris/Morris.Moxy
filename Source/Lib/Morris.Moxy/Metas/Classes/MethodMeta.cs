using Morris.Moxy.Extensions;
using System.Collections.Immutable;

namespace Morris.Moxy.Metas.Classes;

internal class MethodMeta : IEquatable<MethodMeta>
{
	public readonly string Name;
	public readonly string ReturnTypeName;
	public readonly string Accessibility;
	public readonly ImmutableArray<ParameterMeta> Parameters;

	private readonly Lazy<int> CachedHashCode;

	public MethodMeta()
	{
		Name = "";
		ReturnTypeName = "";
		Accessibility = "";
		Parameters = ImmutableArray<ParameterMeta>.Empty;
		CachedHashCode = new Lazy<int>(() => typeof(MethodMeta).GetHashCode());
	}

	public MethodMeta(
		string name,
		string returnTypeName,
		string accessibility,
		ImmutableArray<ParameterMeta> parameters)
	{
		Name = name;
		ReturnTypeName = returnTypeName;
		Accessibility = accessibility;
		Parameters = parameters;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			name,
			returnTypeName,
			accessibility,
			parameters.GetContentsHashCode()));
	}

	public static bool operator ==(MethodMeta left, MethodMeta right) => left.Equals(right);
	public static bool operator !=(MethodMeta left, MethodMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is MethodMeta other && Equals(other);

	public bool Equals(MethodMeta other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& ReturnTypeName == other.ReturnTypeName
			&& Accessibility == other.Accessibility
			&& Parameters.SequenceEqual(other.Parameters)
		);

	public override int GetHashCode() => CachedHashCode.Value;
}

internal class ParameterMeta : IEquatable<ParameterMeta>
{
	public readonly string Name;
	public readonly string TypeName;
	public readonly string? DefaultValue;

	private readonly Lazy<int> CachedHashCode;

	public ParameterMeta()
	{
		Name = "";
		TypeName = "";
		DefaultValue = null;
		CachedHashCode = new Lazy<int>(() => typeof(ParameterMeta).GetHashCode());
	}

	public ParameterMeta(
		string name,
		string typeName,
		string? defaultValue = null)
	{
		Name = name;
		TypeName = typeName;
		DefaultValue = defaultValue;

		CachedHashCode = new Lazy<int>(() => HashCode.Combine(
			name,
			typeName,
			defaultValue));
	}

	public static bool operator ==(ParameterMeta left, ParameterMeta right) => left.Equals(right);
	public static bool operator !=(ParameterMeta left, ParameterMeta right) => !(left == right);
	public override bool Equals(object obj) => obj is ParameterMeta other && Equals(other);

	public bool Equals(ParameterMeta other) =>
		ReferenceEquals(this, other)
		||
		(
			CachedHashCode.IsValueCreated == other.CachedHashCode.IsValueCreated == true
				? CachedHashCode.Value == other.CachedHashCode.Value
				: true
			&& Name == other.Name
			&& TypeName == other.TypeName
			&& DefaultValue == other.DefaultValue
		);

	public override int GetHashCode() => CachedHashCode.Value;
}