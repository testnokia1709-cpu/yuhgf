using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class InventoryFilterUnmarshaller : IUnmarshaller<InventoryFilter, XmlUnmarshallerContext>, IUnmarshaller<InventoryFilter, JsonUnmarshallerContext>
	{
		private static InventoryFilterUnmarshaller _instance;

		public static InventoryFilterUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryFilterUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryFilter Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryFilter inventoryFilter = new InventoryFilter();
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
					if (context.TestExpression("Prefix", num))
					{
						inventoryFilter.InventoryFilterPredicate = new InventoryPrefixPredicate(StringUnmarshaller.Instance.Unmarshall(context));
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return inventoryFilter;
				}
			}
			return inventoryFilter;
		}

		public InventoryFilter Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
