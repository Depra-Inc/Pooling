// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using Depra.Borrow;

namespace Depra.Pooling
{
	[System.Serializable]
	public sealed record PoolConfiguration
	{
		private const int MAX_CAPACITY = 1000;

		public PoolConfiguration(int initCapacity = 10,
			int maxCapacity = MAX_CAPACITY,
			BorrowStrategy borrowStrategy = BorrowStrategy.FIFO,
			OverflowStrategy overflowStrategy = OverflowStrategy.REUSE)
		{
			InitCapacity = initCapacity < 0 ? 0 : initCapacity;
			MaxCapacity = maxCapacity < 0 ? MAX_CAPACITY : maxCapacity;
			BorrowStrategy = borrowStrategy;
			OverflowStrategy = overflowStrategy;
		}

		public int InitCapacity { get; }
		public int MaxCapacity { get; }
		public BorrowStrategy BorrowStrategy { get; }
		public OverflowStrategy OverflowStrategy { get; }
	}
}