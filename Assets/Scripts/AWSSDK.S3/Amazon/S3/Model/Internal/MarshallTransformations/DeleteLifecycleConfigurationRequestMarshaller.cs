using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteLifecycleConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteLifecycleConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteLifecycleConfigurationRequestMarshaller _instance;

		public static DeleteLifecycleConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteLifecycleConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteLifecycleConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteLifecycleConfigurationRequest deleteLifecycleConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteLifecycleConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteLifecycleConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("lifecycle");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
