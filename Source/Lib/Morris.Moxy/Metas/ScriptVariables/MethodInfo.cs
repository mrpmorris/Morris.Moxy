namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct MethodInfo
{
	public readonly string Name;
	public readonly string ReturnType;
	public readonly string Accessibility;
	public readonly IReadOnlyList<ParameterInfo> Parameters;

	public MethodInfo(string name, string returnType, string accessibility, IReadOnlyList<ParameterInfo>? parameters = null)
	{
		Name = name;
		ReturnType = returnType;
		Accessibility = accessibility;
		Parameters = parameters ?? Array.Empty<ParameterInfo>();
	}
}
