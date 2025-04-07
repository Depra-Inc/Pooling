// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class BorrowQueue<TValue> : IBorrowBuffer<TValue>
	{
		private readonly Queue<TValue> _values;
		private readonly Action<TValue> _disposeAction;

		public BorrowQueue(int capacity, Action<TValue> disposeAction)
		{
			_disposeAction = disposeAction;
			_values = new Queue<TValue>(capacity);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TValue Next() => _values.Dequeue();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(TValue instance) => _values.Enqueue(instance);
	}
}