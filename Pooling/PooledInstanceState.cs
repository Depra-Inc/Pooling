namespace Depra.Pooling
{
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
}