using System;
using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class S3Location
	{
		public string BucketName { get; set; }

		public string Prefix { get; set; }

		public S3Encryption Encryption { get; set; }

		public S3CannedACL CannedACL { get; set; }

		public S3AccessControlList AccessControlList { get; set; }

		public Tagging Tagging { get; set; }

		public MetadataCollection UserMetadata { get; set; }

		public S3StorageClass StorageClass { get; set; }

		internal bool IsSetBucketName()
		{
			return BucketName != null;
		}

		internal bool IsSetPrefix()
		{
			return Prefix != null;
		}

		internal bool IsSetEncryption()
		{
			return Encryption != null;
		}

		internal bool IsSetCannedACL()
		{
			return CannedACL != null;
		}

		internal bool IsSetAccessControlList()
		{
			return AccessControlList != null;
		}

		internal bool IsSetTagging()
		{
			return Tagging != null;
		}

		internal bool IsSetUserMetadata()
		{
			return UserMetadata != null;
		}

		internal bool IsSetStorageClass()
		{
			return StorageClass != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			if (string.IsNullOrEmpty(BucketName))
			{
				throw new ArgumentException("BucketName is a required property and must be set before making this call.", "S3Location.BucketName");
			}
			if (string.IsNullOrEmpty(Prefix))
			{
				throw new ArgumentException("Prefix is a required property and must be set before making this call.", "S3Location.Prefix");
			}
			xmlWriter.WriteStartElement(memberName);
			xmlWriter.WriteElementString("BucketName", S3Transforms.ToXmlStringValue(BucketName));
			xmlWriter.WriteElementString("Prefix", S3Transforms.ToXmlStringValue(Prefix));
			if (IsSetEncryption())
			{
				Encryption.Marshall("Encryption", xmlWriter);
			}
			if (IsSetCannedACL())
			{
				xmlWriter.WriteElementString("CannedACL", S3Transforms.ToXmlStringValue(CannedACL.Value));
			}
			if (IsSetAccessControlList())
			{
				AccessControlList.Marshall("AccessControlList", xmlWriter);
			}
			if (IsSetTagging())
			{
				Tagging.Marshall("Tagging", xmlWriter);
			}
			if (IsSetUserMetadata())
			{
				UserMetadata.Marshall("UserMetadata", xmlWriter);
			}
			if (IsSetStorageClass())
			{
				xmlWriter.WriteElementString("StorageClass", S3Transforms.ToXmlStringValue(StorageClass.Value));
			}
			xmlWriter.WriteEndElement();
		}
	}
}
