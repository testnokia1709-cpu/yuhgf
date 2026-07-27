using System;
using System.Globalization;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class SyncableItem : IEquatable<SyncableItem>, IJsonConvertible, IJsonSerializeable, IJsonDeserializable
	{
		private const string c_oldAliasValueString = "_vs";

		private const string c_oldAliasMetadata = "_md";

		private const string c_aliasValueString = "v";

		private const string c_aliasMetadata = "m";

		private string valueString;

		public SyncableItemMetaData Metadata { get; private set; }

		public string ValueString
		{
			get
			{
				return valueString ?? (valueString = string.Empty);
			}
			set
			{
				valueString = value;
				if (Metadata.PersistenceType == PersistenceType.Latest)
				{
					Metadata.UpdateDateTime();
				}
			}
		}

		public SyncableItem(JSONObject itemData)
		{
			FromJSONObject(itemData);
		}

		public SyncableItem(string value, SyncableItemMetaData metadata)
		{
			valueString = value;
			Metadata = metadata;
		}

		public bool Equals(SyncableItem other)
		{
			if (other == null)
			{
				return false;
			}
			return string.Equals(valueString, other.valueString) && Metadata.Equals(other.Metadata);
		}

		public JSONObject ToJSONObject()
		{
			JSONObject jSONObject = new JSONObject(JSONObject.Type.Object);
			jSONObject.AddField("v", ValueString.ToString(CultureInfo.InvariantCulture));
			jSONObject.AddField("m", Metadata.ToJSONObject());
			return jSONObject;
		}

		public void FromJSONObject(JSONObject jsonObject)
		{
			string alias = CloudOnceUtils.GetAlias(typeof(SyncableItem).Name, jsonObject, "v", "_vs");
			string alias2 = CloudOnceUtils.GetAlias(typeof(SyncableItem).Name, jsonObject, "m", "_md");
			valueString = jsonObject[alias].String;
			Metadata = new SyncableItemMetaData(jsonObject[alias2]);
		}

		public override string ToString()
		{
			return string.Format("Value: {0}" + Environment.NewLine + " Meta Data: {1}", ValueString, Metadata);
		}
	}
}
