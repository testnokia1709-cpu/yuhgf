using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ReplicationDestinationUnmarshaller : IUnmarshaller<ReplicationDestination, XmlUnmarshallerContext>, IUnmarshaller<ReplicationDestination, JsonUnmarshallerContext>
	{
		private static ReplicationDestinationUnmarshaller _instance;

		public static ReplicationDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ReplicationDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public ReplicationDestination Unmarshall(XmlUnmarshallerContext context)
		{
			ReplicationDestination replicationDestination = new ReplicationDestination();
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Bucket", num))
					{
						replicationDestination.BucketArn = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("StorageClass", num))
					{
						replicationDestination.StorageClass = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("Account", num))
					{
						replicationDestination.AccountId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("AccessControlTranslation", num))
					{
						replicationDestination.AccessControlTranslation = AccessControlTranslationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("EncryptionConfiguration", num))
					{
						replicationDestination.EncryptionConfiguration = EncryptionConfigurationUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return replicationDestination;
				}
			}
			return replicationDestination;
		}

		public ReplicationDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
