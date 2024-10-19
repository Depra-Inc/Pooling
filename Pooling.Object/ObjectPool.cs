// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;
using Depra.Borrow;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Pooling.Object
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ObjectPool<TPooled> : IPool<TPooled>, IPoolHandle<TPooled>, IDisposable where TPooled : IPooled
	{
		private readonly int _maxCapacity;
		private readonly OverflowStrategy _overflowStrategy;
		private readonly IPooledObjectFactory<TPooled> _objectFactory;
		private readonly IBorrowBuffer<PooledInstance<TPooled>> _passiveInstances;
		private readonly BorrowCircularList<PooledInstance<TPooled>> _activeInstances;

		public ObjectPool(PoolConfiguration configuration, IPooledObjectFactory<TPooled> factory, object key = null)
		{
			Key = key ?? this;
			_maxCapacity = configuration.MaxCapacity;
			_overflowStrategy = configuration.OverflowStrategy;
			_objectFactory = factory ?? throw new ArgumentNullException(nameof(factory));
			_activeInstances = new BorrowCircularList<PooledInstance<TPooled>>(configuration.MaxCapacity);
			_passiveInstances = BorrowBuffer.Create<PooledInstance<TPooled>>(
				configuration.BorrowStrategy,
				configuration.InitialCapacity,
				DisposeInstance);
		}

		public void Dispose()
		{
			_activeInstances.Dispose();
			_passiveInstances.Dispose();
			CountAll = 0;
		}

		public object Key { get; }

		public int CountAll
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
			TPooled obj;
			if (CountPassive == 0)
			{
				obj = CountActive < _maxCapacity
					? CreateInstance(out instance)
					: CreateInstanceOverCapacity(out instance);
			}
			else
			{
				obj = ReusePassiveInstance(out instance);
			}

			instance.OnPoolGet();
			_objectFactory.OnEnable(Key, obj);
			_activeInstances.Add(instance);

			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Release(TPooled obj)
		{
			Guard.AgainstNull(obj, nameof(obj));

			var instance = PooledInstance<TPooled>.Create(this, obj);
			instance.OnPoolSleep();
			_objectFactory.OnDisable(Key, obj);

			if (CountPassive < _maxCapacity)
			{
				_passiveInstances.Add(instance);
			}

			if (CountActive > 0)
			{
				_activeInstances.Remove(instance);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TPooled CreateInstanceOverCapacity(out PooledInstance<TPooled> instance) => _overflowStrategy switch
		{
			OverflowStrategy.REQUEST => CreateInstance(out instance),
			OverflowStrategy.REUSE => ReuseActiveInstance(out instance),
			OverflowStrategy.THROW_EXCEPTION => throw new PoolOverflowed(Key),
			_ => throw new ArgumentOutOfRangeException(nameof(OverflowStrategy))
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TPooled CreateInstance(out PooledInstance<TPooled> instance)
		{
			instance = new PooledInstance<TPooled>(this, _objectFactory.Create(Key));
			var obj = instance.Obj;
			obj.OnPoolCreate(this);
			++CountAll;

			return obj;
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
		private TPooled ReuseActiveInstance(out PooledInstance<TPooled> instance)
		{
			instance = _activeInstances.Next();
			var obj = instance.Obj;
			obj.OnPoolReuse();

			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TPooled ReusePassiveInstance(out PooledInstance<TPooled> instance)
		{
			instance = _passiveInstances.Next();
			var obj = instance.Obj;
			obj.OnPoolReuse();

			return obj;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		IPooled IPool.RequestPooled() => Request();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPool.ReleasePooled(IPooled pooled) => Release((TPooled)pooled);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPoolHandle<TPooled>.ReturnInstanceToPool(PooledInstance<TPooled> instance) => Release(instance.Obj);
	}
}