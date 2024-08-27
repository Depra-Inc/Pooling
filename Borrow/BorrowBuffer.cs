// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
	public static class BorrowBuffer
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IBorrowBuffer<T> Create<T>(BorrowStrategy strategy, Action<T> dispose, int capacity) => strategy switch
		{
			BorrowStrategy.LIFO => new BorrowStack<T>(capacity, dispose),
			BorrowStrategy.FIFO => new BorrowQueue<T>(capacity, dispose),
			BorrowStrategy.RANDOM => new BorrowBag<T>(capacity, dispose, new Random()),
			_ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
		};
	}
}