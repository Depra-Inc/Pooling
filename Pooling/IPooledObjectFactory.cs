// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

namespace Depra.Pooling
{
	public interface IPooledObjectFactory<TSource>
	{
		/// <summary>
		/// Create new Instance of Type <see cref="TSource"/> using source object.
		/// </summary>
		/// <param name="key">Unique identifier for pool.</param>
		/// <returns>New instance.</returns>
		TSource Create(object key);

		/// <summary>
		/// Destroy Instance of Type <see cref="TSource"/>.
		/// </summary>
		/// <param name="key">Unique identifier for pool.</param>
		/// <param name="instance">Instance to destroy.</param>
		void Destroy(object key, TSource instance);

		/// <summary>
		/// Called when instance is enabled.
		/// </summary>
		/// <param name="key">Unique identifier for pool.</param>
		/// <param name="instance">Instance to enable.</param>
		void OnEnable(object key, TSource instance);

		/// <summary>
		/// Called when instance is disabled.
		/// </summary>
		/// <param name="key">Unique identifier for pool.</param>
		/// <param name="instance">Instance to disable.</param>
		void OnDisable(object key, TSource instance);
	}
}