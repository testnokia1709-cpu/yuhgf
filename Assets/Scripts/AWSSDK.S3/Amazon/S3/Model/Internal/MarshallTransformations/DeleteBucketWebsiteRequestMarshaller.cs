using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketWebsiteRequestMarshaller : IMarshaller<IRequest, DeleteBucketWebsiteRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketWebsiteRequestMarshaller _instance;

		public static DeleteBucketWebsiteRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketWebsiteRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketWebsiteRequest)input);
		}

		public IRequest Marshall(DeleteBucketWebsiteRequest deleteBucketWebsiteRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketWebsiteRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketWebsiteRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("website");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
