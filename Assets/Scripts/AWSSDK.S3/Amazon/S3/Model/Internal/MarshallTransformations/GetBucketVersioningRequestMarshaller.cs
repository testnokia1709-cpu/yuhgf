using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketVersioningRequestMarshaller : IMarshaller<IRequest, GetBucketVersioningRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketVersioningRequestMarshaller _instance;

		public static GetBucketVersioningRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketVersioningRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketVersioningRequest)input);
		}

		public IRequest Marshall(GetBucketVersioningRequest getBucketVersioningRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketVersioningRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketVersioningRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("versioning");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
