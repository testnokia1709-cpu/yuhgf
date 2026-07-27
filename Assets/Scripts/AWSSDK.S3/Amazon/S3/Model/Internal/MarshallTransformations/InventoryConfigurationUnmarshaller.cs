using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class InventoryConfigurationUnmarshaller : IUnmarshaller<InventoryConfiguration, XmlUnmarshallerContext>, IUnmarshaller<InventoryConfiguration, JsonUnmarshallerContext>
	{
		private static InventoryConfigurationUnmarshaller _instance;

		public static InventoryConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public InventoryConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			InventoryConfiguration inventoryConfiguration = new InventoryConfiguration();
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
					if (context.TestExpression("Destination", num))
					{
						inventoryConfiguration.Destination = InventoryDestinationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IsEnabled", num))
					{
						inventoryConfiguration.IsEnabled = BoolUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						inventoryConfiguration.InventoryFilter = InventoryFilterUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Id", num))
					{
						inventoryConfiguration.InventoryId = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IncludedObjectVersions", num))
					{
						inventoryConfiguration.IncludedObjectVersions = InventoryIncludedObjectVersions.FindValue(StringUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Field", num + 1))
					{
						inventoryConfiguration.InventoryOptionalFields.Add(InventoryOptionalField.FindValue(StringUnmarshaller.Instance.Unmarshall(context)));
					}
					else if (context.TestExpression("Schedule", num))
					{
						inventoryConfiguration.Schedule = InventoryScheduleUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return inventoryConfiguration;
				}
			}
			return inventoryConfiguration;
		}

		public InventoryConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
