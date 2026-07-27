using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketLocationRequestMarshaller : IMarshaller<IRequest, GetBucketLocationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketLocationRequestMarshaller _instance;

		public static GetBucketLocationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketLocationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketLocationRequest)input);
		}

		public IRequest Marshall(GetBucketLocationRequest getBucketLocationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketLocationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketLocationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("location");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
