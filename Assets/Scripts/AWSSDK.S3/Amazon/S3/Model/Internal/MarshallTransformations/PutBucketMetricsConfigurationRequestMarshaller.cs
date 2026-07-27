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
	public class PutBucketMetricsConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketMetricsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketMetricsConfigurationRequestMarshaller _instance;

		public static PutBucketMetricsConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketMetricsConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketMetricsConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketMetricsConfigurationRequest PutBucketMetricsConfigurationRequest)
		{
			IRequest request = new DefaultRequest(PutBucketMetricsConfigurationRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(PutBucketMetricsConfigurationRequest.BucketName);
			request.AddSubResource("metrics");
			request.AddSubResource("id", PutBucketMetricsConfigurationRequest.MetricsId);
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				MetricsConfiguration metricsConfiguration = PutBucketMetricsConfigurationRequest.MetricsConfiguration;
				if (metricsConfiguration != null)
				{
					xmlWriter.WriteStartElement("MetricsConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
					if (metricsConfiguration != null)
					{
						if (metricsConfiguration.IsSetMetricsId())
						{
							xmlWriter.WriteElementString("Id", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(metricsConfiguration.MetricsId));
						}
						if (metricsConfiguration.IsSetMetricsFilter())
						{
							xmlWriter.WriteStartElement("Filter", "http://s3.amazonaws.com/doc/2006-03-01/");
							metricsConfiguration.MetricsFilter.MetricsFilterPredicate.Accept(new MetricsPredicateVisitor(xmlWriter));
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
