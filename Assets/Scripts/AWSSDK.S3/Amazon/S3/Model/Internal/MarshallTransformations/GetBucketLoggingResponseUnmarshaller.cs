using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketLoggingResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketLoggingResponseUnmarshaller _instance;

		public static GetBucketLoggingResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketLoggingResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketLoggingResponse getBucketLoggingResponse = new GetBucketLoggingResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketLoggingResponse);
				}
			}
			return getBucketLoggingResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketLoggingResponse response)
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
					if (context.TestExpression("LoggingEnabled", num))
					{
						response.BucketLoggingConfig = LoggingEnabledUnmarshaller.Instance.Unmarshall(context);
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
