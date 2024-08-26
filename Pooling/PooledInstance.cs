using System.Runtime.CompilerServices;

namespace Depra.Pooling
{
	public readonly struct PooledInstance<TPooled> where TPooled : IPooled
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Dispose(PooledInstance<TPooled> self) => self.Dispose();

		private readonly IPoolHandle<TPooled> _poolHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal PooledInstance(IPoolHandle<TPooled> pool, TPooled obj) : this()
		{
			_poolHandle = pool;
			Obj = obj;
			//Info = new PooledInstanceInfo(obj.GetHashCode());

			Deactivate();
		}

		public void Dispose() => _poolHandle.ReturnInstanceToPool(this, true);

		public TPooled Obj { get; }

		//public PooledInstanceInfo Info { get; }

		internal void Activate()
		{
			//Info.OnActivate();
		}

		internal void Deactivate()
		{
			//Info.OnDeactivate();
		}
	}

	public static class PolledInstanceExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PooledInstance<TPooled> ToInstance<TPooled>(this TPooled @object, IPoolHandle<TPooled> pool)
			where TPooled : IPooled => new(pool, @object);
	}
}