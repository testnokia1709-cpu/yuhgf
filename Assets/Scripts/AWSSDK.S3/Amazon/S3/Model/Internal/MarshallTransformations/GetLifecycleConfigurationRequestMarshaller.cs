using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetLifecycleConfigurationRequestMarshaller : IMarshaller<IRequest, GetLifecycleConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetLifecycleConfigurationRequestMarshaller _instance;

		public static GetLifecycleConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetLifecycleConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetLifecycleConfigurationRequest)input);
		}

		public IRequest Marshall(GetLifecycleConfigurationRequest getLifecycleConfiguration)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getLifecycleConfiguration, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getLifecycleConfiguration.BucketName);
			((IRequest)defaultRequest).AddSubResource("lifecycle");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
