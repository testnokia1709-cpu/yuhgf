using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketMetricsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketMetricsConfigurationResponseUnmarshaller _instance;

		public static GetBucketMetricsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketMetricsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketMetricsConfigurationResponse getBucketMetricsConfigurationResponse = new GetBucketMetricsConfigurationResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketMetricsConfigurationResponse);
				}
			}
			return getBucketMetricsConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketMetricsConfigurationResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			response.MetricsConfiguration = new MetricsConfiguration();
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Filter", num))
					{
						response.MetricsConfiguration.MetricsFilter = new MetricsFilter
						{
							MetricsFilterPredicate = MetricsPredicateListFilterUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("Id", num))
					{
						response.MetricsConfiguration.MetricsId = StringUnmarshaller.Instance.Unmarshall(context);
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
