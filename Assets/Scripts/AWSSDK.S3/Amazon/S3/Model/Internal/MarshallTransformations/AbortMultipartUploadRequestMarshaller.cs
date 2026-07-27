using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class AbortMultipartUploadRequestMarshaller : IMarshaller<IRequest, AbortMultipartUploadRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static AbortMultipartUploadRequestMarshaller _instance;

		public static AbortMultipartUploadRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new AbortMultipartUploadRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((AbortMultipartUploadRequest)input);
		}

		public IRequest Marshall(AbortMultipartUploadRequest abortMultipartUploadRequest)
		{
			IRequest request = new DefaultRequest(abortMultipartUploadRequest, "AmazonS3");
			request.HttpMethod = "DELETE";
			if (abortMultipartUploadRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(abortMultipartUploadRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(abortMultipartUploadRequest.BucketName), S3Transforms.ToStringValue(abortMultipartUploadRequest.Key));
			request.AddSubResource("uploadId", S3Transforms.ToStringValue(abortMultipartUploadRequest.UploadId));
			request.UseQueryString = true;
			return request;
		}
	}
}
