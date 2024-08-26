// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Pooling
{
	public static class PoolExtensions
	{
		/// <summary>
		/// Warm up the <see cref="IPool{TPooled}"/> by requesting and releasing <paramref name="count"/> objects.
		/// </summary>
		public static void WarmUp<TPooled>(this IPool<TPooled> pool, int count)
			where TPooled : IPooled
		{
			var collection = pool.RequestRange(count);
			pool.ReleaseRange(collection);
		}

		/// <summary>
		/// Request a range of objects from the <see cref="IPool{TPooled}"/>.
		/// </summary>
		public static IEnumerable<TPooled> RequestRange<TPooled>(this IPool<TPooled> pool, int count)
			where TPooled : IPooled
		{
			var collection = new TPooled[count];
			for (var index = 0; index < count; index++)
			{
				collection[index] = pool.Request();
			}

			return collection;
		}

		/// <summary>
		/// Releases all objects in the list, the list should be cleared afterward.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReleaseRange<TPooled>(this IPool<TPooled> pool, TPooled[] collection)
			where TPooled : IPooled
		{
			foreach (var item in collection)
			{
				pool.Release(item);
			}
		}

		/// <summary>
		/// Releases all objects in the list, the list should be cleared afterward.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReleaseRange<TPooled>(this IPool<TPooled> pool, IEnumerable<TPooled> collection)
			where TPooled : IPooled
		{
			foreach (var item in collection)
			{
				pool.Release(item);
			}
		}
	}
}