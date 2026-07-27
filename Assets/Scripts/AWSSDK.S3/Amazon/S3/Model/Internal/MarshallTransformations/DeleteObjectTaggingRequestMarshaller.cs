using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteObjectTaggingRequestMarshaller : IMarshaller<IRequest, DeleteObjectTaggingRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteObjectTaggingRequestMarshaller _instance;

		public static DeleteObjectTaggingRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteObjectTaggingRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteObjectTaggingRequest)input);
		}

		public IRequest Marshall(DeleteObjectTaggingRequest deleteObjectTaggingRequest)
		{
			IRequest request = new DefaultRequest(deleteObjectTaggingRequest, "AmazonS3");
			request.HttpMethod = "DELETE";
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(deleteObjectTaggingRequest.BucketName), S3Transforms.ToStringValue(deleteObjectTaggingRequest.Key));
			request.AddSubResource("tagging");
			if (deleteObjectTaggingRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(deleteObjectTaggingRequest.VersionId));
			}
			return request;
		}
	}
}
