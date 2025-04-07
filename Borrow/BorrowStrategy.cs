// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

namespace Depra.Borrow
{
	public enum BorrowStrategy : byte
	{
		/// <summary>
		/// Last in first out (stack).
		/// </summary>
		LIFO,

		/// <summary>
		/// First in first out (queue).
		/// </summary>
		FIFO,

		/// <summary>
		/// Random out.
		/// </summary>
		RANDOM
	}
}