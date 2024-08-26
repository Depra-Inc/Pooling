// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

namespace Depra.Pooling.Object.Benchmarks;

internal sealed class PoolableObject : IPooled
{
	void IPooled.OnPoolCreate(IPool pool) { }
	void IPooled.OnPoolGet() { }
	void IPooled.OnPoolSleep() { }
	void IPooled.OnPoolReuse() { }
}