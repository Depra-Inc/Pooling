// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;

namespace Depra.Pooling.Object
{
	public sealed class LambdaBasedPooledObjectFactory<TObject> : IPooledObjectFactory<TObject> where TObject : class
	{
		private readonly Func<TObject> _create;
		private readonly Action<TObject> _destroy;
		private readonly Action<object, TObject> _onEnable;
		private readonly Action<object, TObject> _onDisable;

		public LambdaBasedPooledObjectFactory(
			Func<TObject> create,
			Action<TObject> destroy,
			Action<object, TObject> onEnable = null,
			Action<object, TObject> onDisable = null)
		{
			_create = create;
			_destroy = destroy;
			_onEnable = onEnable;
			_onDisable = onDisable;
		}

		TObject IPooledObjectFactory<TObject>.Create(object key) => _create.Invoke();
		void IPooledObjectFactory<TObject>.Destroy(object key, TObject instance) => _destroy.Invoke(instance);

		void IPooledObjectFactory<TObject>.OnEnable(object key, TObject instance) => _onEnable?.Invoke(key, instance);
		void IPooledObjectFactory<TObject>.OnDisable(object key, TObject instance) => _onDisable?.Invoke(key, instance);
	}
}