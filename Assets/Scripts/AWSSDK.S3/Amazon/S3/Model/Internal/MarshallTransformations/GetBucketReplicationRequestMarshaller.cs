using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketReplicationRequestMarshaller : IMarshaller<IRequest, GetBucketReplicationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketReplicationRequestMarshaller _instance;

		public static GetBucketReplicationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketReplicationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketReplicationRequest)input);
		}

		public IRequest Marshall(GetBucketReplicationRequest getBucketReplicationConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketReplicationConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketReplicationConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("replication");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
