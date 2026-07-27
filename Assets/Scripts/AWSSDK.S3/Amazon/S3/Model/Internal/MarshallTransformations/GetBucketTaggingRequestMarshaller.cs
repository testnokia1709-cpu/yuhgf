using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketTaggingRequestMarshaller : IMarshaller<IRequest, GetBucketTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketTaggingRequestMarshaller _instance;

		public static GetBucketTaggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketTaggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketTaggingRequest)input);
		}

		public IRequest Marshall(GetBucketTaggingRequest getBucketTaggingRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketTaggingRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketTaggingRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("tagging");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
