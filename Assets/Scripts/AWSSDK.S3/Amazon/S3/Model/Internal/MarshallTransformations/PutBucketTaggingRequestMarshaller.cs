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
	public class PutBucketTaggingRequestMarshaller : IMarshaller<IRequest, PutBucketTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketTaggingRequestMarshaller _instance;

		public static PutBucketTaggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketTaggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketTaggingRequest)input);
		}

		public IRequest Marshall(PutBucketTaggingRequest putBucketTaggingRequest)
		{
			IRequest request = new DefaultRequest(putBucketTaggingRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketTaggingRequest.BucketName);
			request.AddSubResource("tagging");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("Tagging", "");
				List<Tag> tagSet = putBucketTaggingRequest.TagSet;
				if (tagSet != null && tagSet.Count > 0)
				{
					xmlWriter.WriteStartElement("TagSet", "");
					foreach (Tag item in tagSet)
					{
						xmlWriter.WriteStartElement("Tag", "");
						if (item.IsSetKey())
						{
							xmlWriter.WriteElementString("Key", "", S3Transforms.ToXmlStringValue(item.Key));
						}
						if (item.IsSetValue())
						{
							xmlWriter.WriteElementString("Value", "", S3Transforms.ToXmlStringValue(item.Value));
						}
						xmlWriter.WriteEndElement();
					}
					xmlWriter.WriteEndElement();
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
