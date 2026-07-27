using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class MetricsConfigurationUnmarshaller : IUnmarshaller<MetricsConfiguration, XmlUnmarshallerContext>, IUnmarshaller<MetricsConfiguration, JsonUnmarshallerContext>
	{
		private static MetricsConfigurationUnmarshaller _instance;

		public static MetricsConfigurationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new MetricsConfigurationUnmarshaller();
				}
				return _instance;
			}
		}

		public MetricsConfiguration Unmarshall(XmlUnmarshallerContext context)
		{
			MetricsConfiguration metricsConfiguration = new MetricsConfiguration();
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
					if (context.TestExpression("Filter", num))
					{
						metricsConfiguration.MetricsFilter = new MetricsFilter
						{
							MetricsFilterPredicate = MetricsPredicateListFilterUnmarshaller.Instance.Unmarshall(context)[0]
						};
					}
					else if (context.TestExpression("Id", num))
					{
						metricsConfiguration.MetricsId = StringUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return metricsConfiguration;
				}
			}
			return metricsConfiguration;
		}

		public MetricsConfiguration Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
