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
	public class PutBucketInventoryConfigurationRequestMarshaller : IMarshaller<IRequest, PutBucketInventoryConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketInventoryConfigurationRequestMarshaller _instance;

		public static PutBucketInventoryConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketInventoryConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketInventoryConfigurationRequest)input);
		}

		public IRequest Marshall(PutBucketInventoryConfigurationRequest putBucketInventoryConfigurationRequest)
		{
			IRequest request = new DefaultRequest(putBucketInventoryConfigurationRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketInventoryConfigurationRequest.BucketName);
			request.AddSubResource("inventory");
			if (putBucketInventoryConfigurationRequest.IsSetInventoryId())
			{
				request.AddSubResource("id", S3Transforms.ToStringValue(putBucketInventoryConfigurationRequest.InventoryId));
			}
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				if (putBucketInventoryConfigurationRequest.IsSetInventoryConfiguration())
				{
					InventoryConfiguration inventoryConfiguration = putBucketInventoryConfigurationRequest.InventoryConfiguration;
					xmlWriter.WriteStartElement("InventoryConfiguration", "http://s3.amazonaws.com/doc/2006-03-01/");
					if (inventoryConfiguration != null)
					{
						if (inventoryConfiguration.IsSetDestination())
						{
							InventoryDestination destination = inventoryConfiguration.Destination;
							xmlWriter.WriteStartElement("Destination", "http://s3.amazonaws.com/doc/2006-03-01/");
							if (destination.isSetS3BucketDestination())
							{
								InventoryS3BucketDestination s3BucketDestination = destination.S3BucketDestination;
								xmlWriter.WriteStartElement("S3BucketDestination", "http://s3.amazonaws.com/doc/2006-03-01/");
								if (s3BucketDestination.IsSetAccountId())
								{
									xmlWriter.WriteElementString("AccountId", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.AccountId));
								}
								if (s3BucketDestination.IsSetBucketName())
								{
									xmlWriter.WriteElementString("Bucket", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.BucketName));
								}
								if (s3BucketDestination.IsSetInventoryFormat())
								{
									xmlWriter.WriteElementString("Format", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.InventoryFormat));
								}
								if (s3BucketDestination.IsSetPrefix())
								{
									xmlWriter.WriteElementString("Prefix", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(s3BucketDestination.Prefix));
								}
								if (s3BucketDestination.IsSetInventoryEncryption())
								{
									xmlWriter.WriteStartElement("Encryption", "http://s3.amazonaws.com/doc/2006-03-01/");
									InventoryEncryption inventoryEncryption = s3BucketDestination.InventoryEncryption;
									if (inventoryEncryption.IsSetSSEKMS())
									{
										xmlWriter.WriteStartElement("SSE-KMS", "http://s3.amazonaws.com/doc/2006-03-01/");
										if (inventoryEncryption.SSEKMS.IsSetKeyId())
										{
											xmlWriter.WriteElementString("KeyId", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(inventoryEncryption.SSEKMS.KeyId));
										}
										xmlWriter.WriteEndElement();
									}
									if (inventoryEncryption.IsSetSSES3())
									{
										xmlWriter.WriteStartElement("SSE-S3", "http://s3.amazonaws.com/doc/2006-03-01/");
										xmlWriter.WriteEndElement();
									}
									xmlWriter.WriteEndElement();
								}
								xmlWriter.WriteEndElement();
							}
							xmlWriter.WriteEndElement();
						}
						xmlWriter.WriteElementString("IsEnabled", "http://s3.amazonaws.com/doc/2006-03-01/", inventoryConfiguration.IsEnabled.ToString().ToLowerInvariant());
						if (inventoryConfiguration.IsSetInventoryFilter())
						{
							xmlWriter.WriteStartElement("Filter", "http://s3.amazonaws.com/doc/2006-03-01/");
							inventoryConfiguration.InventoryFilter.InventoryFilterPredicate.Accept(new InventoryPredicateVisitor(xmlWriter));
							xmlWriter.WriteEndElement();
						}
						if (inventoryConfiguration.IsSetInventoryId())
						{
							xmlWriter.WriteElementString("Id", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(inventoryConfiguration.InventoryId));
						}
						if (inventoryConfiguration.IsSetIncludedObjectVersions())
						{
							xmlWriter.WriteElementString("IncludedObjectVersions", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(inventoryConfiguration.IncludedObjectVersions));
						}
						if (inventoryConfiguration.IsSetInventoryOptionalFields())
						{
							xmlWriter.WriteStartElement("OptionalFields", "http://s3.amazonaws.com/doc/2006-03-01/");
							foreach (InventoryOptionalField inventoryOptionalField in inventoryConfiguration.InventoryOptionalFields)
							{
								xmlWriter.WriteElementString("Field", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(inventoryOptionalField));
							}
							xmlWriter.WriteEndElement();
						}
						if (inventoryConfiguration.IsSetSchedule())
						{
							xmlWriter.WriteStartElement("Schedule", "http://s3.amazonaws.com/doc/2006-03-01/");
							InventorySchedule schedule = inventoryConfiguration.Schedule;
							if (schedule.IsFrequency())
							{
								xmlWriter.WriteElementString("Frequency", "http://s3.amazonaws.com/doc/2006-03-01/", S3Transforms.ToXmlStringValue(schedule.Frequency));
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
