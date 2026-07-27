using System;
using System.Collections.Generic;
using Parse.Internal;

namespace Parse.Utilities
{
	public static class Conversion
	{
		private static readonly Dictionary<Tuple<Type, Type>, Type> interfaceLookupCache = new Dictionary<Tuple<Type, Type>, Type>();

		public static T As<T>(object value) where T : class
		{
			return ConvertTo<T>(value) as T;
		}

		public static T To<T>(object value)
		{
			return (T)ConvertTo<T>(value);
		}

		internal static object ConvertTo<T>(object value)
		{
			if (value is T || value == null)
			{
				return value;
			}
			if (ReflectionHelpers.IsPrimitive(typeof(T)))
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}
			if (ReflectionHelpers.IsConstructedGenericType(typeof(T)))
			{
				if (ReflectionHelpers.IsNullable(typeof(T)))
				{
					Type type = ReflectionHelpers.GetGenericTypeArguments(typeof(T))[0];
					if (ReflectionHelpers.IsPrimitive(type))
					{
						return (T)Convert.ChangeType(value, type);
					}
				}
				Type interfaceType = GetInterfaceType(value.GetType(), typeof(IList<>));
				if (interfaceType != null && typeof(T).GetGenericTypeDefinition() == typeof(IList<>))
				{
					return Activator.CreateInstance(typeof(FlexibleListWrapper<, >).MakeGenericType(ReflectionHelpers.GetGenericTypeArguments(typeof(T))[0], ReflectionHelpers.GetGenericTypeArguments(interfaceType)[0]), value);
				}
				Type interfaceType2 = GetInterfaceType(value.GetType(), typeof(IDictionary<, >));
				if (interfaceType2 != null && typeof(T).GetGenericTypeDefinition() == typeof(IDictionary<, >))
				{
					return Activator.CreateInstance(typeof(FlexibleDictionaryWrapper<, >).MakeGenericType(ReflectionHelpers.GetGenericTypeArguments(typeof(T))[1], ReflectionHelpers.GetGenericTypeArguments(interfaceType2)[1]), value);
				}
			}
			return value;
		}

		private static Type GetInterfaceType(Type objType, Type genericInterfaceType)
		{
			if (ReflectionHelpers.IsConstructedGenericType(genericInterfaceType))
			{
				genericInterfaceType = genericInterfaceType.GetGenericTypeDefinition();
			}
			Tuple<Type, Type> key = new Tuple<Type, Type>(objType, genericInterfaceType);
			if (interfaceLookupCache.ContainsKey(key))
			{
				return interfaceLookupCache[key];
			}
			foreach (Type @interface in ReflectionHelpers.GetInterfaces(objType))
			{
				if (ReflectionHelpers.IsConstructedGenericType(@interface) && @interface.GetGenericTypeDefinition() == genericInterfaceType)
				{
					return interfaceLookupCache[key] = @interface;
				}
			}
			return null;
		}
	}
}
