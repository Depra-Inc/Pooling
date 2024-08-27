// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Pooling.Object
{
	public static class ObjectPoolExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddFreeRange<T>(this ObjectPool<T> self, IEnumerable<T> collection) where T : IPooled
		{
			foreach (var item in collection)
			{
				self.Release(item);
			}
		}
	}
}