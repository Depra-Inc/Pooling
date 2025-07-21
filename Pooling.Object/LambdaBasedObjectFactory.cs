// SPDX-License-Identifier: Apache-2.0
// © 2024-2025 Depra <n.melnikov@depra.org>

using System;
using System.Runtime.CompilerServices;

namespace Depra.Pooling.Object
{
#if ENABLE_IL2CPP
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
#endif
	public sealed class LambdaBasedObjectFactory<TObject> : IPooledObjectFactory<TObject>
	{
		private readonly Func<TObject> _create;
		private readonly Action<TObject> _destroy;
		private readonly Action<object, TObject> _onEnable;
		private readonly Action<object, TObject> _onDisable;

		public LambdaBasedObjectFactory(
			Func<TObject> create,
			Action<TObject> destroy = null,
			Action<object, TObject> onEnable = null,
			Action<object, TObject> onDisable = null)
		{
			_create = create;
			_destroy = destroy;
			_onEnable = onEnable;
			_onDisable = onDisable;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		TObject IPooledObjectFactory<TObject>.Create(object key) => _create.Invoke();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPooledObjectFactory<TObject>.Destroy(object key, TObject instance) => _destroy?.Invoke(instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPooledObjectFactory<TObject>.OnEnable(object key, TObject instance) => _onEnable?.Invoke(key, instance);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPooledObjectFactory<TObject>.OnDisable(object key, TObject instance) => _onDisable?.Invoke(key, instance);
	}
}