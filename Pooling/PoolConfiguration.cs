// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using Depra.Borrow;

namespace Depra.Pooling
{
	[Serializable]
	public sealed record PoolConfiguration
	{
		private const int MAX_CAPACITY = 1000;

		public PoolConfiguration(int initialCapacity = 10,
			int maxCapacity = MAX_CAPACITY,
			BorrowStrategy borrowStrategy = BorrowStrategy.FIFO,
			OverflowStrategy overflowStrategy = OverflowStrategy.REUSE)
		{
			InitialCapacity = initialCapacity < 0 ? 0 : initialCapacity;
			MaxCapacity = maxCapacity < 0 ? MAX_CAPACITY : maxCapacity;
			BorrowStrategy = borrowStrategy;
			OverflowStrategy = overflowStrategy;
		}

		public int InitialCapacity { get; }
		public int MaxCapacity { get; }
		public BorrowStrategy BorrowStrategy { get; }
		public OverflowStrategy OverflowStrategy { get; }
	}
}