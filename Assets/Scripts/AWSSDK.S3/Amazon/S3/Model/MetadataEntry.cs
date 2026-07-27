using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class MetadataEntry
	{
		public string Name { get; set; }

		public string Value { get; set; }

		internal bool IsSetName()
		{
			return Name != null;
		}

		internal bool IsSetValue()
		{
			return Value != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(memberName);
			if (IsSetName())
			{
				xmlWriter.WriteElementString("Name", S3Transforms.ToXmlStringValue(Name));
			}
			if (IsSetValue())
			{
				xmlWriter.WriteElementString("Value", S3Transforms.ToXmlStringValue(Value));
			}
			xmlWriter.WriteEndElement();
		}
	}
}
