namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct ClassVariable
{
	public readonly string Name;
	public readonly string Namespace;

	public ClassVariable(string name, string @namespace)
	{
		Name = name;
		Namespace = @namespace;
	}
}