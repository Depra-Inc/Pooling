﻿namespace Depra.Pooling
{
	public interface IPool
	{
		int Count { get; }
		int Capacity { get; }

		IPooled RequestPooled();
		void ReleasePooled(IPooled pooled);
	}

	public interface IPool<TPooled> : IPool
	{
		/// <summary>
		/// Returns an object from the pool.
		/// </summary>
		TPooled Request();

		/// <summary>
		/// Releases an object back to the pool.
		/// </summary>
		/// <param name="obj">Object to release.</param>
		void Release(TPooled obj);
	}
}