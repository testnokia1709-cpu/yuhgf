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
	public class PutBucketLoggingRequestMarshaller : IMarshaller<IRequest, PutBucketLoggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketLoggingRequestMarshaller _instance;

		public static PutBucketLoggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketLoggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketLoggingRequest)input);
		}

		public IRequest Marshall(PutBucketLoggingRequest putBucketLoggingRequest)
		{
			IRequest request = new DefaultRequest(putBucketLoggingRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketLoggingRequest.BucketName);
			request.AddSubResource("logging");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("BucketLoggingStatus", "");
				S3BucketLoggingConfig loggingConfig = putBucketLoggingRequest.LoggingConfig;
				if (loggingConfig != null && loggingConfig != null)
				{
					S3BucketLoggingConfig s3BucketLoggingConfig = loggingConfig;
					if (s3BucketLoggingConfig != null && s3BucketLoggingConfig.IsSetTargetBucket())
					{
						xmlWriter.WriteStartElement("LoggingEnabled", "");
						xmlWriter.WriteElementString("TargetBucket", "", S3Transforms.ToXmlStringValue(s3BucketLoggingConfig.TargetBucketName));
						List<S3Grant> grants = s3BucketLoggingConfig.Grants;
						if (grants != null && grants.Count > 0)
						{
							xmlWriter.WriteStartElement("TargetGrants", "");
							foreach (S3Grant item in grants)
							{
								xmlWriter.WriteStartElement("Grant", "");
								if (item != null)
								{
									S3Grantee grantee = item.Grantee;
									if (grantee != null)
									{
										xmlWriter.WriteStartElement("Grantee", "");
										if (grantee.IsSetType())
										{
											xmlWriter.WriteAttributeString("xsi", "type", "http://www.w3.org/2001/XMLSchema-instance", grantee.Type.ToString());
										}
										if (grantee.IsSetDisplayName())
										{
											xmlWriter.WriteElementString("DisplayName", "", S3Transforms.ToXmlStringValue(grantee.DisplayName));
										}
										if (grantee.IsSetEmailAddress())
										{
											xmlWriter.WriteElementString("EmailAddress", "", S3Transforms.ToXmlStringValue(grantee.EmailAddress));
										}
										if (grantee.IsSetCanonicalUser())
										{
											xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(grantee.CanonicalUser));
										}
										if (grantee.IsSetURI())
										{
											xmlWriter.WriteElementString("URI", "", S3Transforms.ToXmlStringValue(grantee.URI));
										}
										xmlWriter.WriteEndElement();
									}
									if (item.IsSetPermission())
									{
										xmlWriter.WriteElementString("Permission", "", S3Transforms.ToXmlStringValue(item.Permission));
									}
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						if (s3BucketLoggingConfig.IsSetTargetPrefix())
						{
							xmlWriter.WriteElementString("TargetPrefix", "", S3Transforms.ToXmlStringValue(s3BucketLoggingConfig.TargetPrefix));
						}
						else
						{
							xmlWriter.WriteStartElement("TargetPrefix");
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
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
