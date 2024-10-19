// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using FluentAssertions;

namespace Depra.Pooling.Object.UnitTests;

public sealed class ObjectPoolTests
{
	[Theory]
	[InlineData(30)]
	public void Warm_Up(int amount)
	{
		// Arrange:
		var configuration = new PoolConfiguration(amount);
		var pool = new ObjectPool<TestPooled>(configuration, new ReflectionBasedObjectFactory<TestPooled>());

		// Act:
		pool.WarmUp(amount);

		// Assert:
		pool.CountAll.Should().Be(amount);
		pool.CountPassive.Should().Be(amount);
	}

	[Fact]
	public void Request()
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(new PoolConfiguration(), new ReflectionBasedObjectFactory<TestPooled>());

		// Act:
		var obj = pool.Request();

		// Assert:
		obj.Should().NotBeNull();
		obj.Created.Should().BeTrue();
		pool.CountActive.Should().Be(1);
	}

	[Fact]
	public void Request_WhenPoolIsOverflowed_ShouldThrow()
	{
		// Arrange:
		var configuration = new PoolConfiguration(0, 0, overflowStrategy: OverflowStrategy.THROW_EXCEPTION);
		var pool = new ObjectPool<TestPooled>(configuration, new ReflectionBasedObjectFactory<TestPooled>());

		// Act:
		Action act = () => pool.Request();

		// Assert:
		act.Should().Throw<PoolOverflowed>();
	}

	[Fact]
	public void Request_WhenPoolIsOverflowed_ShouldReuse()
	{
		// Arrange:
		var configuration = new PoolConfiguration(1, 1, overflowStrategy: OverflowStrategy.REUSE);
		var pool = new ObjectPool<TestPooled>(configuration, new ReflectionBasedObjectFactory<TestPooled>());
		pool.Request();

		// Act:
		var obj = pool.Request();

		// Assert:
		obj.Should().NotBeNull();
		obj.Created.Should().BeTrue();
		pool.CountActive.Should().Be(1);
	}

	[Theory]
	[InlineData(2)]
	public void Release(int amount)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(new PoolConfiguration(), new ReflectionBasedObjectFactory<TestPooled>());
		var collection = new List<TestPooled>(amount);
		for (var i = 0; i < amount; i++)
		{
			collection.Add(pool.Request());
		}

		var lastObject = collection.Last();

		// Act:
		pool.AddFreeRange(collection);
		pool.Request();
		pool.Request();
		pool.Release(lastObject);

		// Assert:
		lastObject.Free.Should().BeTrue();
		pool.CountPassive.Should().Be(1);
		pool.CountActive.Should().Be(amount - 1);
	}

	[Fact]
	public void Dispose()
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(new PoolConfiguration(), new ReflectionBasedObjectFactory<TestPooled>());

		// Act:
		pool.Request();

		// Assert:
		pool.CountAll.Should().NotBe(0);

		// Act:
		pool.Dispose();

		// Assert:
		pool.CountAll.Should().Be(0);
		pool.CountActive.Should().Be(0);
		pool.CountPassive.Should().Be(0);
	}

	[Theory]
	[InlineData(30)]
	public void AddFreeRange(int amount)
	{
		// Arrange:
		var configuration = new PoolConfiguration(amount);
		var pool = new ObjectPool<TestPooled>(configuration, new ReflectionBasedObjectFactory<TestPooled>());
		var collection = new TestPooled[amount];
		for (var index = 0; index < collection.Length; index++)
		{
			collection[index] = pool.Request();
		}

		// Act:
		pool.AddFreeRange(collection);

		// Assert:
		pool.CountPassive.Should().Be(amount);
		Array.ForEach(collection, pooled => pooled.Free.Should().BeTrue());
	}

	[Theory]
	[InlineData(30)]
	public void RequestRange(int amount)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(new PoolConfiguration(), new ReflectionBasedObjectFactory<TestPooled>());

		// Act:
		pool.RequestRange(amount);

		// Assert:
		pool.CountAll.Should().Be(amount);
		pool.CountActive.Should().Be(amount);
	}

	[Theory]
	[InlineData(30)]
	public void ReleaseRange(int amount)
	{
		// Arrange:
		var configuration = new PoolConfiguration(amount);
		var pool = new ObjectPool<TestPooled>(configuration, new ReflectionBasedObjectFactory<TestPooled>());
		var collection = new TestPooled[amount];
		for (var index = 0; index < collection.Length; index++)
		{
			collection[index] = pool.Request();
		}

		// Act:
		pool.ReleaseRange(collection);

		// Assert:
		Array.ForEach(collection, pooled => pooled.Free.Should().BeTrue());
		pool.CountPassive.Should().Be(collection.Length);
	}
}