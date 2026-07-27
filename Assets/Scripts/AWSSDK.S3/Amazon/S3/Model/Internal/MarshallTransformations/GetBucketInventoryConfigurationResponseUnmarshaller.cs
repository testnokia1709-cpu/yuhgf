using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketInventoryConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketInventoryConfigurationResponseUnmarshaller _instance;

		public static GetBucketInventoryConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketInventoryConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketInventoryConfigurationResponse getBucketInventoryConfigurationResponse = new GetBucketInventoryConfigurationResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketInventoryConfigurationResponse);
				}
			}
			return getBucketInventoryConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketInventoryConfigurationResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			response.InventoryConfiguration = new InventoryConfiguration();
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Destination", num))
					{
						response.InventoryConfiguration.Destination = InventoryDestinationUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IsEnabled", num))
					{
						response.InventoryConfiguration.IsEnabled = BoolUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						response.InventoryConfiguration.InventoryFilter = InventoryFilterUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Id", num))
					{
						response.InventoryConfiguration.InventoryId = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("IncludedObjectVersions", num))
					{
						response.InventoryConfiguration.IncludedObjectVersions = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Field", num + 1))
					{
						response.InventoryConfiguration.InventoryOptionalFields.Add(StringUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("Schedule", num))
					{
						response.InventoryConfiguration.Schedule = InventoryScheduleUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					break;
				}
			}
		}
	}
}
