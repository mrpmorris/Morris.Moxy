namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct ClassVariable
{
	public readonly string Name;
	public readonly string Namespace;
	public readonly DeclaringTypeVariable? DeclaringType;
	public readonly IReadOnlyList<FieldInfo> Fields;
	public readonly IReadOnlyList<PropertyInfo> Properties;
	public readonly IReadOnlyList<MethodInfo> Methods;

	public ClassVariable(
		string name,
		string @namespace,
		DeclaringTypeVariable? declaringType = null,
		IReadOnlyList<FieldInfo>? fields = null,
		IReadOnlyList<PropertyInfo>? properties = null,
		IReadOnlyList<MethodInfo>? methods = null)
	{
		Name = name;
		Namespace = @namespace;
		DeclaringType = declaringType;
		Fields = fields ?? new List<FieldInfo>();
		Properties = properties ?? new List<PropertyInfo>();
		Methods = methods ?? new List<MethodInfo>();
	}
}
