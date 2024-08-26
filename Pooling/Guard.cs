using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Depra.Borrow;

[assembly: InternalsVisibleTo("Depra.Pooling.Object")]

namespace Depra.Pooling
{
	internal static class Guard
	{
		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Against(bool condition, Func<Exception> exception)
		{
			if (condition)
			{
				throw exception();
			}
		}

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull(object value, string name) =>
			AgainstNull(value, () => new ArgumentNullException(name));

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstNull<T>(T value, Func<Exception> exception) => Against(value == null, exception);

		[Conditional("DEBUG")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void AgainstEmpty<TValue>(IBorrowBuffer<TValue> buffer, Func<Exception> exception) =>
			Against(buffer.Count <= 0, exception);
	}

	internal sealed class NoInstanceAvailable : Exception
	{
		public NoInstanceAvailable() : base("No instance available in the pool.") { }
	}

	internal sealed class NoInstanceToMakePassive : Exception
	{
		public NoInstanceToMakePassive() : base("No instances to make passive.") { }
	}
}