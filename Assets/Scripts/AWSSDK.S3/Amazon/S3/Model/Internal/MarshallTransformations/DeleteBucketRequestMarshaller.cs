using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketRequestMarshaller : IMarshaller<IRequest, DeleteBucketRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketRequestMarshaller _instance;

		public static DeleteBucketRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketRequest)input);
		}

		public IRequest Marshall(DeleteBucketRequest deleteBucketRequest)
		{
			IRequest request = new DefaultRequest(deleteBucketRequest, "AmazonS3");
			request.HttpMethod = "DELETE";
			request.ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketRequest.BucketName);
			if (deleteBucketRequest.BucketRegion != null)
			{
				request.AlternateEndpoint = RegionEndpoint.GetBySystemName(deleteBucketRequest.BucketRegion.Value);
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
