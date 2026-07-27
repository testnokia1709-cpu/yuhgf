using System;
using System.Collections.Generic;
using System.Reflection;
using Amazon.Util.Internal;

namespace Amazon.Runtime
{
	public class ConstantClass
	{
		private static readonly object staticFieldsLock = new object();

		private static Dictionary<Type, Dictionary<string, ConstantClass>> staticFields = new Dictionary<Type, Dictionary<string, ConstantClass>>();

		public string Value { get; private set; }

		protected ConstantClass(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Intern().Value;
		}

		public string ToString(IFormatProvider provider)
		{
			return Intern().Value;
		}

		public static implicit operator string(ConstantClass value)
		{
			if (value == null)
			{
				return null;
			}
			return value.Intern().Value;
		}

		internal ConstantClass Intern()
		{
			if (!staticFields.ContainsKey(GetType()))
			{
				LoadFields(GetType());
			}
			ConstantClass value;
			if (!staticFields[GetType()].TryGetValue(Value, out value))
			{
				return this;
			}
			return value;
		}

		protected static T FindValue<T>(string value) where T : ConstantClass
		{
			if (value == null)
			{
				return null;
			}
			if (!staticFields.ContainsKey(typeof(T)))
			{
				LoadFields(typeof(T));
			}
			ConstantClass value2;
			if (!staticFields[typeof(T)].TryGetValue(value, out value2))
			{
				return TypeFactory.GetTypeInfo(typeof(T)).GetConstructor(new ITypeInfo[1] { TypeFactory.GetTypeInfo(typeof(string)) }).Invoke(new object[1] { value }) as T;
			}
			return value2 as T;
		}

		private static void LoadFields(Type t)
		{
			if (staticFields.ContainsKey(t))
			{
				return;
			}
			lock (staticFieldsLock)
			{
				if (staticFields.ContainsKey(t))
				{
					return;
				}
				Dictionary<string, ConstantClass> dictionary = new Dictionary<string, ConstantClass>(StringComparer.OrdinalIgnoreCase);
				foreach (FieldInfo field in TypeFactory.GetTypeInfo(t).GetFields())
				{
					if (field.IsStatic && field.FieldType == t)
					{
						ConstantClass constantClass = field.GetValue(null) as ConstantClass;
						dictionary[constantClass.Value] = constantClass;
					}
				}
				Dictionary<Type, Dictionary<string, ConstantClass>> dictionary2 = new Dictionary<Type, Dictionary<string, ConstantClass>>(staticFields);
				dictionary2[t] = dictionary;
				staticFields = dictionary2;
			}
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this == obj)
			{
				return true;
			}
			ConstantClass obj2 = obj as ConstantClass;
			if (Equals(obj2))
			{
				return true;
			}
			string text = obj as string;
			if (text != null)
			{
				return Equals(text);
			}
			return false;
		}

		public virtual bool Equals(ConstantClass obj)
		{
			if ((object)obj == null)
			{
				return false;
			}
			return StringComparer.OrdinalIgnoreCase.Equals(Value, obj.Value);
		}

		protected virtual bool Equals(string value)
		{
			return StringComparer.OrdinalIgnoreCase.Equals(Value, value);
		}

		public static bool operator ==(ConstantClass a, ConstantClass b)
		{
			if ((object)a == b)
			{
				return true;
			}
			if ((object)a == null)
			{
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(ConstantClass a, ConstantClass b)
		{
			return !(a == b);
		}

		public static bool operator ==(ConstantClass a, string b)
		{
			if ((object)a == null && b == null)
			{
				return true;
			}
			if ((object)a == null)
			{
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator ==(string a, ConstantClass b)
		{
			return b == a;
		}

		public static bool operator !=(ConstantClass a, string b)
		{
			return !(a == b);
		}

		public static bool operator !=(string a, ConstantClass b)
		{
			return !(a == b);
		}
	}
}
