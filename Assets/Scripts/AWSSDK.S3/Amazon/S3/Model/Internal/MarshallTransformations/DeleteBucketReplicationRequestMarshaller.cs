using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketReplicationRequestMarshaller : IMarshaller<IRequest, DeleteBucketReplicationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketReplicationRequestMarshaller _instance;

		public static DeleteBucketReplicationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketReplicationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketReplicationRequest)input);
		}

		public IRequest Marshall(DeleteBucketReplicationRequest deleteBucketReplicationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteBucketReplicationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteBucketReplicationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("replication");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
