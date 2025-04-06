// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;

namespace Depra.Borrow
{
	/// <summary>
	/// Interface for a buffer that can be borrowed from.
	/// </summary>
	/// <typeparam name="TValue">Type of the value to store in the buffer.</typeparam>
	public interface IBorrowBuffer<TValue> : IDisposable
	{
		int Count { get; }

		TValue Next();

		void Add(TValue instance);
	}
}