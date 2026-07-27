using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AnalyticsConfigurationUnmarshaller : IUnmarshaller<AnalyticsConfiguration, XmlUnmarshallerContext>, IUnmarshaller<AnalyticsConfiguration, JsonUnmarshallerContext>
	{
		private static AnalyticsConfigurationUnmarshaller _instance;

		public static AnalyticsConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public AnalyticsConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			AnalyticsConfiguration analyticsConfiguration = new AnalyticsConfiguration();
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
					if (context.TestExpression("Id", num))
					{
						analyticsConfiguration.AnalyticsId = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Filter", num))
					{
						analyticsConfiguration.AnalyticsFilter = new AnalyticsFilter
						{
							AnalyticsFilterPredicate = AnalyticsPredicateListUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("StorageClassAnalysis", num))
					{
						analyticsConfiguration.StorageClassAnalysis = StorageClassAnalysisUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return analyticsConfiguration;
				}
			}
			return analyticsConfiguration;
		}

		public AnalyticsConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
