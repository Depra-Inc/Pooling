using System;
using Depra.Borrow;

namespace Depra.Pooling.Object
{
	public sealed class ObjectPool<TPooled> : IPool<TPooled>, IPoolHandle<TPooled>, IDisposable where TPooled : IPooled
	{
		private const int DEFAULT_CAPACITY = 16;

		private readonly PooledInstanceFactory<TPooled> _instanceFactory;

		public ObjectPool(BorrowStrategy borrowStrategy, IPooledObjectFactory<TPooled> objectFactory,
			int capacity = DEFAULT_CAPACITY, object key = null)
		{
			Key = key ?? typeof(TPooled);
			_borrowBuffer = BorrowBuffer.Create<PooledInstance<TPooled>>(borrowStrategy, DisposeInstance, capacity);
			_instanceFactory = new PooledInstanceFactory<TPooled>(Key, this, objectFactory, _borrowBuffer);
		}

		private readonly IBorrowBuffer<PooledInstance<TPooled>> _borrowBuffer;

		public ObjectPool(IBorrowBuffer<PooledInstance<TPooled>> borrowBuffer,
			IPooledObjectFactory<TPooled> objectFactory, object key = null)
		{
			Key = key ?? typeof(TPooled);
			_borrowBuffer = borrowBuffer ?? throw new ArgumentNullException(nameof(borrowBuffer));
			_instanceFactory = new PooledInstanceFactory<TPooled>(Key, this, objectFactory, _borrowBuffer);
		}

		public void Dispose() => _borrowBuffer.Dispose();

		public object Key { get; }
		public int Count => _borrowBuffer.Count;
		public int Capacity => _borrowBuffer.Capacity;

		public TPooled Request()
		{
			Request(out var obj);
			return obj;
		}

		public PooledInstance<TPooled> Request(out TPooled obj)
		{
			var instance = _instanceFactory.MakeActiveInstance(out var reuse);
			obj = instance.Obj;

			if (reuse)
			{
				obj.OnPoolReuse();
			}
			else
			{
				obj.OnPoolCreate(this);
			}

			obj.OnPoolGet();

			return instance;
		}

		public void Release(TPooled obj)
		{
			Guard.AgainstNull(obj, nameof(obj));
			_instanceFactory.MakePassiveInstance().Obj.OnPoolSleep();
		}

		public void AddInactive(TPooled obj)
		{
			_instanceFactory.MakePassiveInstance(obj);
			obj.OnPoolSleep();
		}

		private void DisposeInstance(PooledInstance<TPooled> instance)
		{
			instance.Obj.OnPoolSleep();
			_instanceFactory.DestroyInstance(ref instance);
		}

		IPooled IPool.RequestPooled() => Request();

		void IPool.ReleasePooled(IPooled pooled) => Release((TPooled) pooled);

		void IPoolHandle<TPooled>.ReturnInstanceToPool(PooledInstance<TPooled> instance, bool reRegisterForFinalization)
		{
			if (reRegisterForFinalization)
			{
				_borrowBuffer.Add(ref instance);
			}

			Release(instance.Obj);
		}
	}
}