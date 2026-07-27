using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketMetricsConfigurationsRequestMarshaller : IMarshaller<IRequest, ListBucketMetricsConfigurationsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListBucketMetricsConfigurationsRequestMarshaller _instance;

		public static ListBucketMetricsConfigurationsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketMetricsConfigurationsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketMetricsConfigurationsRequest)input);
		}

		public IRequest Marshall(ListBucketMetricsConfigurationsRequest listBucketMetricsConfigurationRequest)
		{
			IRequest request = new DefaultRequest(listBucketMetricsConfigurationRequest, "AmazonS3");
			request.HttpMethod = "GET";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(listBucketMetricsConfigurationRequest.BucketName);
			request.AddSubResource("metrics");
			if (listBucketMetricsConfigurationRequest.IsSetContinuationToken())
			{
				request.AddSubResource("continuation-token", listBucketMetricsConfigurationRequest.ContinuationToken.ToString());
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
