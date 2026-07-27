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
	public class PutBucketRequestPaymentRequestMarshaller : IMarshaller<IRequest, PutBucketRequestPaymentRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketRequestPaymentRequestMarshaller _instance;

		public static PutBucketRequestPaymentRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketRequestPaymentRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketRequestPaymentRequest)input);
		}

		public IRequest Marshall(PutBucketRequestPaymentRequest putBucketRequestPaymentRequest)
		{
			IRequest request = new DefaultRequest(putBucketRequestPaymentRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketRequestPaymentRequest.BucketName);
			request.AddSubResource("requestPayment");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				RequestPaymentConfiguration requestPaymentConfiguration = putBucketRequestPaymentRequest.RequestPaymentConfiguration;
				if (requestPaymentConfiguration != null)
				{
					xmlWriter.WriteStartElement("RequestPaymentConfiguration", "");
					if (requestPaymentConfiguration.IsSetPayer())
					{
						xmlWriter.WriteElementString("Payer", "", S3Transforms.ToXmlStringValue(requestPaymentConfiguration.Payer));
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
