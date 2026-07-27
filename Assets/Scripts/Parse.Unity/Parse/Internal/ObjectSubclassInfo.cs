using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Parse.Internal
{
	internal class ObjectSubclassInfo
	{
		public Type TypeInfo { get; private set; }

		public string ClassName { get; private set; }

		public IDictionary<string, string> PropertyMappings { get; private set; }

		private ConstructorInfo Constructor { get; set; }

		public ObjectSubclassInfo(Type type, ConstructorInfo constructor)
		{
			TypeInfo = type.GetTypeInfo();
			ClassName = GetClassName(TypeInfo);
			Constructor = constructor;
			PropertyMappings = (from prop in ReflectionHelpers.GetProperties(type)
				select Tuple.Create(prop, prop.GetCustomAttribute<ParseFieldNameAttribute>(true)) into t
				where t.Item2 != null
				select Tuple.Create(t.Item1, t.Item2.FieldName)).ToDictionary((Tuple<PropertyInfo, string> t) => t.Item1.Name, (Tuple<PropertyInfo, string> t) => t.Item2);
		}

		public ParseObject Instantiate()
		{
			return (ParseObject)Constructor.Invoke(null);
		}

		internal static string GetClassName(Type type)
		{
			ParseClassNameAttribute customAttribute = type.GetCustomAttribute<ParseClassNameAttribute>();
			if (customAttribute == null)
			{
				return null;
			}
			return customAttribute.ClassName;
		}
	}
}
