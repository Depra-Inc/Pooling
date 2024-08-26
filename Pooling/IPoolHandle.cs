// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Pooling
{
	public interface IPoolHandle<T> where T : IPooled
	{
		void ReturnInstanceToPool(PooledInstance<T> instance, bool reRegisterForFinalization);
	}
}