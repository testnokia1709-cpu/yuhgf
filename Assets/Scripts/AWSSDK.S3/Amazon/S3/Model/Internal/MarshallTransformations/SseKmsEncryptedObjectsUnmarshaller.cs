using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class SseKmsEncryptedObjectsUnmarshaller : IUnmarshaller<SseKmsEncryptedObjects, XmlUnmarshallerContext>, IUnmarshaller<SseKmsEncryptedObjects, JsonUnmarshallerContext>
	{
		private static SseKmsEncryptedObjectsUnmarshaller _instance;

		public static SseKmsEncryptedObjectsUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new SseKmsEncryptedObjectsUnmarshaller();
				}
				return _instance;
			}
		}

		public SseKmsEncryptedObjects Unmarshall(XmlUnmarshallerContext context)
		{
			SseKmsEncryptedObjects sseKmsEncryptedObjects = new SseKmsEncryptedObjects();
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
					if (context.TestExpression("Status", num))
					{
						sseKmsEncryptedObjects.SseKmsEncryptedObjectsStatus = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return sseKmsEncryptedObjects;
				}
			}
			return sseKmsEncryptedObjects;
		}

		public SseKmsEncryptedObjects Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
