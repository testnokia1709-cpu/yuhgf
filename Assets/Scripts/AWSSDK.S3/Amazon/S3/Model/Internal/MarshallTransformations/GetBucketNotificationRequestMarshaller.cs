using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketNotificationRequestMarshaller : IMarshaller<IRequest, GetBucketNotificationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketNotificationRequestMarshaller _instance;

		public static GetBucketNotificationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketNotificationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketNotificationRequest)input);
		}

		public IRequest Marshall(GetBucketNotificationRequest getBucketNotificationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketNotificationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketNotificationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("notification");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
