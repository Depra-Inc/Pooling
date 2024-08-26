using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
	public sealed class BorrowBag<TValue> : IBorrowBuffer<TValue>
	{
		private readonly Random _random;
		private readonly List<TValue> _values;
		private readonly Action<TValue> _disposeAction;

		public BorrowBag(int capacity, Action<TValue> disposeAction, Random random)
		{
			_random = random;
			_disposeAction = disposeAction;
			_values = new List<TValue>(Capacity = capacity);
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
		public TValue Next()
		{
			var randomIndex = RandomIndex();
			var randomInstance = _values[randomIndex];
			_values.RemoveAt(randomIndex);

			return randomInstance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Get(int index) => _values[index];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(ref TValue instance) => _values.Add(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(ref TValue instance) => _values.Contains(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IEnumerable<TValue> Enumerate() => _values;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int RandomIndex()
		{
			const int MIN_INDEX = 0;
			var maxIndex = _values.Count;
			var randomIndex = _random.Next(MIN_INDEX, maxIndex);

			return randomIndex;
		}
	}
}