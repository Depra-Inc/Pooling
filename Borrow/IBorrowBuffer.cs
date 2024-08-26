using System;
using System.Collections.Generic;

namespace Depra.Borrow
{
	/// <summary>
	/// Interface for a buffer that can be borrowed from.
	/// </summary>
	/// <typeparam name="TValue">Type of the value to store in the buffer.</typeparam>
	public interface IBorrowBuffer<TValue> : IDisposable
	{
		int Count { get; }
		int Capacity { get; }

		TValue Next();

		TValue Get(int index);

		void Add(ref TValue instance);

		bool Contains(ref TValue instance);

		IEnumerable<TValue> Enumerate();
	}
}