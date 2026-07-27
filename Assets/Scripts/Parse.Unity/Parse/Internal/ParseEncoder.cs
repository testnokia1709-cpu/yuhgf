using System;
using System.Collections.Generic;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal abstract class ParseEncoder
	{
		public static bool IsValidType(object value)
		{
			if (value != null && !ReflectionHelpers.IsPrimitive(value.GetType()) && !(value is string) && !(value is ParseObject) && !(value is ParseACL) && !(value is ParseFile) && !(value is ParseGeoPoint) && !(value is ParseRelationBase) && !(value is DateTime) && !(value is byte[]) && !(Conversion.ConvertTo<IDictionary<string, object>>(value) is IDictionary<string, object>))
			{
				return Conversion.ConvertTo<IList<object>>(value) is IList<object>;
			}
			return true;
		}

		public object Encode(object value)
		{
			if (value is DateTime)
			{
				return new Dictionary<string, object>
				{
					{
						"iso",
						((DateTime)value).ToString(ParseClient.DateFormatStrings.First())
					},
					{ "__type", "Date" }
				};
			}
			byte[] array = value as byte[];
			if (array != null)
			{
				return new Dictionary<string, object>
				{
					{ "__type", "Bytes" },
					{
						"base64",
						Convert.ToBase64String(array)
					}
				};
			}
			ParseObject parseObject = value as ParseObject;
			if (parseObject != null)
			{
				return EncodeParseObject(parseObject);
			}
			IJsonConvertible jsonConvertible = value as IJsonConvertible;
			if (jsonConvertible != null)
			{
				return jsonConvertible.ToJSON();
			}
			IDictionary<string, object> dictionary = Conversion.ConvertTo<IDictionary<string, object>>(value) as IDictionary<string, object>;
			if (dictionary != null)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				{
					foreach (KeyValuePair<string, object> item in dictionary)
					{
						dictionary2[item.Key] = Encode(item.Value);
					}
					return dictionary2;
				}
			}
			IList<object> list = Conversion.ConvertTo<IList<object>>(value) as IList<object>;
			if (list != null)
			{
				return EncodeList(list);
			}
			IParseFieldOperation parseFieldOperation = value as IParseFieldOperation;
			if (parseFieldOperation != null)
			{
				return parseFieldOperation.Encode();
			}
			return value;
		}

		protected abstract IDictionary<string, object> EncodeParseObject(ParseObject value);

		private object EncodeList(IList<object> list)
		{
			List<object> list2 = new List<object>();
			if (PlatformHooks.IsCompiledByIL2CPP && list.GetType().IsArray)
			{
				list = new List<object>(list);
			}
			foreach (object item in list)
			{
				if (!IsValidType(item))
				{
					throw new ArgumentException("Invalid type for value in an array");
				}
				list2.Add(Encode(item));
			}
			return list2;
		}
	}
}
