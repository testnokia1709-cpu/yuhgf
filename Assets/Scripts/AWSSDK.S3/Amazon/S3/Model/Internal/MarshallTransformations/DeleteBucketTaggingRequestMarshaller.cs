using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketTaggingRequestMarshaller : IMarshaller<IRequest, DeleteBucketTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketTaggingRequestMarshaller _instance;

		public static DeleteBucketTaggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketTaggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketTaggingRequest)input);
		}

		public IRequest Marshall(DeleteBucketTaggingRequest deleteBucketTaggingRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketTaggingRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketTaggingRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("tagging");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
