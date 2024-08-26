using System;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
	public static class BorrowBuffer
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IBorrowBuffer<TValue> Create<TValue>(BorrowStrategy strategy,
			Action<TValue> dispose, int capacity) => strategy switch
		{
			BorrowStrategy.LIFO => new BorrowStack<TValue>(capacity, dispose),
			BorrowStrategy.FIFO => new BorrowQueue<TValue>(capacity, dispose),
			BorrowStrategy.RANDOM => new BorrowBag<TValue>(capacity, dispose, new Random()),
			_ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
		};
	}
}