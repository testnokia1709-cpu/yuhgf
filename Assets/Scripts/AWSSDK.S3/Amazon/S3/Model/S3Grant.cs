using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class S3Grant
	{
		private S3Grantee grantee;

		private S3Permission permission;

		public S3Grantee Grantee
		{
			get
			{
				return grantee;
			}
			set
			{
				grantee = value;
			}
		}

		public S3Permission Permission
		{
			get
			{
				return permission;
			}
			set
			{
				permission = value;
			}
		}

		internal bool IsSetGrantee()
		{
			return grantee != null;
		}

		internal bool IsSetPermission()
		{
			return permission != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement(memberName);
			if (Grantee != null)
			{
				xmlWriter.WriteStartElement("Grantee");
				if (Grantee.IsSetType())
				{
					xmlWriter.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", Grantee.Type.ToString());
				}
				if (Grantee.IsSetDisplayName())
				{
					xmlWriter.WriteElementString("DisplayName", S3Transforms.ToXmlStringValue(Grantee.DisplayName));
				}
				if (Grantee.IsSetEmailAddress())
				{
					xmlWriter.WriteElementString("EmailAddress", S3Transforms.ToXmlStringValue(Grantee.EmailAddress));
				}
				if (Grantee.IsSetCanonicalUser())
				{
					xmlWriter.WriteElementString("ID", S3Transforms.ToXmlStringValue(Grantee.CanonicalUser));
				}
				if (Grantee.IsSetURI())
				{
					xmlWriter.WriteElementString("URI", S3Transforms.ToXmlStringValue(Grantee.URI));
				}
				xmlWriter.WriteEndElement();
			}
			if (IsSetPermission())
			{
				xmlWriter.WriteElementString("Permission", S3Transforms.ToXmlStringValue(Permission));
			}
			xmlWriter.WriteEndElement();
		}
	}
}
