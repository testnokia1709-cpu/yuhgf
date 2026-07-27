using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketVersioningResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketVersioningResponseUnmarshaller _instance;

		public static PutBucketVersioningResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketVersioningResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketVersioningResponse();
		}
	}
}
