using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketPolicyRequestMarshaller : IMarshaller<IRequest, DeleteBucketPolicyRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketPolicyRequestMarshaller _instance;

		public static DeleteBucketPolicyRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketPolicyRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketPolicyRequest)input);
		}

		public IRequest Marshall(DeleteBucketPolicyRequest deleteBucketPolicyRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketPolicyRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketPolicyRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("policy");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
