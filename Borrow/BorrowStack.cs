using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
	public sealed class BorrowStack<TValue> : IBorrowBuffer<TValue>
	{
		private readonly Stack<TValue> _values;
		private readonly Action<TValue> _disposeAction;

		public BorrowStack(int capacity, Action<TValue> disposeAction)
		{
			_disposeAction = disposeAction;
			_values = new Stack<TValue>(Capacity = capacity);
		}

		public void Dispose()
		{
			foreach (var value in _values)
			{
				_disposeAction(value);
			}

			_values.Clear();
		}

		public int Count => _values.Count;
		public int Capacity { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Next() => _values.Pop();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Get(int index) => _values.ToArray()[index];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(ref TValue instance) => _values.Push(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(ref TValue instance) => _values.Contains(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<TValue> Enumerate() => _values;
	}
}