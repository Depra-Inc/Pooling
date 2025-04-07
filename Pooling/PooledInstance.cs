// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;

namespace Depra.Pooling
{
	public readonly struct PooledInstance<TPooled> where TPooled : IPooled
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PooledInstance<TPooled> Create(IPoolHandle<TPooled> pool, TPooled @object) => new(pool, @object);

		private readonly IPoolHandle<TPooled> _pool;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal PooledInstance(IPoolHandle<TPooled> pool, TPooled obj)
		{
			Obj = obj;
			_pool = pool;
			Metadata = new PooledInstanceMetadata(Obj.GetHashCode());

			OnPoolSleep();
		}

		public void Dispose()
		{
			Metadata.OnDispose();
			_pool.ReturnInstanceToPool(this);
		}

		public TPooled Obj
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get;
		}

		public PooledInstanceMetadata Metadata { get; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnPoolGet()
		{
			Obj.OnPoolGet();
			Metadata.OnActivate();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void OnPoolSleep()
		{
			Obj.OnPoolSleep();
			Metadata.OnDeactivate();
		}
	}

	/// <summary>
	/// Describes the state of a pooled object.
	/// </summary>
	public enum PooledInstanceState
	{
		/// <summary>
		/// The object is inside the pool, waiting to be used.
		/// </summary>
		AVAILABLE = 0,

		/// <summary>
		/// The object is outside the pool, waiting to return to the pool.
		/// </summary>
		UNAVAILABLE = 1,

		/// <summary>
		/// The object has been disposed and cannot be used anymore.
		/// </summary>
		DISPOSED = 2
	}

	[Serializable]
	public struct PooledInstanceMetadata : IEquatable<PooledInstanceMetadata>
	{
		internal PooledInstanceMetadata(int id)
		{
			Id = id;
			State = PooledInstanceState.AVAILABLE;
			ActiveTime = 0;
		}

		public int Id { get; }
		public float ActiveTime { get; private set; }
		public PooledInstanceState State { get; internal set; }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnActivate()
		{
			ActiveTime = DateTime.Now.Ticks;
			State = PooledInstanceState.AVAILABLE;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnDeactivate()
		{
			ActiveTime = 0f;
			State = PooledInstanceState.UNAVAILABLE;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void OnDispose()
		{
			ActiveTime = 0f;
			State = PooledInstanceState.DISPOSED;
		}

		public override int GetHashCode() => Id;
		public bool Equals(PooledInstanceMetadata other) => Id == other.Id;
		public override bool Equals(object obj) => obj is PooledInstanceMetadata other && Equals(other);
	}
}