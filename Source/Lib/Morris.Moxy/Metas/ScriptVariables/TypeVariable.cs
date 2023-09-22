namespace Morris.Moxy.Metas.ScriptVariables;

public readonly struct TypeVariable
{
	public readonly string Name;
	public readonly string FullName;

	public TypeVariable(string name, string fullName)
	{
		Name = name ?? throw new ArgumentNullException(nameof(name));
		FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
	}

	public override string ToString() => Name;
}
