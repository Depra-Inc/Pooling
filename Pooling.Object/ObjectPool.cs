// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.Borrow;

namespace Depra.Pooling.Object
{
	public sealed class ObjectPool<TPooled> : IPool<TPooled>, IPoolHandle<TPooled>, IDisposable where TPooled : IPooled
	{
		private const int DEFAULT_CAPACITY = 16;
		private readonly IPooledObjectFactory<TPooled> _objectFactory;
		private readonly IBorrowBuffer<PooledInstance<TPooled>> _activeInstances;
		private readonly IBorrowBuffer<PooledInstance<TPooled>> _passiveInstances;

		public ObjectPool(BorrowStrategy borrowStrategy, IPooledObjectFactory<TPooled> objectFactory,
			int capacity = DEFAULT_CAPACITY, object key = null)
		{
			Key = key ?? this;
			_objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
			_activeInstances = BorrowBuffer.Create<PooledInstance<TPooled>>(borrowStrategy, DisposeInstance, capacity);
			_passiveInstances = BorrowBuffer.Create<PooledInstance<TPooled>>(borrowStrategy, DisposeInstance, capacity);
		}

		public ObjectPool(IPooledObjectFactory<TPooled> objectFactory,
			IBorrowBuffer<PooledInstance<TPooled>> activeInstances,
			IBorrowBuffer<PooledInstance<TPooled>> passiveInstances, object key = null)
		{
			Key = key ?? this;
			_objectFactory = objectFactory ?? throw new ArgumentNullException(nameof(objectFactory));
			_activeInstances = activeInstances ?? throw new ArgumentNullException(nameof(activeInstances));
			_passiveInstances = passiveInstances ?? throw new ArgumentNullException(nameof(passiveInstances));
		}

		public void Dispose()
		{
			_activeInstances.Dispose();
			_passiveInstances.Dispose();
		}

		public object Key { get; }
		public int Count => ActiveCount + PassiveCount;
		public int ActiveCount => _activeInstances.Count;
		public int PassiveCount => _passiveInstances.Count;

		public TPooled Request() => Request(out _);

		public TPooled Request(out PooledInstance<TPooled> instance)
		{
			if (_passiveInstances.Count > 0)
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
			}

			instance.Activate();
			instance.Obj.OnPoolGet();
			_activeInstances.Add(ref instance);

			return instance.Obj;
		}

		public void Release(TPooled obj)
		{
			Guard.AgainstNull(obj, nameof(obj));
			Guard.AgainstEmpty(_activeInstances, () => new NoInstanceToMakePassive());

			PassivateInstance(_activeInstances.Next());
		}

		public void AddInactive(TPooled obj) => PassivateInstance(PooledInstance<TPooled>.Create(obj, this));

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void PassivateInstance(PooledInstance<TPooled> instance)
		{
			instance.OnPoolSleep();
			instance.Obj.OnPoolSleep();
			_passiveInstances.Add(ref instance);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void DisposeInstance(PooledInstance<TPooled> instance)
		{
			var obj = instance.Obj;
			obj.OnPoolSleep();
			_objectFactory.OnDisable(Key, obj);
			_objectFactory.Destroy(Key, obj);
		}

		IPooled IPool.RequestPooled() => Request();

		void IPool.ReleasePooled(IPooled pooled) => Release((TPooled) pooled);

		void IPoolHandle<TPooled>.ReturnInstanceToPool(PooledInstance<TPooled> instance, bool reRegisterForFinalization)
		{
			if (reRegisterForFinalization)
			{
				_activeInstances.Add(ref instance);
			}

			Release(instance.Obj);
		}
	}
}