// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.Borrow;

namespace Depra.Pooling.Object
{
	public sealed class ObjectPool<TPooled> : IPool<TPooled>, IPoolHandle<TPooled>, IDisposable where TPooled : IPooled
	{
		private const int MAX_CAPACITY = 10000;
		private const int DEFAULT_CAPACITY = 10;

		private readonly int _maxCapacity;
		private readonly IPooledObjectFactory<TPooled> _objectFactory;
		private readonly IBorrowBuffer<PooledInstance<TPooled>> _passiveInstances;

		public ObjectPool(BorrowStrategy borrowStrategy, IPooledObjectFactory<TPooled> objectFactory,
			object key = null, int capacity = DEFAULT_CAPACITY, int maxCapacity = MAX_CAPACITY)
		{
			Key = key ?? this;
			_maxCapacity = maxCapacity < 0 ? MAX_CAPACITY : maxCapacity;
			_objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
			_passiveInstances = BorrowBuffer.Create<PooledInstance<TPooled>>(borrowStrategy, DisposeInstance, capacity);
		}

		public ObjectPool(IPooledObjectFactory<TPooled> objectFactory,
			IBorrowBuffer<PooledInstance<TPooled>> passiveInstances, object key = null, int maxCapacity = MAX_CAPACITY)
		{
			Key = key ?? this;
			_maxCapacity = maxCapacity < 0 ? MAX_CAPACITY : maxCapacity;
			_objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
			_passiveInstances = passiveInstances ?? throw new ArgumentNullException(nameof(passiveInstances));
		}

		public void Dispose()
		{
			_passiveInstances.Dispose();
			CountAll = 0;
		}

		public object Key { get; }

		public int CountAll
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			private set;
		}

		public int CountActive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => CountAll - CountPassive;
		}

		public int CountPassive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _passiveInstances.Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPooled Request() => Request(out _);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TPooled Request(out PooledInstance<TPooled> instance)
		{
			if (CountPassive > 0)
			{
				// Reuse an instance if there are active instances.
				instance = _passiveInstances.Next();
				instance.Obj.OnPoolReuse();
			}
			else
			{
				// Create a new instance if there are no active instances.
				instance = new PooledInstance<TPooled>(this, _objectFactory.Create(Key));
				instance.Obj.OnPoolCreate(this);
				++CountAll;
			}

			instance.OnPoolGet();
			return instance.Obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Release(TPooled obj)
		{
			Guard.AgainstNull(obj, nameof(obj));

			var instance = PooledInstance<TPooled>.Create(this, obj);
			instance.OnPoolSleep();

			if (CountPassive < _maxCapacity)
			{
				_passiveInstances.Add(ref instance);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisposeInstance(PooledInstance<TPooled> instance)
		{
			var obj = instance.Obj;
			obj.OnPoolSleep();

			_objectFactory.OnDisable(Key, obj);
			_objectFactory.Destroy(Key, obj);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IPooled IPool.RequestPooled() => Request();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPool.ReleasePooled(IPooled pooled) => Release((TPooled) pooled);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPoolHandle<TPooled>.ReturnInstanceToPool(PooledInstance<TPooled> instance) => Release(instance.Obj);
	}
}