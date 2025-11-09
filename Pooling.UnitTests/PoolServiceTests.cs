// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using NSubstitute;

namespace Depra.Pooling.UnitTests;

public sealed class PoolServiceTests
{
	private readonly PoolService _service = new();

	[Fact]
	public void RequestUntyped_WithValidKey_ReturnsPooledObject()
	{
		// Arrange:
		var poolKey = 123;
		_service.Register(poolKey, MockPool());

		// Act:
		var pooledObject = _service.Request(poolKey);

		// Assert:
		Assert.NotNull(pooledObject);
	}

	[Fact]
	public void RequestUntyped_WithInvalidKey_ReturnNull()
	{
		// Arrange:
		var poolKey = 123;

		// Act:
		var result = _service.Request(poolKey);

		// Assert:
		Assert.IsType<NullPooled>(result);
	}

	[Fact]
	public void RequestTyped_WithValidKey_ReturnsPooledObject()
	{
		// Arrange:
		var poolKey = 123;
		_service.Register(poolKey, MockPool());

		// Act:
		var pooledObject = _service.Request<FakePooledObject>(poolKey);

		// Assert:
		Assert.NotNull(pooledObject);
		Assert.IsType<FakePooledObject>(pooledObject);
	}
	
	[Fact]
	public void RequestTyped_WithInvalidKey_ThrowsException()
	{
		// Arrange:
		var poolKey = 123;

		// Act:
		var exception = Record.Exception(() => _service.Request<FakePooledObject>(poolKey));

		// Assert:
		Assert.IsType<PoolService.PoolNotRegistered>(exception);
	}

	[Fact]
	public void Release_WithValidKeyAndObject_CallsRelease()
	{
		// Arrange:
		var poolKey = 123;
		var pool = MockPool();
		_service.Register(poolKey, pool);
		var pooledObject = new FakePooledObject();

		// Act:
		_service.Release(poolKey, pooledObject);

		// Assert:
		pool.Received(1).ReleasePooled(Arg.Is<FakePooledObject>(x => x == pooledObject));
	}
	
	[Fact]
	public void Release_WithInvalidKey_DoNothing()
	{
		// Arrange:
		var poolKey = 123;
		var pooledObject = new FakePooledObject();

		// Act & Assert:
		_service.Release(poolKey, pooledObject);
	}

	private IPool MockPool()
	{
		var pool = Substitute.For<IPool>();
		pool.RequestPooled().Returns(new FakePooledObject());

		return pool;
	}

	public sealed class FakePooledObject : IPooled
	{
		void IPooled.OnPoolCreate(IPool pool) { }

		void IPooled.OnPoolGet() { }

		void IPooled.OnPoolSleep() { }

		void IPooled.OnPoolReuse() { }
	}
}