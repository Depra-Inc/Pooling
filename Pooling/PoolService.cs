// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;

namespace Depra.Pooling
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class PoolService : IDisposable
	{
		private readonly Dictionary<int, IPool> _pools = new(32);

		public void Dispose()
		{
			foreach (var pool in _pools.Values)
			{
				if (pool is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
		}

		public void Register(int key, IPool pool) => _pools.TryAdd(key, pool);

		public TPooled Request<TPooled>(int key) where TPooled : IPooled => (TPooled)Request(key);

		public IPooled Request(int key)
		{
			Guard.Against(!_pools.ContainsKey(key), () => new PoolNotRegistered(key));
			return _pools[key].RequestPooled();
		}

		public void Release(int key, IPooled instance)
		{
			Guard.Against(!_pools.ContainsKey(key), () => new PoolNotRegistered(key));
			_pools[key].ReleasePooled(instance);
		}

		internal sealed class PoolNotRegistered : Exception
		{
			public PoolNotRegistered(int key) : base($"Pool with key {key} is not registered") { }
		}
	}
}