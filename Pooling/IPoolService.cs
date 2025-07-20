// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System.Collections.Generic;

namespace Depra.Pooling
{
	public interface IPoolService
	{
		void Clear();

		IEnumerable<IPool> Enumerate();

		IPool<TPooled> Pool<TPooled>(int key) where TPooled : IPooled;
	}
}