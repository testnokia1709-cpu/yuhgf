using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteCORSConfigurationRequestMarshaller : IMarshaller<IRequest, DeleteCORSConfigurationRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteCORSConfigurationRequestMarshaller _instance;

		public static DeleteCORSConfigurationRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteCORSConfigurationRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteCORSConfigurationRequest)input);
		}

		public IRequest Marshall(DeleteCORSConfigurationRequest deleteCORSConfigurationRequest)
		{
			DefaultRequest defaultRequest = new DefaultRequest(deleteCORSConfigurationRequest, "AmazonS3");
			((IRequest)defaultRequest).HttpMethod = "DELETE";
			((IRequest)defaultRequest).ResourcePath = "/" + S3Transforms.ToStringValue(deleteCORSConfigurationRequest.BucketName);
			((IRequest)defaultRequest).AddSubResource("cors");
			((IRequest)defaultRequest).UseQueryString = true;
			return defaultRequest;
		}
	}
}
