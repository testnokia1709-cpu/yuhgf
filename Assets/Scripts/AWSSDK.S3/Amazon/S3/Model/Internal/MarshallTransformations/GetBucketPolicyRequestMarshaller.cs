using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketPolicyRequestMarshaller : IMarshaller<IRequest, GetBucketPolicyRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketPolicyRequestMarshaller _instance;

		public static GetBucketPolicyRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketPolicyRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketPolicyRequest)input);
		}

		public IRequest Marshall(GetBucketPolicyRequest getBucketPolicyRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketPolicyRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketPolicyRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("policy");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
