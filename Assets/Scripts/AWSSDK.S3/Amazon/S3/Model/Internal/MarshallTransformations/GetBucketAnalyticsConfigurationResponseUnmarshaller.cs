using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketAnalyticsConfigurationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketAnalyticsConfigurationResponseUnmarshaller _instance;

		public static GetBucketAnalyticsConfigurationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketAnalyticsConfigurationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketAnalyticsConfigurationResponse getBucketAnalyticsConfigurationResponse = new GetBucketAnalyticsConfigurationResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketAnalyticsConfigurationResponse);
				}
			}
			return getBucketAnalyticsConfigurationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketAnalyticsConfigurationResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			response.AnalyticsConfiguration = new AnalyticsConfiguration();
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Id", num))
					{
						response.AnalyticsConfiguration.AnalyticsId = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						response.AnalyticsConfiguration.AnalyticsFilter = new AnalyticsFilter
						{
							AnalyticsFilterPredicate = AnalyticsPredicateListUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("StorageClassAnalysis", num))
					{
						response.AnalyticsConfiguration.StorageClassAnalysis = StorageClassAnalysisUnmarshaller.Instance.Unmarshall(context);
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
