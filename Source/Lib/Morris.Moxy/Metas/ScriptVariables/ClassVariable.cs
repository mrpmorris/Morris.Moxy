namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct ClassVariable
{
	public readonly string Name;
	public readonly string Namespace;
	public readonly DeclaringTypeVariable? DeclaringType;

	public ClassVariable(string name, string @namespace, DeclaringTypeVariable? declaringType = null)
	{
		Name = name;
		Namespace = @namespace;
		DeclaringType = declaringType;
	}
}