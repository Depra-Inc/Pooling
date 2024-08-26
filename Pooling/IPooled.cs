namespace Depra.Pooling
{
	/// <summary>
	/// Classes that implement <see cref="IPooled"/> will receive calls from the <see cref="IPool"/>.
	/// </summary>
	public interface IPooled
	{
		/// <summary>
		/// Invoked when the object is instantiated.
		/// </summary>
		void OnPoolCreate(IPool pool);

		/// <summary>
		/// Invoked when the object is grabbed from the <see cref="IPool"/>.
		/// </summary>s
		void OnPoolGet();

		/// <summary>
		/// Invoked when the object is released back to the <see cref="IPool"/>.
		/// </summary>
		void OnPoolSleep();

		/// <summary>
		/// Invoked when the object is reused.
		/// </summary>
		void OnPoolReuse();
	}
}