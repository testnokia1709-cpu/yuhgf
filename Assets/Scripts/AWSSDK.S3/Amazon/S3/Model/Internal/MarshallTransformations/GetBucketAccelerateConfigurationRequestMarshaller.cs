using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketAccelerateConfigurationRequestMarshaller : IMarshaller<IRequest, GetBucketAccelerateConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketAccelerateConfigurationRequestMarshaller _instance;

		public static GetBucketAccelerateConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketAccelerateConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketAccelerateConfigurationRequest)input);
		}

		public IRequest Marshall(GetBucketAccelerateConfigurationRequest getBucketAccelerateRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketAccelerateRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketAccelerateRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("accelerate");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
