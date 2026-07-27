using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketVersioningResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketVersioningResponseUnmarshaller _instance;

		public static GetBucketVersioningResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketVersioningResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketVersioningResponse getBucketVersioningResponse = new GetBucketVersioningResponse();
			while (context.Read())
			{
				if (context.IsStartElement)
				{
					UnmarshallResult(context, getBucketVersioningResponse);
				}
			}
			return getBucketVersioningResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketVersioningResponse response)
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
					if (context.TestExpression("Status", num))
					{
						response.VersioningConfig.Status = StringUnmarshaller.GetInstance().Unmarshall(context);
					}
					else if (context.TestExpression("MfaDelete", num))
					{
						response.VersioningConfig.EnableMfaDelete = string.Equals(StringUnmarshaller.GetInstance().Unmarshall(context), "Enabled");
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
