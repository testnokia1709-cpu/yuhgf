using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ServerSideEncryptionRuleUnmarshaller : IUnmarshaller<ServerSideEncryptionRule, XmlUnmarshallerContext>, IUnmarshaller<ServerSideEncryptionRule, JsonUnmarshallerContext>
	{
		private static ServerSideEncryptionRuleUnmarshaller _instance;

		public static ServerSideEncryptionRuleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ServerSideEncryptionRuleUnmarshaller();
				}
				return _instance;
			}
		}

		public ServerSideEncryptionRule Unmarshall(XmlUnmarshallerContext context)
		{
			ServerSideEncryptionRule serverSideEncryptionRule = new ServerSideEncryptionRule();
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
					if (context.TestExpression("ApplyServerSideEncryptionByDefault", num))
					{
						serverSideEncryptionRule.ServerSideEncryptionByDefault = ServerSideEncryptionByDefaultUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return serverSideEncryptionRule;
				}
			}
			return serverSideEncryptionRule;
		}

		public ServerSideEncryptionRule Unmarshall(JsonUnmarshallerContext input)
		{
			return null;
		}
	}
}
