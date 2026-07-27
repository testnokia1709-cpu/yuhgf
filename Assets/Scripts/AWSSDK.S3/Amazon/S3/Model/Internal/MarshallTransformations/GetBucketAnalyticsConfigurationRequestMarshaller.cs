using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketAnalyticsConfigurationRequestMarshaller : IMarshaller<IRequest, GetBucketAnalyticsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketAnalyticsConfigurationRequestMarshaller _instance;

		public static GetBucketAnalyticsConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketAnalyticsConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketAnalyticsConfigurationRequest)input);
		}

		public IRequest Marshall(GetBucketAnalyticsConfigurationRequest getAnalyticsConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getAnalyticsConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getAnalyticsConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("analytics");
			((IRequest)defaultRequest).AddSubResource("id", getAnalyticsConfigurationRequest.AnalyticsId);
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
