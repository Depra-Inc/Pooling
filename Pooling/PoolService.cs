// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Pooling
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class PoolService
	{
		private readonly Dictionary<object, IPool> _pools = new(32);

		public void Register(object key, IPool pool) => _pools.TryAdd(key, pool);

		public TPooled Request<TPooled>(object key) where TPooled : IPooled => (TPooled)Request(key);

		public IPooled Request(object key)
		{
			Guard.Against(_pools.ContainsKey(key) == false, () => new PoolNotRegistered(key));
			return _pools[key].RequestPooled();
		}

		public void Release(object key, IPooled instance)
		{
			Guard.Against(_pools.ContainsKey(key) == false, () => new PoolNotRegistered(key));
			_pools[key].ReleasePooled(instance);
		}

		private sealed class PoolNotRegistered : Exception
		{
			public PoolNotRegistered(object key) : base($"Pool with key {key} is not registered") { }
		}
	}
}