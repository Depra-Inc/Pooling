// SPDX-License-Identifier: Apache-2.0
// © 2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if ENABLE_IL2CPP
using Unity.IL2CPP.CompilerServices;
#endif

namespace Depra.Pooling.Object
{
#if ENABLE_IL2CPP
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
#endif
	public sealed class ReflectionBasedObjectFactory<TClass> : IPooledObjectFactory<TClass> where TClass : class
	{
		private readonly object[] _constructorArgs;
		private readonly Dictionary<object, TClass> _instances;

		public ReflectionBasedObjectFactory(params object[] constructorArgs)
		{
			_constructorArgs = constructorArgs;
			_instances = new Dictionary<object, TClass>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		TClass IPooledObjectFactory<TClass>.Create(object key)
		{
			if (_instances.TryGetValue(key, out var instance))
			{
				return instance;
			}

			instance = _constructorArgs == null
				? Activator.CreateInstance<TClass>()
				: (TClass)Activator.CreateInstance(typeof(TClass), _constructorArgs);
			_instances.Add(key, instance);

			return instance;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void IPooledObjectFactory<TClass>.Destroy(object key, TClass instance)
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