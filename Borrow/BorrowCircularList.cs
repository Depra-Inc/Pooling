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
	public sealed class BorrowCircularList<TValue> : IBorrowBuffer<TValue>
	{
		private int _head;
		private int _tail;
		private bool _isFull;
		private TValue[] _values;
		private readonly Action<TValue> _disposeAction;

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private set;
		}

		public BorrowCircularList(int capacity, Action<TValue> disposeAction)
		{
			Count = 0;
			_head = 0;
			_tail = 0;
			_isFull = false;
			_values = new TValue[capacity];
			_disposeAction = disposeAction;
		}

		public void Dispose()
		{
			for (var index = 0; index < _values.Length; index++)
			{
				_disposeAction(_values[index]);
			}

			Count = 0;
			_head = 0;
			_tail = 0;
			_values = null;
			_isFull = false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Next()
		{
			var item = _values[_head];
			_head = (_head + 1) % _values.Length;
			Count--;
			_isFull = false;

			return item;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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