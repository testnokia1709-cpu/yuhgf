using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketEncryptionRequestMarshaller : IMarshaller<IRequest, GetBucketEncryptionRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketEncryptionRequestMarshaller _instance;

		public static GetBucketEncryptionRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketEncryptionRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketEncryptionRequest)input);
		}

		public IRequest Marshall(GetBucketEncryptionRequest getEncryptionRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getEncryptionRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getEncryptionRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("encryption");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
