// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Pooling
{
	public interface IPool
	{
		int Count { get; }
		int ActiveCount { get; }
		int PassiveCount { get; }

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