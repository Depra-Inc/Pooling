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
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WarmUp<TPooled>(this IPool<TPooled> pool, int count)
			where TPooled : IPooled =>
			pool.ReleaseRange(pool.RequestRange(count));

		/// <summary>
		/// Warm up the <see cref="IPool"/> by requesting and releasing <paramref name="count"/> objects.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WarmUp(this IPool pool, int count) => pool.ReleaseRange(pool.RequestRange(count));

		/// <summary>
		/// Request a range of objects from the <see cref="IPool{TPooled}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
		/// Request a range of objects from the <see cref="IPool"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<IPooled> RequestRange(this IPool pool, int count)
		{
			var collection = new IPooled[count];
			for (var index = 0; index < count; index++)
			{
				collection[index] = pool.RequestPooled();
			}

			return collection;
		}

		/// <summary>
		/// Releases all objects in the list.
		/// </summary>
		/// <remarks>The list should be cleared afterward.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReleaseRange<TPooled>(this IPool<TPooled> pool, IEnumerable<TPooled> collection)
			where TPooled : IPooled
		{
			foreach (var item in collection)
			{
				pool.Release(item);
			}
		}

		/// <summary>
		/// Releases all objects in the list.
		/// </summary>
		/// <remarks>The list should be cleared afterward.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReleaseRange(this IPool pool, IEnumerable<IPooled> collection)
		{
			foreach (var item in collection)
			{
				pool.ReleasePooled(item);
			}
		}
	}
}