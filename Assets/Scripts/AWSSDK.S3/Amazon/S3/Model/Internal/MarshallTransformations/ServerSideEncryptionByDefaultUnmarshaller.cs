using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ServerSideEncryptionByDefaultUnmarshaller : IUnmarshaller<ServerSideEncryptionByDefault, XmlUnmarshallerContext>, IUnmarshaller<ServerSideEncryptionByDefault, JsonUnmarshallerContext>
	{
		private static ServerSideEncryptionByDefaultUnmarshaller _instance;

		public static ServerSideEncryptionByDefaultUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ServerSideEncryptionByDefaultUnmarshaller();
				}
				return _instance;
			}
		}

		public ServerSideEncryptionByDefault Unmarshall(XmlUnmarshallerContext context)
		{
			ServerSideEncryptionByDefault serverSideEncryptionByDefault = new ServerSideEncryptionByDefault();
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
					if (context.TestExpression("SSEAlgorithm", num))
					{
						serverSideEncryptionByDefault.ServerSideEncryptionAlgorithm = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("KMSMasterKeyID", num))
					{
						serverSideEncryptionByDefault.ServerSideEncryptionKeyManagementServiceKeyId = StringUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return serverSideEncryptionByDefault;
				}
			}
			return serverSideEncryptionByDefault;
		}

		public ServerSideEncryptionByDefault Unmarshall(JsonUnmarshallerContext input)
		{
			return null;
		}
	}
}
