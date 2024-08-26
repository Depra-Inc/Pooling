using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
	public sealed class BorrowQueue<TValue> : IBorrowBuffer<TValue>
	{
		private readonly Queue<TValue> _values;
		private readonly Action<TValue> _disposeAction;

		public BorrowQueue(int capacity, Action<TValue> disposeAction)
		{
			_disposeAction = disposeAction;
			_values = new Queue<TValue>(Capacity = capacity);
		}

		public void Dispose()
		{
			foreach (var instance in _values)
			{
				_disposeAction(instance);
			}

			_values.Clear();
		}

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _values.Count;
		}

		public int Capacity { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Next() => _values.Dequeue();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Get(int index) => _values.ToArray()[index];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(ref TValue instance) => _values.Enqueue(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(ref TValue instance) => _values.Contains(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<TValue> Enumerate() => _values;
	}
}