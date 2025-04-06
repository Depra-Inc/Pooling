// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

namespace Depra.Pooling
{
	/// <summary>
	/// One-way interface for returning instances to the pool.
	/// </summary>
	/// <typeparam name="TPooled">Type of the pooled object.</typeparam>
	public interface IPoolHandle<TPooled> where TPooled : IPooled
	{
		void ReturnInstanceToPool(PooledInstance<TPooled> instance);
	}
}