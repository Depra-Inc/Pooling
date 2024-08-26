// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Borrow;
using FluentAssertions;

namespace Depra.Pooling.Object.UnitTests;

public sealed class ObjectPoolTests
{
	[Theory]
	[InlineData(30)]
	public void Warm_Up(int amount)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>(), amount);

		// Act:
		pool.WarmUp(amount);

		// Assert:
		pool.Count.Should().Be(amount);
	}

	[Fact]
	public void Request()
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>());

		// Act:
		var obj = pool.Request();

		// Assert:
		obj.Should().NotBeNull();
		obj.Created.Should().BeTrue();
	}

	[Theory]
	[InlineData(2)]
	public void Release(int amount)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>());

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
	}

	[Fact]
	public void Dispose()
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>());

		// Act:
		pool.Request();

		// Assert:
		pool.Count.Should().NotBe(0);

		// Act:
		pool.Dispose();

		// Assert:
		pool.Count.Should().Be(0);
	}

	[Theory]
	[InlineData(30)]
	public void AddFreeRange(int capacity)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>(), capacity);
		var collection = new[] { pool.Request() };

		// Act:
		pool.AddFreeRange(collection);

		// Assert:
		//pool.CountInactive.Should().Be(1);
		Array.ForEach(collection, pooled => pooled.Free.Should().BeTrue());
	}

	[Theory]
	[InlineData(30)]
	public void RequestRange(int amount)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>(), amount);

		// Act:
		pool.RequestRange(amount);

		// Assert:
		pool.Count.Should().Be(amount);
	}

	[Theory]
	[InlineData(30)]
	public void ReleaseRange(int capacity)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new PooledClassFactory<TestPooled>(), capacity);
		var collection = new[] { pool.Request() };

		// Act:
		pool.ReleaseRange(collection);

		// Assert:
		Array.ForEach(collection, pooled => pooled.Free.Should().BeTrue());
	}
}