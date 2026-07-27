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
	public class PutBucketReplicationRequestMarshaller : IMarshaller<IRequest, PutBucketReplicationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutBucketReplicationRequestMarshaller _instance;

		public static PutBucketReplicationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketReplicationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutBucketReplicationRequest)input);
		}

		public IRequest Marshall(PutBucketReplicationRequest putBucketreplicationRequest)
		{
			IRequest request = new DefaultRequest(putBucketreplicationRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(putBucketreplicationRequest.BucketName);
			request.AddSubResource("replication");
			StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
			{
				Encoding = Encoding.UTF8,
				OmitXmlDeclaration = true
			}))
			{
				ReplicationConfiguration configuration = putBucketreplicationRequest.Configuration;
				if (configuration != null)
				{
					xmlWriter.WriteStartElement("ReplicationConfiguration", "");
					if (configuration.Role != null)
					{
						xmlWriter.WriteElementString("Role", "", S3Transforms.ToXmlStringValue(configuration.Role));
					}
					if (configuration.Rules != null)
					{
						foreach (ReplicationRule rule in configuration.Rules)
						{
							xmlWriter.WriteStartElement("Rule");
							if (rule.IsSetId())
							{
								xmlWriter.WriteElementString("ID", "", S3Transforms.ToXmlStringValue(rule.Id));
							}
							if (rule.IsSetPrefix())
							{
								xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(rule.Prefix));
							}
							else
							{
								xmlWriter.WriteElementString("Prefix", "", S3Transforms.ToXmlStringValue(""));
							}
							if (rule.IsSetStatus())
							{
								xmlWriter.WriteElementString("Status", "", S3Transforms.ToXmlStringValue(rule.Status.ToString()));
							}
							if (rule.IsSetSourceSelectionCriteria())
							{
								xmlWriter.WriteStartElement("SourceSelectionCriteria");
								if (rule.SourceSelectionCriteria.IsSetSseKmsEncryptedObjects())
								{
									xmlWriter.WriteStartElement("SseKmsEncryptedObjects");
									if (rule.SourceSelectionCriteria.SseKmsEncryptedObjects.IsSetSseKmsEncryptedObjectsStatus())
									{
										xmlWriter.WriteElementString("Status", "", rule.SourceSelectionCriteria.SseKmsEncryptedObjects.SseKmsEncryptedObjectsStatus);
									}
									xmlWriter.WriteEndElement();
								}
								xmlWriter.WriteEndElement();
							}
							if (rule.IsSetDestination())
							{
								xmlWriter.WriteStartElement("Destination", "");
								if (rule.Destination.IsSetBucketArn())
								{
									xmlWriter.WriteElementString("Bucket", "", rule.Destination.BucketArn);
								}
								if (rule.Destination.IsSetStorageClass())
								{
									xmlWriter.WriteElementString("StorageClass", "", rule.Destination.StorageClass);
								}
								if (rule.Destination.IsSetAccountId())
								{
									xmlWriter.WriteElementString("Account", "", S3Transforms.ToXmlStringValue(rule.Destination.AccountId));
								}
								if (rule.Destination.IsSetEncryptionConfiguration())
								{
									xmlWriter.WriteStartElement("EncryptionConfiguration");
									if (rule.Destination.EncryptionConfiguration.isSetReplicaKmsKeyID())
									{
										xmlWriter.WriteElementString("ReplicaKmsKeyID", "", S3Transforms.ToXmlStringValue(rule.Destination.EncryptionConfiguration.ReplicaKmsKeyID));
									}
									xmlWriter.WriteEndElement();
								}
								if (rule.Destination.IsSetAccessControlTranslation())
								{
									xmlWriter.WriteStartElement("AccessControlTranslation");
									if (rule.Destination.AccessControlTranslation.IsSetOwner())
									{
										xmlWriter.WriteElementString("Owner", "", S3Transforms.ToXmlStringValue(rule.Destination.AccessControlTranslation.Owner));
									}
									xmlWriter.WriteEndElement();
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
