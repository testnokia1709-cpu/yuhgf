using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketAnalyticsConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteBucketAnalyticsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketAnalyticsConfigurationRequestMarshaller _instance;

		public static DeleteBucketAnalyticsConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketAnalyticsConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketAnalyticsConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteBucketAnalyticsConfigurationRequest deleteBucketAnalyticsConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketAnalyticsConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketAnalyticsConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("analytics");
			((IRequest)defaultRequest).AddSubResource("id", deleteBucketAnalyticsConfigurationRequest.AnalyticsId);
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
