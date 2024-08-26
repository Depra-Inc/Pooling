// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Pooling
{
	/// <summary>
	/// Common interface for all pools.
	/// </summary>
	public interface IPool
	{
		int Count { get; }
		int ActiveCount { get; }
		int PassiveCount { get; }

		IPooled RequestPooled();
		void ReleasePooled(IPooled pooled);
	}

	/// <summary>
	/// Common interface for all typed pools.
	/// </summary>
	/// <typeparam name="TPooled">Type of the pooled object.</typeparam>
	public interface IPool<TPooled> : IPool
	{
		TPooled Request();
		void Release(TPooled obj);
	}
}