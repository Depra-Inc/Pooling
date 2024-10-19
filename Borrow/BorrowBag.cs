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
			for (var index = 0; index < _values.Count; index++)
			{
				_disposeAction(_values[index]);
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