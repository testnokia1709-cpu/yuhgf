using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketMetricsConfigurationRequestMarshaller : IMarshaller<IRequest, GetBucketMetricsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketMetricsConfigurationRequestMarshaller _instance;

		public static GetBucketMetricsConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketMetricsConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketMetricsConfigurationRequest)input);
		}

		public IRequest Marshall(GetBucketMetricsConfigurationRequest getBucketMetricsConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketMetricsConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketMetricsConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("metrics");
			((IRequest)defaultRequest).AddSubResource("id", getBucketMetricsConfigurationRequest.MetricsId);
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
