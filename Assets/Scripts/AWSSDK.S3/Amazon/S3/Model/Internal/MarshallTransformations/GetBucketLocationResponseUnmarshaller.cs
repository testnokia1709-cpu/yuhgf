using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketLocationResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static GetBucketLocationResponseUnmarshaller _instance;

		public static GetBucketLocationResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketLocationResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			GetBucketLocationResponse getBucketLocationResponse = new GetBucketLocationResponse();
			UnmarshallResult(context, getBucketLocationResponse);
			return getBucketLocationResponse;
		}

		private static void UnmarshallResult(XmlUnmarshallerContext context, GetBucketLocationResponse response)
		{
			int currentDepth = context.CurrentDepth;
			int startingStackDepth = 1;
			while (context.Read())
			{
				if (context.IsStartElement || context.IsAttribute)
				{
					if (context.TestExpression("LocationConstraint", startingStackDepth))
					{
						response.Location = StringUnmarshaller.GetInstance().Unmarshall(context);
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
