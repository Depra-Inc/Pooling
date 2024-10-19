// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Borrow
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class BorrowCircularList<TValue> : IBorrowBuffer<TValue>
	{
		private int _head;
		private int _tail;
		private bool _isFull;
		private TValue[] _values;

		public int Count { get; private set; }

		public BorrowCircularList(int capacity)
		{
			Count = 0;
			_head = 0;
			_tail = 0;
			_isFull = false;
			_values = new TValue[capacity];
		}

		public void Dispose()
		{
			Count = 0;
			_head = 0;
			_tail = 0;
			_values = null;
			_isFull = false;
		}

		public void Add(TValue instance)
		{
			_values[_tail] = instance;

			if (_isFull)
			{
				_head = (_head + 1) % _values.Length;
			}

			_tail = (_tail + 1) % _values.Length;
			Count = Math.Min(Count + 1, _values.Length);
			_isFull = Count == _values.Length;
		}

		public TValue Next()
		{
			var item = _values[_head];
			_head = (_head + 1) % _values.Length;
			Count--;
			_isFull = false;

			return item;
		}

		public bool Remove(TValue instance)
		{
			var index = Array.IndexOf(_values, instance, _head, Count);
			if (index == -1)
			{
				return false;
			}
			
			for (var i = index; i != _tail; i = (i + 1) % _values.Length)
			{
				_values[i] = _values[(i + 1) % _values.Length];
			}

			_tail = (_tail - 1 + _values.Length) % _values.Length;
			Count--;
			_isFull = false;

			return true;
		}
	}
}