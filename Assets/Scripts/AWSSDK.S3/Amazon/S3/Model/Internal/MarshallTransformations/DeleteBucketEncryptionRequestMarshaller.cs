using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketEncryptionRequestMarshaller : IMarshaller<IRequest, DeleteBucketEncryptionRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketEncryptionRequestMarshaller _instance;

		public static DeleteBucketEncryptionRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketEncryptionRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketEncryptionRequest)input);
		}

		public IRequest Marshall(DeleteBucketEncryptionRequest deleteBucketEncryptionRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketEncryptionRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketEncryptionRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("encryption");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
