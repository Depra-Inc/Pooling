// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
	public static class BorrowBuffer
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IBorrowBuffer<T> Create<T>(BorrowStrategy strategy, int capacity, Action<T> dispose) => strategy switch
		{
			BorrowStrategy.LIFO => new BorrowStack<T>(capacity, dispose),
			BorrowStrategy.FIFO => new BorrowQueue<T>(capacity, dispose),
			BorrowStrategy.RANDOM => new BorrowBag<T>(capacity, dispose, new Random()),
			_ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
		};
	}
}