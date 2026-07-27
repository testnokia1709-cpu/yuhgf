using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class DeleteObjectRequestMarshaller : IMarshaller<IRequest, DeleteObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static DeleteObjectRequestMarshaller _instance;

		public static DeleteObjectRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new DeleteObjectRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((DeleteObjectRequest)input);
		}

		public IRequest Marshall(DeleteObjectRequest deleteObjectRequest)
		{
			IRequest request = new DefaultRequest(deleteObjectRequest, "AmazonS3");
			request.HttpMethod = "DELETE";
			if (deleteObjectRequest.IsSetMfaCodes())
			{
				request.Headers.Add("x-amz-mfa", deleteObjectRequest.MfaCodes.FormattedMfaCodes);
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(deleteObjectRequest.BucketName), S3Transforms.ToStringValue(deleteObjectRequest.Key));
			if (deleteObjectRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(deleteObjectRequest.VersionId));
			}
			if (deleteObjectRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(deleteObjectRequest.RequestPayer.ToString()));
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
