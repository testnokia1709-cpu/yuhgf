using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class AssumedRoleUserUnmarshaller : IUnmarshaller<AssumedRoleUser, XmlUnmarshallerContext>, IUnmarshaller<AssumedRoleUser, JsonUnmarshallerContext>
	{
		private static AssumedRoleUserUnmarshaller _instance = new AssumedRoleUserUnmarshaller();

		public static AssumedRoleUserUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public AssumedRoleUser Unmarshall(XmlUnmarshallerContext context)
		{
			AssumedRoleUser assumedRoleUser = new AssumedRoleUser();
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			while (context.ReadAtDepth(currentDepth))
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Arn", num))
					{
						StringUnmarshaller instance = StringUnmarshaller.Instance;
						assumedRoleUser.Arn = instance.Unmarshall(context);
					}
					else if (context.TestExpression("AssumedRoleId", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.Instance;
						assumedRoleUser.AssumedRoleId = instance2.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return assumedRoleUser;
				}
			}
			return assumedRoleUser;
		}

		public AssumedRoleUser Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
