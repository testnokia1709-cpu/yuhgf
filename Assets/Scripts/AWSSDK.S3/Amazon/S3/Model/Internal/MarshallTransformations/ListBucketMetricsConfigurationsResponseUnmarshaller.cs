using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketMetricsConfigurationsResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static ListBucketMetricsConfigurationsResponseUnmarshaller _instance = new ListBucketMetricsConfigurationsResponseUnmarshaller();

		public static ListBucketMetricsConfigurationsResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketMetricsConfigurationsResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			ListBucketMetricsConfigurationsResponse listBucketMetricsConfigurationsResponse = new ListBucketMetricsConfigurationsResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, listBucketMetricsConfigurationsResponse);
				}
			}
			return listBucketMetricsConfigurationsResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, ListBucketMetricsConfigurationsResponse response)
		{
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
					if (context.TestExpression("ContinuationToken", num))
					{
						response.Token = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("InventoryConfiguration", num))
					{
						response.MetricsConfigurationList.Add(MetricsConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("IsTruncated", num))
					{
						response.IsTruncated = BoolUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("NextContinuationToken", num))
					{
						response.NextToken = StringUnmarshaller.Instance.Unmarshall(context);
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
