using System;
using System.Runtime.CompilerServices;

namespace Depra.Pooling
{
	[Serializable]
	public struct PooledInstanceInfo : IEquatable<PooledInstanceInfo>
	{
		internal PooledInstanceInfo(int id)
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

		public bool Equals(PooledInstanceInfo other) => Id == other.Id;

		public override bool Equals(object obj) => obj is PooledInstanceInfo other && Equals(other);
	}
}