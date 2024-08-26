// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using BenchmarkDotNet.Attributes;
using Depra.Borrow;

namespace Depra.Pooling.Object.Benchmarks;

public class ObjectPoolBenchmarks
{
	private const int WARMUP_AMOUNT = 1000;
	private ObjectPool<FakePooledObject> _hotPool;
	private ObjectPool<FakePooledObject> _coldPool;

	[IterationSetup]
	public void Setup()
	{
		var fakeObject = new FakePooledObject();
		_coldPool = new ObjectPool<FakePooledObject>(BorrowStrategy.LIFO,
			new LambdaBasedPooledObjectFactory<FakePooledObject>(() => fakeObject));
		_hotPool = new ObjectPool<FakePooledObject>(BorrowStrategy.LIFO,
			new LambdaBasedPooledObjectFactory<FakePooledObject>(() => fakeObject));
		_hotPool.WarmUp(WARMUP_AMOUNT);
	}

	[IterationCleanup]
	public void Cleanup() => _hotPool.Dispose();

	[Benchmark]
	public void Request() => _hotPool.Request();

	[Benchmark]
	public void Cycle() => _hotPool.Release(_hotPool.Request());

	[Benchmark]
	public void WarmUp() => _coldPool.WarmUp(WARMUP_AMOUNT);
}