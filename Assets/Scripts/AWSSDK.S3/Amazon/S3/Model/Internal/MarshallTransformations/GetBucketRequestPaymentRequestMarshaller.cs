using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketRequestPaymentRequestMarshaller : IMarshaller<IRequest, GetBucketRequestPaymentRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketRequestPaymentRequestMarshaller _instance;

		public static GetBucketRequestPaymentRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketRequestPaymentRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketRequestPaymentRequest)input);
		}

		public IRequest Marshall(GetBucketRequestPaymentRequest getBucketRequestPaymentRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getBucketRequestPaymentRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getBucketRequestPaymentRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("requestPayment");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
