using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class CurrencyValue : IJsonConvertible, IJsonSerializeable, IJsonDeserializable
	{
		private const string c_oldAliasAdditions = "cdAdd";

		private const string c_oldAliasSubtractions = "cdSub";

		private const string c_aliasAdditions = "a";

		private const string c_aliasSubtractions = "s";

		public float Additions { get; set; }

		public float Subtractions { get; set; }

		public float Value
		{
			get
			{
				return Additions + Subtractions;
			}
			set
			{
				float num = value - Value;
				if (num > 0f)
				{
					Additions += num;
				}
				else
				{
					Subtractions += num;
				}
			}
		}

		public CurrencyValue()
		{
		}

		public CurrencyValue(float additions, float subtractions)
		{
			Additions = additions;
			Subtractions = subtractions;
		}

		public CurrencyValue(float value)
		{
			Value = value;
		}

		public CurrencyValue(JSONObject jsonObject)
		{
			FromJSONObject(jsonObject);
		}

		public JSONObject ToJSONObject()
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.Object);
			jSONObject.AddField("a", Additions);
			jSONObject.AddField("s", Subtractions);
			return jSONObject;
		}

		public void FromJSONObject(JSONObject jsonObject)
		{
			string alias = CloudOnceUtils.GetAlias(typeof(CurrencyValue).Name, jsonObject, "a", "cdAdd");
			string alias2 = CloudOnceUtils.GetAlias(typeof(CurrencyValue).Name, jsonObject, "s", "cdSub");
			Additions = jsonObject[alias].F;
			Subtractions = jsonObject[alias2].F;
		}
	}
}
