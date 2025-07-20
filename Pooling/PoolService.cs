// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;

namespace Depra.Pooling
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
	public sealed class PoolService : IPoolService, IDisposable
	{
		private const int DEFAULT_CAPACITY = 32;
		private readonly Dictionary<int, IPool> _pools = new(DEFAULT_CAPACITY);

		public void Clear() => _pools.Clear();

		public void Register(int key, IPool pool) => _pools.TryAdd(key, pool);

		public TPooled Request<TPooled>(int key) where TPooled : IPooled => (TPooled)Request(key);

		public IPooled Request(int key) => _pools.TryGetValue(key, out var pool)
			? pool.RequestPooled()
			: new NullPooled();

		public void Release(int key, IPooled instance)
		{
			if (_pools.TryGetValue(key, out var pool))
			{
				pool.ReleasePooled(instance);
			}
		}

		IPool<TPooled> IPoolService.Pool<TPooled>(int key) => _pools.TryGetValue(key, out var pool)
			? (IPool<TPooled>)pool
			: throw new PoolNotRegistered(key);

		IEnumerable<IPool> IPoolService.Enumerate() => _pools.Values;

		void IDisposable.Dispose()
		{
			foreach (var pool in _pools.Values)
			{
				if (pool is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}

			Clear();
		}

		internal sealed class PoolNotRegistered : Exception
		{
			public PoolNotRegistered(int key) : base($"Pool with key {key} is not registered") { }
		}
	}
}