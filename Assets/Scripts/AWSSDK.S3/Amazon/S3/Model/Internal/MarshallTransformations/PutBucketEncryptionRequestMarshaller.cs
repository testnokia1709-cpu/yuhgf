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
	public class PutBucketEncryptionRequestMarshaller : IMarshaller<IRequest, PutBucketEncryptionRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketEncryptionRequestMarshaller _instance;

		public static PutBucketEncryptionRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketEncryptionRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketEncryptionRequest)input);
		}

		public IRequest Marshall(PutBucketEncryptionRequest putBucketEncryptionRequest)
		{
			IRequest request = new DefaultRequest(putBucketEncryptionRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketEncryptionRequest.BucketName);
			request.AddSubResource("encryption");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				if (putBucketEncryptionRequest.IsSetServerSideEncryptionConfiguration())
				{
					ServerSideEncryptionConfiguration serverSideEncryptionConfiguration = putBucketEncryptionRequest.ServerSideEncryptionConfiguration;
					xmlWriter.WriteStartElement("ServerSideEncryptionConfiguration", "");
					if (serverSideEncryptionConfiguration != null)
					{
						foreach (ServerSideEncryptionRule serverSideEncryptionRule in serverSideEncryptionConfiguration.ServerSideEncryptionRules)
						{
							xmlWriter.WriteStartElement("Rule", "");
							if (serverSideEncryptionRule != null && serverSideEncryptionRule.IsSetServerSideEncryptionByDefault())
							{
								xmlWriter.WriteStartElement("ApplyServerSideEncryptionByDefault", "");
								ServerSideEncryptionByDefault serverSideEncryptionByDefault = serverSideEncryptionRule.ServerSideEncryptionByDefault;
								if (serverSideEncryptionByDefault.IsSetServerSideEncryptionAlgorithm())
								{
									xmlWriter.WriteElementString("SSEAlgorithm", "", S3Transforms.ToXmlStringValue(serverSideEncryptionByDefault.ServerSideEncryptionAlgorithm));
								}
								if (serverSideEncryptionByDefault.IsSetServerSideEncryptionKeyManagementServiceKeyId())
								{
									xmlWriter.WriteElementString("KMSMasterKeyID", "", S3Transforms.ToXmlStringValue(serverSideEncryptionByDefault.ServerSideEncryptionKeyManagementServiceKeyId));
								}
								xmlWriter.WriteEndElement();
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
				if (putBucketEncryptionRequest.IsSetContentMD5())
				{
					value = putBucketEncryptionRequest.ContentMD5;
				}
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
