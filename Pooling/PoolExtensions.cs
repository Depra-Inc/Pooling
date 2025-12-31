// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Depra.Pooling
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
	public static class PoolExtensions
	{
		/// <summary>
		/// Warm up the <see cref="IPool{TPooled}"/> by requesting and releasing <paramref name="count"/> objects.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WarmUp<TPooled>(this IPool<TPooled> self, int count)
			where TPooled : IPooled =>
			self.ReleaseRange(self.RequestRange(count));

		/// <summary>
		/// Warm up the <see cref="IPool"/> by requesting and releasing <paramref name="count"/> objects.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WarmUp(this IPool self, int count) => self.ReleaseRange(self.RequestRange(count));

		public static async Task WarmUpAsync<TPooled>(this IAsyncPool<TPooled> self, int count,
			CancellationToken cancellationToken = default) where TPooled : IPooled =>
			self.ReleaseRange(await self.RequestAsync(count, cancellationToken));

		/// <summary>
		/// Request a range of objects from the <see cref="IPool{TPooled}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<TPooled> RequestRange<TPooled>(this IPool<TPooled> self, int count)
			where TPooled : IPooled
		{
			var collection = new TPooled[count];
			for (var index = 0; index < count; index++)
			{
				collection[index] = self.Request();
			}

			return collection;
		}

		/// <summary>
		/// Request a range of objects from the <see cref="IPool"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<IPooled> RequestRange(this IPool self, int count)
		{
			var collection = new IPooled[count];
			for (var index = 0; index < count; index++)
			{
				collection[index] = self.RequestPooled();
			}

			return collection;
		}

		/// <summary>
		/// Releases all objects in the list.
		/// </summary>
		/// <remarks>The list should be cleared afterward.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReleaseRange<TPooled>(this IPool<TPooled> self, IEnumerable<TPooled> collection)
			where TPooled : IPooled
		{
			foreach (var item in collection)
			{
				self.Release(item);
			}
		}

		/// <summary>
		/// Releases all objects in the list.
		/// </summary>
		/// <remarks>The list should be cleared afterward.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReleaseRange(this IPool self, IEnumerable<IPooled> collection)
		{
			foreach (var item in collection)
			{
				self.ReleasePooled(item);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AddFreeRange<T>(this IPool<T> self, IEnumerable<T> collection) where T : IPooled
		{
			foreach (var item in collection)
			{
				self.Release(item);
			}
		}
	}
}