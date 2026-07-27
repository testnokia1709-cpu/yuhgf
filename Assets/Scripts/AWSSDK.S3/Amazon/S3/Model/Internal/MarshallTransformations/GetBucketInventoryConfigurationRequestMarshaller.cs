using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetBucketInventoryConfigurationRequestMarshaller : IMarshaller<IRequest, GetBucketInventoryConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetBucketInventoryConfigurationRequestMarshaller _instance;

		public static GetBucketInventoryConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetBucketInventoryConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetBucketInventoryConfigurationRequest)input);
		}

		public IRequest Marshall(GetBucketInventoryConfigurationRequest getInventoryConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getInventoryConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getInventoryConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("inventory");
			((IRequest)defaultRequest).AddSubResource("id", getInventoryConfigurationRequest.InventoryId);
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
