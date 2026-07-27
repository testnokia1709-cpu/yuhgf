using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketWebsiteRequestMarshaller : IMarshaller<IRequest, GetBucketWebsiteRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketWebsiteRequestMarshaller _instance;

		public static GetBucketWebsiteRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketWebsiteRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketWebsiteRequest)input);
		}

		public IRequest Marshall(GetBucketWebsiteRequest getBucketWebsiteRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketWebsiteRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketWebsiteRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("website");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
