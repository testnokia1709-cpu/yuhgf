using System;
using System.Collections.Generic;
using System.Reflection;

namespace CloudOnce.Internal
{
	public static class JsonHelper
	{
		public static T Convert<T>(JSONObject jsonObject)
		{
			return (T)Convert(jsonObject, typeof(T));
		}

		public static JSONObject ToJsonObject<T>(Dictionary<string, T> serializableDictionary) where T : IJsonSerializeable
		{
			Dictionary<string, IJsonSerializeable> dictionary = ConvertToSerializable(serializableDictionary);
			Dictionary<string, JSONObject> dictionary2 = new Dictionary<string, JSONObject>();
			foreach (KeyValuePair<string, IJsonSerializeable> item in dictionary)
			{
				dictionary2.Add(item.Key, item.Value.ToJSONObject());
			}
			return new JSONObject(dictionary2);
		}

		public static JSONObject ToJsonObject<T>(List<T> serializableList) where T : IJsonSerializeable
		{
			List<JSONObject> list = new List<JSONObject>();
			foreach (T serializable in serializableList)
			{
				list.Add(serializable.ToJSONObject());
			}
			return new JSONObject(list);
		}

		private static object Convert(JSONObject jsonObject, Type type)
		{
			if (type == typeof(Dictionary<string, float>))
			{
				return ToStringFloatDictionary(jsonObject);
			}
			if (type == typeof(Dictionary<string, SyncableItem>))
			{
				return ConstructDictionaryOfType<SyncableItem>(jsonObject);
			}
			if (type == typeof(Dictionary<string, SyncableCurrency>))
			{
				return ConstructDictionaryOfType<SyncableCurrency>(jsonObject);
			}
			if (type == typeof(Dictionary<string, CurrencyValue>))
			{
				return ConstructDictionaryOfType<CurrencyValue>(jsonObject);
			}
			return null;
		}

		private static Dictionary<string, IJsonSerializeable> ConvertToSerializable<T>(Dictionary<string, T> dictionary) where T : IJsonSerializeable
		{
			Dictionary<string, IJsonSerializeable> dictionary2 = new Dictionary<string, IJsonSerializeable>();
			foreach (KeyValuePair<string, T> item in dictionary)
			{
				dictionary2.Add(item.Key, item.Value);
			}
			return dictionary2;
		}

		private static Dictionary<string, float> ToStringFloatDictionary(JSONObject jObject)
		{
			Dictionary<string, float> dictionary = new Dictionary<string, float>();
			foreach (string key in jObject.Keys)
			{
				dictionary.Add(key, jObject[key].F);
			}
			return dictionary;
		}

		private static Dictionary<string, T> ConstructDictionaryOfType<T>(JSONObject jsonObject) where T : class
		{
			ConstructorInfo constructor = typeof(T).GetConstructor(new Type[1] { typeof(JSONObject) });
			if (constructor != null)
			{
				Dictionary<string, T> dictionary = new Dictionary<string, T>();
				{
					foreach (string key in jsonObject.Keys)
					{
						dictionary.Add(key, (T)constructor.Invoke(new object[1] { jsonObject[key] }));
					}
					return dictionary;
				}
			}
			return null;
		}
	}
}
