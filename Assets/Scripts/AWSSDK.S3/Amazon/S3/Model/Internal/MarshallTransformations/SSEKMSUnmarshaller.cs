using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class SSEKMSUnmarshaller : IUnmarshaller<SSEKMS, XmlUnmarshallerContext>, IUnmarshaller<SSEKMS, JsonUnmarshallerContext>
	{
		private static SSEKMSUnmarshaller _instance;

		public static SSEKMSUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SSEKMSUnmarshaller();
				}
				return _instance;
			}
		}

		public SSEKMS Unmarshall(XmlUnmarshallerContext context)
		{
			SSEKMS sSEKMS = new SSEKMS();
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
					if (context.TestExpression("KeyId", num))
					{
						sSEKMS.KeyId = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return sSEKMS;
				}
			}
			return sSEKMS;
		}

		public SSEKMS Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
