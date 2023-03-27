namespace MixInAMethod;

[SayHello]
internal partial class Person
{
	public Guid Id { get; set; } = Guid.NewGuid();
}
