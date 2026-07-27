using Amazon.Runtime;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketEncryptionResponseUnmarshaller : S3ReponseUnmarshaller
	{
		private static DeleteBucketEncryptionResponseUnmarshaller _instance;

		public static DeleteBucketEncryptionResponseUnmarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketEncryptionResponseUnmarshaller();
				}
				return _instance;
			}
		}

		public override AmazonWebServiceResponse Unmarshall(XmlUnmarshallerContext context)
		{
			return new DeleteBucketEncryptionResponse();
		}
	}
}
