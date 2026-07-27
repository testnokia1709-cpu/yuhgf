using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketEncryptionResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketEncryptionResponseUnmarshaller _instance;

		public static GetBucketEncryptionResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketEncryptionResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketEncryptionResponse getBucketEncryptionResponse = new GetBucketEncryptionResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketEncryptionResponse);
				}
			}
			return getBucketEncryptionResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketEncryptionResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int num = currentDepth + 1;
			if (context.IsStartOfDocument)
			{
				num += 2;
			}
			response.ServerSideEncryptionConfiguration = new ServerSideEncryptionConfiguration();
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("Rule", num))
					{
						response.ServerSideEncryptionConfiguration.ServerSideEncryptionRules.Add(ServerSideEncryptionRuleUnmarshaller.Instance.Unmarshall(context));
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
