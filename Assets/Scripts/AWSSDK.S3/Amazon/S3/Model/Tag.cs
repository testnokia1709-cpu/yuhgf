using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class Tag
	{
		private string key;

		private string value;

		public string Key
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public string Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		internal bool IsSetKey()
		{
			return key != null;
		}

		internal bool IsSetValue()
		{
			return value != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(memberName);
			xmlWriter.WriteElementString("Key", S3Transforms.ToXmlStringValue(key));
			xmlWriter.WriteElementString("Value", S3Transforms.ToXmlStringValue(key));
			xmlWriter.WriteEndElement();
		}
	}
}
