using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetCORSConfigurationRequestMarshaller : IMarshaller<IRequest, GetCORSConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetCORSConfigurationRequestMarshaller _instance;

		public static GetCORSConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetCORSConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetCORSConfigurationRequest)input);
		}

		public IRequest Marshall(GetCORSConfigurationRequest getCORSConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(getCORSConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).Suppress404Exceptions = true;
			((IRequest)defaultRequest).HttpMethod = "GET";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(getCORSConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("cors");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
