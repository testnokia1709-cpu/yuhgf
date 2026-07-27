using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AnalyticsS3BucketDestinationUnmarshaller : IUnmarshaller<AnalyticsS3BucketDestination, XmlUnmarshallerContext>, IUnmarshaller<AnalyticsS3BucketDestination, JsonUnmarshallerContext>
	{
		private static AnalyticsS3BucketDestinationUnmarshaller _instance;

		public static AnalyticsS3BucketDestinationUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AnalyticsS3BucketDestinationUnmarshaller();
				}
				return _instance;
			}
		}

		public AnalyticsS3BucketDestination Unmarshall(XmlUnmarshallerContext context)
		{
			AnalyticsS3BucketDestination analyticsS3BucketDestination = new AnalyticsS3BucketDestination();
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
					if (context.TestExpression("Format", num))
					{
						analyticsS3BucketDestination.Format = AnalyticsS3ExportFileFormat.FindValue(StringUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("BucketAccountId", num))
					{
						analyticsS3BucketDestination.BucketAccountId = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Bucket", num))
					{
						analyticsS3BucketDestination.BucketName = StringUnmarshaller.Instance.Unmarshall(context);
					}
					else if (context.TestExpression("Prefix", num))
					{
						analyticsS3BucketDestination.Prefix = StringUnmarshaller.Instance.Unmarshall(context);
					}
				}
				else if (context.IsEndElement && context.CurrentDepth < currentDepth)
				{
					return analyticsS3BucketDestination;
				}
			}
			return analyticsS3BucketDestination;
		}

		public AnalyticsS3BucketDestination Unmarshall(JsonUnmarshallerContext context)
		{
			return null;
		}
	}
}
