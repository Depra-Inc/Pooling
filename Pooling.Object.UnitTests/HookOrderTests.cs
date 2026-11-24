// SPDX-License-Identifier: Apache-2.0
// © 2024-2026 Depra <n.melnikov@depra.org>

using FluentAssertions;

namespace Depra.Pooling.Object.UnitTests;

public sealed class HookOrderTests
{
	private readonly List<string> _log = new();

	[Fact]
	public void Request_Created_EnablesBeforeGet()
	{
		// Arrange:
		var pool = CreatePool(new PoolConfiguration());

		// Act:
		pool.Request();

		// Assert:
		_log.Should().ContainInOrder("create", "enable", "get");
	}

	[Fact]
	public void Request_Passive_EnablesBeforeReuseAndGet()
	{
		// Arrange:
		var pool = CreatePool(new PoolConfiguration());
		pool.Release(pool.Request());
		_log.Clear();

		// Act:
		pool.Request();

		// Assert:
		_log.Should().Equal("enable", "reuse", "get");
	}

	[Fact]
	public void Request_Overflowed_EnablesBeforeReuseAndGet()
	{
		// Arrange:
		var pool = CreatePool(new PoolConfiguration(1, 1, overflowStrategy: OverflowStrategy.REUSE));
		pool.Request();
		_log.Clear();

		// Act:
		pool.Request();

		// Assert:
		_log.Should().Equal("enable", "reuse", "get");
	}

	private ObjectPool<LoggedPooled> CreatePool(PoolConfiguration configuration) => new(
		new LambdaBasedObjectFactory<LoggedPooled>(
			create: () => new LoggedPooled(_log),
			onEnable: (_, _) => _log.Add("enable")),
		configuration);

	private sealed class LoggedPooled : IPooled
	{
		private readonly List<string> _log;

		public LoggedPooled(List<string> log) => _log = log;

		void IPooled.OnPoolCreate(IPool pool) => _log.Add("create");
		void IPooled.OnPoolGet() => _log.Add("get");
		void IPooled.OnPoolSleep() { }
		void IPooled.OnPoolReuse() => _log.Add("reuse");
	}
}
