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
	public class PutBucketAnalyticsConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketAnalyticsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketAnalyticsConfigurationRequestMarshaller _instance;

		public static PutBucketAnalyticsConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketAnalyticsConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketAnalyticsConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketAnalyticsConfigurationRequest putBucketAnalyticsConfigurationRequest)
		{
			IRequest request = new DefaultRequest(putBucketAnalyticsConfigurationRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketAnalyticsConfigurationRequest.BucketName);
			request.AddSubResource("analytics");
			if (putBucketAnalyticsConfigurationRequest.IsSetAnalyticsId())
			{
				request.AddSubResource("id", S3Transforms.ToStringValue(putBucketAnalyticsConfigurationRequest.AnalyticsId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				if (putBucketAnalyticsConfigurationRequest.IsSetAnalyticsConfiguration())
				{
					AnalyticsConfiguration analyticsConfiguration = putBucketAnalyticsConfigurationRequest.AnalyticsConfiguration;
					xmlWriter.WriteStartElement("AnalyticsConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
					if (analyticsConfiguration.IsSetAnalyticsId())
					{
						xmlWriter.WriteElementString("Id", "http://s3.amazonaws.com/doc/2006-03-01/", analyticsConfiguration.AnalyticsId);
					}
					if (analyticsConfiguration.IsSetAnalyticsFilter())
					{
						xmlWriter.WriteStartElement("Filter", "http://s3.amazonaws.com/doc/2006-03-01/");
						analyticsConfiguration.AnalyticsFilter.AnalyticsFilterPredicate.Accept(new AnalyticsPredicateVisitor(xmlWriter));
						xmlWriter.WriteEndElement();
					}
					if (analyticsConfiguration.IsSetStorageClassAnalysis() && analyticsConfiguration.IsSetStorageClassAnalysis())
					{
						StorageClassAnalysis storageClassAnalysis = analyticsConfiguration.StorageClassAnalysis;
						xmlWriter.WriteStartElement("StorageClassAnalysis", "http://s3.amazonaws.com/doc/2006-03-01/");
						if (storageClassAnalysis.IsSetDataExport())
						{
							xmlWriter.WriteStartElement("DataExport", "http://s3.amazonaws.com/doc/2006-03-01/");
							StorageClassAnalysisDataExport dataExport = storageClassAnalysis.DataExport;
							if (dataExport.IsSetOutputSchemaVersion())
							{
								StorageClassAnalysisSchemaVersion outputSchemaVersion = dataExport.OutputSchemaVersion;
								if (outputSchemaVersion != null)
								{
									xmlWriter.WriteElementString("OutputSchemaVersion", "http://s3.amazonaws.com/doc/2006-03-01/", outputSchemaVersion);
								}
							}
							if (dataExport.IsSetDestination())
							{
								xmlWriter.WriteStartElement("Destination", "http://s3.amazonaws.com/doc/2006-03-01/");
								AnalyticsExportDestination destination = dataExport.Destination;
								if (destination.IsSetS3BucketDestination())
								{
									xmlWriter.WriteStartElement("S3BucketDestination", "http://s3.amazonaws.com/doc/2006-03-01/");
									AnalyticsS3BucketDestination s3BucketDestination = destination.S3BucketDestination;
									if (s3BucketDestination.IsSetFormat())
									{
										xmlWriter.WriteElementString("Format", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.Format);
									}
									if (s3BucketDestination.IsSetBucketAccountId())
									{
										xmlWriter.WriteElementString("BucketAccountId", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.BucketAccountId);
									}
									if (s3BucketDestination.IsSetBucketName())
									{
										xmlWriter.WriteElementString("Bucket", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.BucketName);
									}
									if (s3BucketDestination.IsSetPrefix())
									{
										xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", s3BucketDestination.Prefix);
									}
									xmlWriter.WriteEndElement();
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteEndElement();
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
