// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

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
			_values = new List<TValue>(capacity);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Next()
		{
			var randomIndex = RandomIndex();
			var randomInstance = _values[randomIndex];
			_values.RemoveAt(randomIndex);

			return randomInstance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(ref TValue instance) => _values.Add(instance);

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