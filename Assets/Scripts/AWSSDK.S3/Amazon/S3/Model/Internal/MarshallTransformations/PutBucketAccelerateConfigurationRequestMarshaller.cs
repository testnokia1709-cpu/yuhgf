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
	public class PutBucketAccelerateConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketAccelerateConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketAccelerateConfigurationRequestMarshaller _instance;

		public static PutBucketAccelerateConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketAccelerateConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketAccelerateConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketAccelerateConfigurationRequest putBucketAccelerateRequest)
		{
			IRequest request = new DefaultRequest(putBucketAccelerateRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketAccelerateRequest.BucketName);
			request.AddSubResource("accelerate");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				AccelerateConfiguration accelerateConfiguration = putBucketAccelerateRequest.AccelerateConfiguration;
				if (accelerateConfiguration != null)
				{
					xmlWriter.WriteStartElement("AccelerateConfiguration", "");
					BucketAccelerateStatus status = accelerateConfiguration.Status;
					if (accelerateConfiguration.IsSetBucketAccelerateStatus() && status != null)
					{
						xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(accelerateConfiguration.Status));
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
