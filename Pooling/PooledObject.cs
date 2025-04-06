// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

namespace Depra.Pooling
{
	public abstract class PooledObject : IPooled
	{
		protected IPool Pool { get; private set; }

		public void OnPoolCreate(IPool pool) => Pool = pool;

		public virtual void OnPoolGet() { }

		public virtual void OnPoolSleep() { }

		public virtual void OnPoolReuse() { }
	}
}