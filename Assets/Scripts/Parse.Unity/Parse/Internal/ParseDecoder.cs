using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class ParseDecoder
	{
		private static readonly ParseDecoder instance = new ParseDecoder();

		public static ParseDecoder Instance
		{
			get
			{
				return instance;
			}
		}

		private ParseDecoder()
		{
		}

		public object Decode(object data)
		{
			if (data == null)
			{
				return null;
			}
			IDictionary<string, object> dictionary = data as IDictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("__op"))
				{
					return ParseFieldOperations.Decode(dictionary);
				}
				object value;
				dictionary.TryGetValue("__type", out value);
				string text = value as string;
				if (text == null)
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					{
						foreach (KeyValuePair<string, object> item in dictionary)
						{
							dictionary2[item.Key] = Decode(item.Value);
						}
						return dictionary2;
					}
				}
				switch (text)
				{
				case "Date":
					return ParseDate(dictionary["iso"] as string);
				case "Bytes":
					return Convert.FromBase64String(dictionary["base64"] as string);
				case "Pointer":
					return DecodePointer(dictionary["className"] as string, dictionary["objectId"] as string);
				case "File":
					return new ParseFile(dictionary["name"] as string, new Uri(dictionary["url"] as string));
				case "GeoPoint":
					return new ParseGeoPoint((double)Conversion.ConvertTo<double>(dictionary["latitude"]), (double)Conversion.ConvertTo<double>(dictionary["longitude"]));
				case "Object":
					return ParseObject.FromState<ParseObject>(ParseObjectCoder.Instance.Decode(dictionary, this), dictionary["className"] as string);
				case "Relation":
					return ParseRelationBase.CreateRelation(null, null, dictionary["className"] as string);
				default:
				{
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					{
						foreach (KeyValuePair<string, object> item2 in dictionary)
						{
							dictionary3[item2.Key] = Decode(item2.Value);
						}
						return dictionary3;
					}
				}
				}
			}
			IList<object> list = data as IList<object>;
			if (list != null)
			{
				return list.Select((object item) => Decode(item)).ToList();
			}
			return data;
		}

		protected virtual object DecodePointer(string className, string objectId)
		{
			return ParseObject.CreateWithoutData(className, objectId);
		}

		internal static DateTime ParseDate(string input)
		{
			return DateTime.ParseExact(input, ParseClient.DateFormatStrings, CultureInfo.InvariantCulture, DateTimeStyles.None);
		}
	}
}
