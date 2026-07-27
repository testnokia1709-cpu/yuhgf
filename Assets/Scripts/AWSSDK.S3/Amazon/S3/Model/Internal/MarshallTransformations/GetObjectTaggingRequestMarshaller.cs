using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectTaggingRequestMarshaller : IMarshaller<IRequest, GetObjectTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetObjectTaggingRequestMarshaller _instance;

		public static GetObjectTaggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectTaggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectTaggingRequest)input);
		}

		public IRequest Marshall(GetObjectTaggingRequest getObjectTaggingRequest)
		{
			IRequest request = new DefaultRequest(getObjectTaggingRequest, "AmazonS3");
			request.HttpMethod = "GET";
			request.UseQueryString = true;
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectTaggingRequest.BucketName), S3Transforms.ToStringValue(getObjectTaggingRequest.Key));
			request.AddSubResource("tagging");
			if (getObjectTaggingRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", getObjectTaggingRequest.VersionId);
			}
			return request;
		}
	}
}
