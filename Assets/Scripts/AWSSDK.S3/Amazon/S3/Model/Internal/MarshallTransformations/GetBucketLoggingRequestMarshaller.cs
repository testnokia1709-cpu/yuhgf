using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketLoggingRequestMarshaller : IMarshaller<IRequest, GetBucketLoggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketLoggingRequestMarshaller _instance;

		public static GetBucketLoggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketLoggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketLoggingRequest)input);
		}

		public IRequest Marshall(GetBucketLoggingRequest getBucketLoggingRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketLoggingRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketLoggingRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("logging");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
