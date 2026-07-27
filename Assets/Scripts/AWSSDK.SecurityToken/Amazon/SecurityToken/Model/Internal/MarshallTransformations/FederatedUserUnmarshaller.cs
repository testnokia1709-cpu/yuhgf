using Amazon.Runtime.Internal.Transform;

namespace Amazon.SecurityToken.Model.Internal.MarshallTransformations
{
	public class FederatedUserUnmarshaller : IUnmarshaller<FederatedUser, XmlUnmarshallerContext>, IUnmarshaller<FederatedUser, JsonUnmarshallerContext>
	{
		private static FederatedUserUnmarshaller _instance = new FederatedUserUnmarshaller();

		public static FederatedUserUnmarshaller Instance
		{
			get
			{
				return _instance;
			}
		}

		public FederatedUser Unmarshall(XmlUnmarshallerContext context)
		{
			FederatedUser federatedUser = new FederatedUser();
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
						federatedUser.Arn = instance.Unmarshall(context);
					}
					else if (context.TestExpression("FederatedUserId", num))
					{
						StringUnmarshaller instance2 = StringUnmarshaller.Instance;
						federatedUser.FederatedUserId = instance2.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return federatedUser;
				}
			}
			return federatedUser;
		}

		public FederatedUser Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
