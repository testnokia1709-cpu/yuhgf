using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketAnalyticsConfigurationsRequestMarshaller : IMarshaller<IRequest, ListBucketAnalyticsConfigurationsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListBucketAnalyticsConfigurationsRequestMarshaller _instance;

		public static ListBucketAnalyticsConfigurationsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketAnalyticsConfigurationsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketAnalyticsConfigurationsRequest)input);
		}

		public IRequest Marshall(ListBucketAnalyticsConfigurationsRequest listBucketAnalyticsConfigurationsRequest)
		{
			IRequest request = new DefaultRequest(listBucketAnalyticsConfigurationsRequest, "AmazonS3");
			request.HttpMethod = "GET";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(listBucketAnalyticsConfigurationsRequest.BucketName);
			request.AddSubResource("analytics");
			if (listBucketAnalyticsConfigurationsRequest.IsSetContinuationToken())
			{
				request.AddSubResource("continuation-token", listBucketAnalyticsConfigurationsRequest.ContinuationToken.ToString());
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
