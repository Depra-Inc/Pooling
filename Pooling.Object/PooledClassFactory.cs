// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Depra.Pooling.Object
{
	public sealed class PooledClassFactory<TClass> : IPooledObjectFactory<TClass> where TClass : class, new()
	{
		private readonly object[] _constructorArgs;
		private readonly Dictionary<object, TClass> _instances;

		public PooledClassFactory(params object[] constructorArgs)
		{
			_constructorArgs = constructorArgs;
			_instances = new Dictionary<object, TClass>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TClass Create(object key)
		{
			if (_instances.TryGetValue(key, out var instance))
			{
				return instance;
			}

			instance = _constructorArgs == null
				? Activator.CreateInstance<TClass>()
				: (TClass) Activator.CreateInstance(typeof(TClass), _constructorArgs);
			_instances.Add(key, instance);

			return instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Destroy(object key, TClass instance)
		{
			if (instance is IDisposable disposableInstance)
			{
				disposableInstance.Dispose();
			}

			if (_instances.TryGetValue(key, out var actualInstance) && instance == actualInstance)
			{
				_instances.Remove(key);
			}
		}

		void IPooledObjectFactory<TClass>.OnEnable(object key, TClass instance) { }
		void IPooledObjectFactory<TClass>.OnDisable(object key, TClass instance) { }
	}
}