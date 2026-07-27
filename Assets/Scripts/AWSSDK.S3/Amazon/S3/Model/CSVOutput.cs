using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class CSVOutput
	{
		public QuoteFields QuoteFields { get; set; }

		public string QuoteEscapeCharacter { get; set; }

		public string RecordDelimiter { get; set; }

		public string FieldDelimiter { get; set; }

		public string QuoteCharacter { get; set; }

		internal bool IsSetQuoteFields()
		{
			return QuoteFields != null;
		}

		internal bool IsSetQuoteEscapeCharacter()
		{
			return QuoteEscapeCharacter != null;
		}

		internal bool IsSetRecordDelimiter()
		{
			return RecordDelimiter != null;
		}

		internal bool IsSetFieldDelimiter()
		{
			return FieldDelimiter != null;
		}

		internal bool IsSetQuoteCharacter()
		{
			return QuoteCharacter != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(memberName);
			if (IsSetQuoteFields())
			{
				xmlWriter.WriteElementString("QuoteFields", S3Transforms.ToXmlStringValue(QuoteFields.Value));
			}
			if (IsSetQuoteEscapeCharacter())
			{
				xmlWriter.WriteElementString("QuoteEscapeCharacter", S3Transforms.ToXmlStringValue(QuoteEscapeCharacter));
			}
			if (IsSetRecordDelimiter())
			{
				xmlWriter.WriteElementString("RecordDelimiter", S3Transforms.ToXmlStringValue(RecordDelimiter));
			}
			if (IsSetFieldDelimiter())
			{
				xmlWriter.WriteElementString("FieldDelimiter", S3Transforms.ToXmlStringValue(FieldDelimiter));
			}
			if (IsSetQuoteCharacter())
			{
				xmlWriter.WriteElementString("QuoteCharacter", S3Transforms.ToXmlStringValue(QuoteCharacter));
			}
			xmlWriter.WriteEndElement();
		}
	}
}
