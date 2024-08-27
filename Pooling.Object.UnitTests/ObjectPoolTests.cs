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
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>(),
			amount);

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
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>());

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
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>());

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
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>());

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
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>(), amount);
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
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>(),
			amount);

		// Act:
		pool.RequestRange(amount);

		// Assert:
		pool.CountAll.Should().Be(amount);
		pool.CountActive.Should().Be(amount);
	}

	[Theory]
	[InlineData(30)]
	public void ReleaseRange(int capacity)
	{
		// Arrange:
		var pool = new ObjectPool<TestPooled>(BorrowStrategy.LIFO, new ReflectionBasedObjectFactory<TestPooled>(),
			capacity);
		var collection = new[] { pool.Request() };

		// Act:
		pool.ReleaseRange(collection);

		// Assert:
		Array.ForEach(collection, pooled => pooled.Free.Should().BeTrue());
		pool.CountPassive.Should().Be(collection.Length);
	}
}