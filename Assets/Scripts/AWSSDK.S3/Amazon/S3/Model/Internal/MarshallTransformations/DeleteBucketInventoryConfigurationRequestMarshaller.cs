using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteBucketInventoryConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteBucketInventoryConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteBucketInventoryConfigurationRequestMarshaller _instance;

		public static DeleteBucketInventoryConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteBucketInventoryConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteBucketInventoryConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteBucketInventoryConfigurationRequest deleteInventoryConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteInventoryConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteInventoryConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("inventory");
			((IRequest)defaultRequest).AddSubResource("id", deleteInventoryConfigurationRequest.InventoryId);
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
