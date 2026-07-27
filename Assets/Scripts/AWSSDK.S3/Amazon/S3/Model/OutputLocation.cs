using System.Xml;

namespace Amazon.S3.Model
{
	public class OutputLocation
	{
		public S3Location S3 { get; set; }

		internal bool IsSetS3()
		{
			return S3 != null;
		}

		internal void Marshall(string propertyName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(propertyName);
			S3.Marshall("S3", xmlWriter);
			xmlWriter.WriteEndElement();
		}
	}
}
