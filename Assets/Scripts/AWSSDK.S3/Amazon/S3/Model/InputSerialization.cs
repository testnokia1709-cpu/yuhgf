using System.Xml;

namespace Amazon.S3.Model
{
	public class InputSerialization
	{
		public CSVInput CSV { get; set; }

		internal bool IsSetCSV()
		{
			return CSV != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(memberName);
			CSV.Marshall("CSV", xmlWriter);
			xmlWriter.WriteEndElement();
		}
	}
}
