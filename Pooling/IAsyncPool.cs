// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System.Threading;
using System.Threading.Tasks;

namespace Depra.Pooling
{
	public interface IAsyncPool<TPooled> : IPool<TPooled>
	{
		Task<TPooled[]> RequestAsync(int count, CancellationToken token);
	}
}