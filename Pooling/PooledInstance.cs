// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;

namespace Depra.Pooling
{
	public readonly struct PooledInstance<TPooled> where TPooled : IPooled
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PooledInstance<TPooled> Create(TPooled @object, IPoolHandle<TPooled> pool) => new(pool, @object);

		private readonly IPoolHandle<TPooled> _poolHandle;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal PooledInstance(IPoolHandle<TPooled> pool, TPooled obj) : this()
		{
			Obj = obj;
			_poolHandle = pool;
			Metadata = new PooledInstanceMetadata(Obj.GetHashCode());

			OnPoolSleep();
		}

		public void Dispose()
		{
			Metadata.OnDispose();
			_poolHandle.ReturnInstanceToPool(this, true);
		}

		public TPooled Obj { get; }
		public PooledInstanceMetadata Metadata { get; }

		internal void Activate() => Metadata.OnActivate();
		internal void OnPoolSleep() => Metadata.OnDeactivate();
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