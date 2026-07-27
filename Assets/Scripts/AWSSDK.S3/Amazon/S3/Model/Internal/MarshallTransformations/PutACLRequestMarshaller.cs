using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutACLRequestMarshaller : IMarshaller<IRequest, PutACLRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutACLRequestMarshaller _instance;

		public static PutACLRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutACLRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutACLRequest)input);
		}

		public IRequest Marshall(PutACLRequest putObjectAclRequest)
		{
			IRequest request = new DefaultRequest(putObjectAclRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (putObjectAclRequest.IsSetCannedACL())
			{
				request.Headers.Add("x-amz-acl", S3Transforms.ToStringValue(putObjectAclRequest.CannedACL));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(putObjectAclRequest.BucketName), S3Transforms.ToStringValue(putObjectAclRequest.Key));
			request.AddSubResource("acl");
			if (putObjectAclRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(putObjectAclRequest.VersionId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				S3AccessControlList accessControlList = putObjectAclRequest.AccessControlList;
				if (accessControlList != null)
				{
					xmlWriter.WriteStartElement("AccessControlPolicy", "");
					List<S3Grant> grants = accessControlList.Grants;
					if (grants != null && grants.Count > 0)
					{
						accessControlList.Marshall("AccessControlList", xmlWriter);
						Owner owner = accessControlList.Owner;
						if (owner != null)
						{
							xmlWriter.WriteStartElement("Owner", "");
							if (owner.IsSetDisplayName())
							{
								xmlWriter.WriteElementString("DisplayName", "", S3Transforms.ToXmlStringValue(owner.DisplayName));
							}
							if (owner.IsSetId())
							{
								xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(owner.Id));
							}
							xmlWriter.WriteEndElement();
						}
					}
					xmlWriter.WriteEndElement();
				}
			}
			try
			{
				string text = stringWriter.ToString();
				request.Content = Encoding.UTF8.GetBytes(text);
				request.Headers["Content-Type"] = "application/xml";
				string value = AmazonS3Util.GenerateChecksumForContent(text, true);
				request.Headers["Content-MD5"] = value;
				return request;
			}
			catch (EncoderFallbackException innerException)
			{
				throw new AmazonServiceException("Unable to marshall request to XML", innerException);
			}
		}
	}
}
