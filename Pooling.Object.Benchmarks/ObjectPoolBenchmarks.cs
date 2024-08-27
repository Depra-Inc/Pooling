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

	[GlobalSetup]
	public void Setup()
	{
		var fakeObject = new FakePooledObject();
		_coldPool = new ObjectPool<FakePooledObject>(BorrowStrategy.LIFO,
			new LambdaBasedObjectFactory<FakePooledObject>(() => fakeObject));
		_hotPool = new ObjectPool<FakePooledObject>(BorrowStrategy.LIFO,
			new LambdaBasedObjectFactory<FakePooledObject>(() => fakeObject));
		_hotPool.WarmUp(WARMUP_AMOUNT);
	}

	[GlobalCleanup]
	public void Cleanup() => _hotPool.Dispose();

	[Benchmark(Baseline = true)]
	public FakePooledObject Request_Hot() => _hotPool.Request();

	[Benchmark]
	public FakePooledObject Request_Cold() => _coldPool.Request();

	[Benchmark]
	public void Cycle() => _hotPool.Release(_hotPool.Request());

	[Benchmark]
	public void WarmUp() => _coldPool.WarmUp(WARMUP_AMOUNT);
}