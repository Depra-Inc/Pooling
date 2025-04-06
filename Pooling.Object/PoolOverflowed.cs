// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;

namespace Depra.Pooling.Object
{
	internal sealed class PoolOverflowed : Exception
	{
		public PoolOverflowed(object key) : base($"Pool with key {key} is overflowed") { }
	}
}