using BenchmarkDotNet.Attributes;
using Depra.Borrow;

namespace Depra.Pooling.Object.Benchmarks;

public class ObjectPoolBenchmarks
{
	private const int DEFAULT_CAPACITY = 100;
	private ObjectPool<PoolableObject> _hotPool;
	private ObjectPool<PoolableObject> _coldPool;

	[GlobalSetup]
	public void GlobalSetup()
	{
		_coldPool = new ObjectPool<PoolableObject>(BorrowStrategy.LIFO, new PooledClassFactory<PoolableObject>());

		_hotPool = new ObjectPool<PoolableObject>(BorrowStrategy.LIFO, new PooledClassFactory<PoolableObject>());
		_hotPool.WarmUp(DEFAULT_CAPACITY);
	}

	[GlobalCleanup]
	public void GlobalCleanup() => _hotPool.Dispose();

	[Benchmark]
	public void Request() => _hotPool.Request();

	[Benchmark]
	public void Cycle() => _hotPool.Release(_hotPool.Request());

	[Benchmark]
	public void WarmUp() => _coldPool.WarmUp(DEFAULT_CAPACITY);
}