using System;
using System.Collections.Generic;

namespace Depra.Borrow
{
	public interface IBorrowBuffer<T> : IDisposable
	{
		int Count { get; }
		int Capacity { get; }

		T Next();

		T Get(int index);

		void Add(ref T instance);

		bool Contains(ref T instance);

		IEnumerable<T> Enumerate();
	}
}