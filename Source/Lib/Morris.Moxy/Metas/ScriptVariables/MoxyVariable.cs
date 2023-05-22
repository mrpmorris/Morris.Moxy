namespace Morris.Moxy.Metas.ScriptVariables;

internal readonly struct MoxyVariable
{
	public readonly ClassVariable Class;

	public MoxyVariable(ClassVariable @class)
	{
		Class = @class;
	}
}
