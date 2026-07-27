using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal class ParseObjectCoder
	{
		private static readonly ParseObjectCoder instance = new ParseObjectCoder();

		public static ParseObjectCoder Instance
		{
			get
			{
				return instance;
			}
		}

		private ParseObjectCoder()
		{
		}

		public IDictionary<string, object> Encode<T>(T state, IDictionary<string, IParseFieldOperation> operations, ParseEncoder encoder) where T : IObjectState
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, IParseFieldOperation> operation in operations)
			{
				IParseFieldOperation value = operation.Value;
				dictionary[operation.Key] = encoder.Encode(value);
			}
			return dictionary;
		}

		public IObjectState Decode(IDictionary<string, object> data, ParseDecoder decoder)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>(data);
			string objectId = extractFromDictionary(dictionary2, "objectId", (object obj) => obj as string);
			DateTime? dateTime = extractFromDictionary((IDictionary<string, object>)dictionary2, "createdAt", (Func<object, DateTime?>)((object obj) => ParseDecoder.ParseDate(obj as string)));
			DateTime? updatedAt = extractFromDictionary((IDictionary<string, object>)dictionary2, "updatedAt", (Func<object, DateTime?>)((object obj) => ParseDecoder.ParseDate(obj as string)));
			if (dictionary2.ContainsKey("ACL"))
			{
				dictionary["ACL"] = extractFromDictionary(dictionary2, "ACL", (object obj) => new ParseACL(obj as IDictionary<string, object>));
			}
			if (dateTime.HasValue && !updatedAt.HasValue)
			{
				updatedAt = dateTime;
			}
			foreach (KeyValuePair<string, object> item in dictionary2)
			{
				if (!(item.Key == "__type") && !(item.Key == "className"))
				{
					object value = item.Value;
					dictionary[item.Key] = decoder.Decode(value);
				}
			}
			return new MutableObjectState
			{
				ObjectId = objectId,
				CreatedAt = dateTime,
				UpdatedAt = updatedAt,
				ServerData = dictionary
			};
		}

		private T extractFromDictionary<T>(IDictionary<string, object> data, string key, Func<object, T> action)
		{
			T result = default(T);
			if (data.ContainsKey(key))
			{
				result = action(data[key]);
				data.Remove(key);
			}
			return result;
		}
	}
}
