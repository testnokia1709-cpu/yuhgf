using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class UploadPartRequestMarshaller : IMarshaller<IRequest, UploadPartRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static UploadPartRequestMarshaller _instance;

		public static UploadPartRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new UploadPartRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((UploadPartRequest)input);
		}

		public IRequest Marshall(UploadPartRequest uploadPartRequest)
		{
			IRequest request = new DefaultRequest(uploadPartRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (uploadPartRequest.IsSetMD5Digest())
			{
				request.Headers["Content-MD5"] = uploadPartRequest.MD5Digest;
			}
			if (uploadPartRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", uploadPartRequest.ServerSideEncryptionCustomerMethod);
			}
			if (uploadPartRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", uploadPartRequest.ServerSideEncryptionCustomerProvidedKey);
				if (uploadPartRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", uploadPartRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(uploadPartRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (uploadPartRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(uploadPartRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(uploadPartRequest.BucketName), S3Transforms.ToStringValue(uploadPartRequest.Key));
			if (uploadPartRequest.IsSetPartNumber())
			{
				request.AddSubResource("partNumber", S3Transforms.ToStringValue(uploadPartRequest.PartNumber));
			}
			if (uploadPartRequest.IsSetUploadId())
			{
				request.AddSubResource("uploadId", S3Transforms.ToStringValue(uploadPartRequest.UploadId));
			}
			if (uploadPartRequest.InputStream != null)
			{
				PartialWrapperStream partialWrapperStream = new PartialWrapperStream(uploadPartRequest.InputStream, uploadPartRequest.PartSize);
				if (partialWrapperStream.Length > 0)
				{
					request.UseChunkEncoding = uploadPartRequest.UseChunkEncoding;
				}
				if (!request.Headers.ContainsKey("Content-Length"))
				{
					request.Headers.Add("Content-Length", partialWrapperStream.Length.ToString(CultureInfo.InvariantCulture));
				}
				MD5Stream inputStream = new MD5Stream(partialWrapperStream, null, partialWrapperStream.Length);
				uploadPartRequest.InputStream = inputStream;
			}
			request.ContentStream = uploadPartRequest.InputStream;
			if (!request.Headers.ContainsKey("Content-Type"))
			{
				request.Headers.Add("Content-Type", "text/plain");
			}
			return request;
		}
	}
}
