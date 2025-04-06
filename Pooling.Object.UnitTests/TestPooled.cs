// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

namespace Depra.Pooling.Object.UnitTests;

internal class TestPooled : IPooled
{
	public bool Created { get; private set; }
	public bool InUse { get; private set; }
	public bool Free { get; private set; }

	void IPooled.OnPoolReuse() { }
	void IPooled.OnPoolCreate(IPool pool) => Created = true;
	void IPooled.OnPoolGet() => InUse = true;
	void IPooled.OnPoolSleep() => Free = true;
}