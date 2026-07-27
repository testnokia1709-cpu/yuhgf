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
	public class CompleteMultipartUploadRequestMarshaller : IMarshaller<IRequest, CompleteMultipartUploadRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static CompleteMultipartUploadRequestMarshaller _instance;

		public static CompleteMultipartUploadRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CompleteMultipartUploadRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CompleteMultipartUploadRequest)input);
		}

		public IRequest Marshall(CompleteMultipartUploadRequest completeMultipartUploadRequest)
		{
			IRequest request = new DefaultRequest(completeMultipartUploadRequest, "AmazonS3");
			request.HttpMethod = "POST";
			if (completeMultipartUploadRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(completeMultipartUploadRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(completeMultipartUploadRequest.BucketName), S3Transforms.ToStringValue(completeMultipartUploadRequest.Key));
			request.AddSubResource("uploadId", S3Transforms.ToStringValue(completeMultipartUploadRequest.UploadId));
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				xmlWriter.WriteStartElement("CompleteMultipartUpload", "");
				List<PartETag> partETags = completeMultipartUploadRequest.PartETags;
				partETags.Sort();
				if (partETags != null && partETags.Count > 0)
				{
					foreach (PartETag item in partETags)
					{
						xmlWriter.WriteStartElement("Part", "");
						if (item.IsSetETag())
						{
							xmlWriter.WriteElementString("ETag", "", S3Transforms.ToXmlStringValue(item.ETag));
						}
						if (item.IsSetPartNumber())
						{
							xmlWriter.WriteElementString("PartNumber", "", S3Transforms.ToXmlStringValue(item.PartNumber));
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
