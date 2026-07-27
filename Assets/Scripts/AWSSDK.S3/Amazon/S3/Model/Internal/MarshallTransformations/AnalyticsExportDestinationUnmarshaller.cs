using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AnalyticsExportDestinationUnmarshaller : IUnmarshaller<AnalyticsExportDestination, XmlUnmarshallerContext>, IUnmarshaller<AnalyticsExportDestination, JsonUnmarshallerContext>
	{
		private static AnalyticsExportDestinationUnmarshaller _instance;

		public static AnalyticsExportDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsExportDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public AnalyticsExportDestination Unmarshall(XmlUnmarshallerContext context)
		{
			AnalyticsExportDestination analyticsExportDestination = new AnalyticsExportDestination();
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
					if (context.TestExpression("S3BucketDestination", num))
					{
						analyticsExportDestination.S3BucketDestination = AnalyticsS3BucketDestinationUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return analyticsExportDestination;
				}
			}
			return analyticsExportDestination;
		}

		public AnalyticsExportDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
