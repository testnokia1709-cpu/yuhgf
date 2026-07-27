using System;
using System.Collections.Generic;
using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public sealed class MetadataCollection
	{
		internal const string MetaDataHeaderPrefix = "x-amz-meta-";

		private IDictionary<string, string> values = new Dictionary<string, string>();

		public string this[string name]
		{
			get
			{
				if (!name.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
				{
					name = "x-amz-meta-" + name;
				}
				string value;
				if (values.TryGetValue(name, out value))
				{
					return value;
				}
				return null;
			}
			set
			{
				if (!name.StartsWith("x-amz-meta-", StringComparison.OrdinalIgnoreCase))
				{
					name = "x-amz-meta-" + name;
				}
				values[name] = value;
			}
		}

		public int Count
		{
			get
			{
				return values.Count;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return values.Keys;
			}
		}

		public void Add(string name, string value)
		{
			this[name] = value;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(memberName);
			foreach (KeyValuePair<string, string> value2 in values)
			{
				xmlWriter.WriteStartElement("MetadataEntry");
				string value = value2.Key.Replace("x-amz-meta-", "");
				xmlWriter.WriteElementString("Name", S3Transforms.ToXmlStringValue(value));
				xmlWriter.WriteElementString("Value", S3Transforms.ToXmlStringValue(value2.Value));
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}
	}
}
