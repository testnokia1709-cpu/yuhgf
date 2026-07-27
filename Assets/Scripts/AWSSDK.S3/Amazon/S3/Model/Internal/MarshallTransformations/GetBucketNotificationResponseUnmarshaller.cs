using System.Collections.Generic;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketNotificationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketNotificationResponseUnmarshaller _instance;

		public static GetBucketNotificationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketNotificationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketNotificationResponse getBucketNotificationResponse = new GetBucketNotificationResponse();
			getBucketNotificationResponse.TopicConfigurations = new List<TopicConfiguration>();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketNotificationResponse);
				}
			}
			return getBucketNotificationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketNotificationResponse response)
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
					if (context.TestExpression("TopicConfiguration", num))
					{
						response.TopicConfigurations.Add(TopicConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("QueueConfiguration", num))
					{
						response.QueueConfigurations.Add(QueueConfigurationUnmarshaller.Instance.Unmarshall(context));
					}
					else if (context.TestExpression("CloudFunctionConfiguration", num))
					{
						response.LambdaFunctionConfigurations.Add(LambdaFunctionConfigurationUnmarshaller.Instance.Unmarshall(context));
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
