namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct PropertyInfo
{
	public readonly string Name;
	public readonly string Type;
	public readonly string Accessibility;
	public readonly bool HasGetter;
	public readonly bool HasSetter;

	public PropertyInfo(string name, string type, string accessibility, bool hasGetter, bool hasSetter)
	{
		Name = name;
		Type = type;
		Accessibility = accessibility;
		HasGetter = hasGetter;
		HasSetter = hasSetter;
	}
}
