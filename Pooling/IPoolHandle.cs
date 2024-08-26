namespace Depra.Pooling
{
	public interface IPoolHandle<T> where T : IPooled
	{
		object Key { get; }

		void ReturnInstanceToPool(PooledInstance<T> instance, bool reRegisterForFinalization);
	}
}