using System.Xml;

namespace Amazon.S3.Model
{
	public class OutputSerialization
	{
		public CSVOutput CSV { get; set; }

		internal bool IsSetCSV()
		{
			return CSV != null;
		}

		internal void Marshall(string propertyName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(propertyName);
			CSV.Marshall("CSV", xmlWriter);
			xmlWriter.WriteEndElement();
		}
	}
}
