// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Borrow
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
	public sealed class BorrowStack<TValue> : IBorrowBuffer<TValue>
	{
		private readonly Stack<TValue> _values;
		private readonly Action<TValue> _disposeAction;

		public BorrowStack(int capacity, Action<TValue> disposeAction)
		{
			_disposeAction = disposeAction;
			_values = new Stack<TValue>(capacity);
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
		public TValue Next() => _values.Pop();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(TValue instance) => _values.Push(instance);
	}
}