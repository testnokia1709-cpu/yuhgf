using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class InventoryScheduleUnmarshaller : IUnmarshaller<InventorySchedule, XmlUnmarshallerContext>, IUnmarshaller<InventorySchedule, JsonUnmarshallerContext>
	{
		private static InventoryScheduleUnmarshaller _instance;

		public static InventoryScheduleUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InventoryScheduleUnmarshaller();
				}
				return _instance;
			}
		}

		public InventorySchedule Unmarshall(XmlUnmarshallerContext context)
		{
			InventorySchedule inventorySchedule = new InventorySchedule();
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
					if (context.TestExpression("Frequency", num))
					{
						inventorySchedule.Frequency = InventoryFrequency.FindValue(StringUnmarshaller.GetInstance().Unmarshall(context));
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return inventorySchedule;
				}
			}
			return inventorySchedule;
		}

		public InventorySchedule Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
