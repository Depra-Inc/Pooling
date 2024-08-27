// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Runtime.CompilerServices;

namespace Depra.Pooling.Object.Benchmarks;

public readonly struct FakePooledObject : IPooled
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IPooled.OnPoolCreate(IPool pool) { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IPooled.OnPoolGet() { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IPooled.OnPoolSleep() { }

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	void IPooled.OnPoolReuse() { }
}