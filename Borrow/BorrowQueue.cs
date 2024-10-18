// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Borrow
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
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