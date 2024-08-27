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

		public int Count
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _values.Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Next()
		{
			var randomIndex = RandomIndex();
			var randomInstance = _values[randomIndex];
			_values.RemoveAt(randomIndex);

			return randomInstance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(TValue instance) => _values.Add(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int RandomIndex() => _random.Next(0, _values.Count);
	}
}