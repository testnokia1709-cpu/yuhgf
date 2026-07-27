using System;
using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class S3Encryption
	{
		public ServerSideEncryptionMethod EncryptionType { get; set; }

		public string KMSKeyId { get; set; }

		public string KMSContext { get; set; }

		internal bool IsSetEncryptionType()
		{
			return EncryptionType != null;
		}

		internal bool IsSetKMSKeyId()
		{
			return KMSKeyId != null;
		}

		internal bool IsSetKMSContext()
		{
			return KMSContext != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			if (!IsSetEncryptionType())
			{
				throw new ArgumentException("EncryptionType is a required property and must be set before making this call.", "S3Encryption.EncryptionType");
			}
			xmlWriter.WriteStartElement(memberName);
			xmlWriter.WriteElementString("EncryptionType", S3Transforms.ToXmlStringValue(EncryptionType.Value));
			if (IsSetKMSKeyId())
			{
				xmlWriter.WriteElementString("KMSKeyId", S3Transforms.ToXmlStringValue(KMSKeyId));
			}
			if (IsSetKMSContext())
			{
				xmlWriter.WriteElementString("KMSContext", S3Transforms.ToXmlStringValue(KMSContext));
			}
			xmlWriter.WriteEndElement();
		}
	}
}
