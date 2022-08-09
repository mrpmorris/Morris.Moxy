namespace Morris.Moxy;

internal class DisposableAction : IDisposable
{
	private readonly Action Action;

	public DisposableAction(Action action)
	{
		Action = action ?? throw new ArgumentNullException(nameof(action));
	}

	void IDisposable.Dispose()
	{
		Action();
	}
}
