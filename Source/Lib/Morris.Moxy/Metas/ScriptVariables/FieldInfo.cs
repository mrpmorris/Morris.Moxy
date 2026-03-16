namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct FieldInfo
{
	public readonly string Name;
	public readonly string Type;
	public readonly string Accessibility;

	public FieldInfo(string name, string type, string accessibility)
	{
		Name = name;
		Type = type;
		Accessibility = accessibility;
	}
}
