using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class InventoryEncryptionUnmarshaller : IUnmarshaller<InventoryEncryption, XmlUnmarshallerContext>, IUnmarshaller<InventoryEncryption, JsonUnmarshallerContext>
	{
		private static InventoryEncryptionUnmarshaller _instance;

		public static InventoryEncryptionUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryEncryptionUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryEncryption Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryEncryption inventoryEncryption = new InventoryEncryption();
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
					if (context.TestExpression("SSE-KMS", num))
					{
						inventoryEncryption.SSEKMS = SSEKMSUnmarshaller.Instance.Unmarshall(context);
					}
					else if (!context.TestExpression("SSE-S3", num))
					{
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return inventoryEncryption;
				}
			}
			return inventoryEncryption;
		}

		public InventoryEncryption Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
