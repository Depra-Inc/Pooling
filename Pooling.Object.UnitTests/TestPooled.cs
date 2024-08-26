namespace Depra.Pooling.Object.UnitTests;

internal class TestPooled : IPooled
{
	public bool Created { get; private set; }

	public bool InUse { get; private set; }

	public bool Free { get; private set; }

	public void OnPoolReuse() { }

	public void OnPoolCreate(IPool pool)
	{
		Created = true;
	}

	public void OnPoolGet()
	{
		InUse = true;
	}

	public void OnPoolSleep()
	{
		Free = true;
	}
}