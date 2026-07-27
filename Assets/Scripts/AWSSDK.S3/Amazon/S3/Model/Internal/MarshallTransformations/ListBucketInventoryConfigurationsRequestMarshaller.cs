using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class ListBucketInventoryConfigurationsRequestMarshaller : IMarshaller<IRequest, ListBucketInventoryConfigurationsRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static ListBucketInventoryConfigurationsRequestMarshaller _instance;

		public static ListBucketInventoryConfigurationsRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ListBucketInventoryConfigurationsRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((ListBucketInventoryConfigurationsRequest)input);
		}

		public IRequest Marshall(ListBucketInventoryConfigurationsRequest listBucketInventoryConfigurationsRequest)
		{
			IRequest request = new DefaultRequest(listBucketInventoryConfigurationsRequest, "AmazonS3");
			request.HttpMethod = "GET";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(listBucketInventoryConfigurationsRequest.BucketName);
			request.AddSubResource("inventory");
			if (listBucketInventoryConfigurationsRequest.IsSetContinuationToken())
			{
				request.AddSubResource("continuation-token", listBucketInventoryConfigurationsRequest.ContinuationToken.ToString());
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
