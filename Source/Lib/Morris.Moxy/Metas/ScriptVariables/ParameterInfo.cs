namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct ParameterInfo
{
	public readonly string Name;
	public readonly string Type;
	public readonly string? DefaultValue;

	public ParameterInfo(string name, string type, string? defaultValue = null)
	{
		Name = name;
		Type = type;
		DefaultValue = defaultValue;
	}
}