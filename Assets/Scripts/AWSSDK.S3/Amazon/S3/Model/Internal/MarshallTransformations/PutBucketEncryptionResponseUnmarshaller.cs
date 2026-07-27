using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutBucketEncryptionResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static PutBucketEncryptionResponseUnmarshaller _instance;

		public static PutBucketEncryptionResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutBucketEncryptionResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new PutBucketEncryptionResponse();
		}
	}
}
