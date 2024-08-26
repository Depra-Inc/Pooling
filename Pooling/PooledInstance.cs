using System.Runtime.CompilerServices;

namespace Depra.Pooling
{
	public readonly struct PooledInstance<TPooled> where TPooled : IPooled
	{
		private readonly IPoolHandle<TPooled> _poolHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal PooledInstance(IPoolHandle<TPooled> pool, TPooled obj) : this()
		{
			Obj = obj;
			_poolHandle = pool;
			Info = new PooledInstanceInfo(Obj.GetHashCode());

			Deactivate();
		}

		public void Dispose()
		{
			Info.OnDispose();
			_poolHandle.ReturnInstanceToPool(this, true);
		}

		public TPooled Obj { get; }
		public PooledInstanceInfo Info { get; }

		internal void Activate() => Info.OnActivate();
		internal void Deactivate() => Info.OnDeactivate();
	}

	public static class PolledInstanceExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PooledInstance<TPooled> ToInstance<TPooled>(this TPooled @object, IPoolHandle<TPooled> pool)
			where TPooled : IPooled => new(pool, @object);
	}
}