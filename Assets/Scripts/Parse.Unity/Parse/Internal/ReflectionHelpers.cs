using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Parse.Internal
{
	internal static class ReflectionHelpers
	{
		internal static IEnumerable<PropertyInfo> GetProperties(Type type)
		{
			return type.GetProperties();
		}

		internal static MethodInfo GetMethod(Type type, string name, Type[] parameters)
		{
			return type.GetMethod(name, parameters);
		}

		internal static bool IsPrimitive(Type type)
		{
			return type.GetTypeInfo().IsPrimitive;
		}

		internal static IEnumerable<Type> GetInterfaces(Type type)
		{
			return type.GetInterfaces();
		}

		internal static bool IsConstructedGenericType(Type type)
		{
			if (type.IsGenericType)
			{
				return !type.IsGenericTypeDefinition;
			}
			return false;
		}

		internal static IEnumerable<ConstructorInfo> GetConstructors(Type type)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			return type.GetConstructors(bindingAttr);
		}

		internal static Type[] GetGenericTypeArguments(Type type)
		{
			return type.GetGenericArguments();
		}

		internal static PropertyInfo GetProperty(Type type, string name)
		{
			return type.GetProperty(name);
		}

		internal static ConstructorInfo FindConstructor(this Type self, params Type[] parameterTypes)
		{
			return (from constructor in GetConstructors(self)
				let parameters = constructor.GetParameters()
				let types = parameters.Select((ParameterInfo p) => p.ParameterType)
				where types.SequenceEqual(parameterTypes)
				select constructor).SingleOrDefault();
		}

		internal static bool IsNullable(Type t)
		{
			if (t.IsGenericType && !t.IsGenericTypeDefinition)
			{
				return t.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
			}
			return false;
		}
	}
}
