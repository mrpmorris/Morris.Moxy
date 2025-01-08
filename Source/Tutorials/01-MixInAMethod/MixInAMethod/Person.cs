namespace MixInAMethod;

[ShowMoxyVariables]
[SayHello]
[MemberDump]
internal partial class Person
{
	public string _field1 = "field val1";
	public int _field2 = 2;

	public string Prop1 { get; set; } = "prop val1";
	public int Prop2 { get; set; } = 22;


	public void Ha()
	{
		Console.WriteLine("Ha");
	}

	public bool Ha2(object param1, Person? param2, string param3="defaultParam1_Value", int param4=4)
	{
		return true;
	}
}
