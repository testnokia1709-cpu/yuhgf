using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Parse.Internal
{
	internal class ObjectSubclassingController : IObjectSubclassingController
	{
		private static readonly string parseObjectClassName = "_ParseObject";

		private readonly ReaderWriterLockSlim mutex;

		private readonly IDictionary<string, ObjectSubclassInfo> registeredSubclasses;

		private IDictionary<string, Action> registerActions;

		public ObjectSubclassingController(IDictionary<Type, Action> actions)
		{
			mutex = new ReaderWriterLockSlim();
			registeredSubclasses = new Dictionary<string, ObjectSubclassInfo>();
			registerActions = actions.ToDictionary((KeyValuePair<Type, Action> p) => GetClassName(p.Key), (KeyValuePair<Type, Action> p) => p.Value);
			RegisterSubclass(typeof(ParseObject));
		}

		public string GetClassName(Type type)
		{
			if (type != typeof(ParseObject))
			{
				return ObjectSubclassInfo.GetClassName(type.GetTypeInfo());
			}
			return parseObjectClassName;
		}

		public Type GetType(string className)
		{
			ObjectSubclassInfo value = null;
			mutex.EnterReadLock();
			registeredSubclasses.TryGetValue(className, out value);
			mutex.ExitReadLock();
			if (value == null)
			{
				return null;
			}
			return value.TypeInfo.AsType();
		}

		public bool IsTypeValid(string className, Type type)
		{
			ObjectSubclassInfo value = null;
			mutex.EnterReadLock();
			registeredSubclasses.TryGetValue(className, out value);
			mutex.ExitReadLock();
			if (value != null)
			{
				return value.TypeInfo == type.GetTypeInfo();
			}
			return type == typeof(ParseObject);
		}

		public void RegisterSubclass(Type type)
		{
			Type typeInfo = type.GetTypeInfo();
			if (!typeof(ParseObject).GetTypeInfo().IsAssignableFrom(typeInfo))
			{
				throw new ArgumentException("Cannot register a type that is not a subclass of ParseObject");
			}
			string className = GetClassName(type);
			try
			{
				mutex.EnterWriteLock();
				ObjectSubclassInfo value = null;
				if (registeredSubclasses.TryGetValue(className, out value))
				{
					if (typeInfo.IsAssignableFrom(value.TypeInfo))
					{
						return;
					}
					if (!value.TypeInfo.IsAssignableFrom(typeInfo))
					{
						throw new ArgumentException("Tried to register both " + value.TypeInfo.FullName + " and " + typeInfo.FullName + " as the ParseObject subclass of " + className + ". Cannot determine the right class to use because neither inherits from the other.");
					}
				}
				ConstructorInfo constructorInfo = type.FindConstructor();
				if (constructorInfo == null)
				{
					throw new ArgumentException("Cannot register a type that does not implement the default constructor!");
				}
				registeredSubclasses[className] = new ObjectSubclassInfo(type, constructorInfo);
			}
			finally
			{
				mutex.ExitWriteLock();
			}
			Action value2 = null;
			if (registerActions.TryGetValue(className, out value2))
			{
				value2();
			}
		}

		public void UnregisterSubclass(Type type)
		{
			mutex.EnterWriteLock();
			registeredSubclasses.Remove(GetClassName(type));
			mutex.ExitWriteLock();
		}

		public ParseObject Instantiate(string className)
		{
			ObjectSubclassInfo value = null;
			mutex.EnterReadLock();
			registeredSubclasses.TryGetValue(className, out value);
			mutex.ExitReadLock();
			if (value == null)
			{
				return new ParseObject(className);
			}
			return value.Instantiate();
		}

		public IDictionary<string, string> GetPropertyMappings(string className)
		{
			ObjectSubclassInfo value = null;
			mutex.EnterReadLock();
			registeredSubclasses.TryGetValue(className, out value);
			if (value == null)
			{
				registeredSubclasses.TryGetValue(parseObjectClassName, out value);
			}
			mutex.ExitReadLock();
			return value.PropertyMappings;
		}
	}
}
