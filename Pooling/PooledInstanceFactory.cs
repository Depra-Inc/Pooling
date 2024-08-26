using System.Runtime.CompilerServices;
using Depra.Borrow;

namespace Depra.Pooling
{
	public sealed class PooledInstanceFactory<TPooled> where TPooled : IPooled
	{
		private readonly object _key;
		private readonly IPoolHandle<TPooled> _pool;
		private readonly IPooledObjectFactory<TPooled> _objectFactory;
		private readonly IBorrowBuffer<PooledInstance<TPooled>> _instances;

		public PooledInstanceFactory(object key,
			IPoolHandle<TPooled> pool,
			IPooledObjectFactory<TPooled> objectFactory,
			IBorrowBuffer<PooledInstance<TPooled>> instances)
		{
			_key = key;
			_pool = pool;
			_objectFactory = objectFactory;
			_instances = instances;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PooledInstance<TPooled> MakeActiveInstance(out bool reuse)
		{
			PooledInstance<TPooled> instance;
			if (_instances.Count > 0 && _instances.Count == _instances.Capacity)
			{
				// Reuse an instance if there are active instances.
				reuse = true;
				instance = _instances.Next();
			}
			else
			{
				// Create a new instance if there are no active instances.
				reuse = false;
				var obj = _objectFactory.Create(_pool);
				instance = new PooledInstance<TPooled>(_pool, obj);
				_instances.Add(ref instance);
			}

			return instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PooledInstance<TPooled> MakePassiveInstance()
		{
			Guard.AgainstEmpty(_instances, () => new NoInstanceToMakePassive());

			var instance = _instances.Next();
			instance.Deactivate();
			_instances.Add(ref instance);

			return instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PooledInstance<TPooled> MakePassiveInstance(TPooled obj)
		{
			Guard.AgainstNull(obj, nameof(obj));

			var instance = obj.ToInstance(_pool);
			instance.Deactivate();
			_instances.Add(ref instance);

			return instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DestroyInstance(ref PooledInstance<TPooled> instance)
		{
			var obj = instance.Obj;
			_objectFactory.OnDisable(_key, obj);
			_objectFactory.Destroy(_key, obj);
		}
	}
}