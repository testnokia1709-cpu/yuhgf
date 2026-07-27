using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketMetricsConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteBucketMetricsConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketMetricsConfigurationRequestMarshaller _instance;

		public static DeleteBucketMetricsConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketMetricsConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketMetricsConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteBucketMetricsConfigurationRequest deleteBucketMetricsConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketMetricsConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketMetricsConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("metrics");
			((IRequest)defaultRequest).AddSubResource("id", deleteBucketMetricsConfigurationRequest.MetricsId);
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
