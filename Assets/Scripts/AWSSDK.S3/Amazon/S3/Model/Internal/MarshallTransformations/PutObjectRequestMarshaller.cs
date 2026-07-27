using System;
using System.Globalization;
using System.IO;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class PutObjectRequestMarshaller : IMarshaller<IRequest, PutObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static PutObjectRequestMarshaller _instance;

		public static PutObjectRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new PutObjectRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((PutObjectRequest)input);
		}

		public IRequest Marshall(PutObjectRequest putObjectRequest)
		{
			IRequest request = new DefaultRequest(putObjectRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (putObjectRequest.IsSetCannedACL())
			{
				request.Headers.Add("x-amz-acl", S3Transforms.ToStringValue(putObjectRequest.CannedACL));
			}
			HeadersCollection headers = putObjectRequest.Headers;
			foreach (string key in headers.Keys)
			{
				request.Headers[key] = headers[key];
			}
			if (putObjectRequest.IsSetMD5Digest())
			{
				request.Headers["Content-MD5"] = putObjectRequest.MD5Digest;
			}
			HeaderACLRequestMarshaller.Marshall(request, putObjectRequest);
			if (putObjectRequest.IsSetServerSideEncryptionMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(putObjectRequest.ServerSideEncryptionMethod));
			}
			if (putObjectRequest.IsSetStorageClass())
			{
				request.Headers.Add("x-amz-storage-class", S3Transforms.ToStringValue(putObjectRequest.StorageClass));
			}
			if (putObjectRequest.IsSetWebsiteRedirectLocation())
			{
				request.Headers.Add("x-amz-website-redirect-location", S3Transforms.ToStringValue(putObjectRequest.WebsiteRedirectLocation));
			}
			if (putObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", putObjectRequest.ServerSideEncryptionCustomerMethod);
			}
			if (putObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", putObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (putObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", putObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(putObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (putObjectRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				request.Headers.Add("x-amz-server-side-encryption-aws-kms-key-id", putObjectRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (putObjectRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(putObjectRequest.RequestPayer.ToString()));
			}
			if (putObjectRequest.IsSetTagSet())
			{
				request.Headers.Add(S3Constants.AmzHeaderTagging, AmazonS3Util.TagSetToQueryString(putObjectRequest.TagSet));
			}
			AmazonS3Util.SetMetadataHeaders(request, putObjectRequest.Metadata);
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(putObjectRequest.BucketName), S3Transforms.ToStringValue(putObjectRequest.Key));
			if (putObjectRequest.InputStream != null)
			{
				Stream streamWithLength = GetStreamWithLength(putObjectRequest.InputStream, putObjectRequest.Headers.ContentLength);
				if (streamWithLength.Length > 0)
				{
					request.UseChunkEncoding = putObjectRequest.UseChunkEncoding;
				}
				long expectedLength = streamWithLength.Length - streamWithLength.Position;
				if (!request.Headers.ContainsKey("Content-Length"))
				{
					request.Headers.Add("Content-Length", expectedLength.ToString(CultureInfo.InvariantCulture));
				}
				MD5Stream inputStream = new MD5Stream(streamWithLength, null, expectedLength);
				putObjectRequest.InputStream = inputStream;
			}
			request.ContentStream = putObjectRequest.InputStream;
			if (!request.Headers.ContainsKey("Content-Type"))
			{
				request.Headers.Add("Content-Type", "text/plain");
			}
			return request;
		}

		private static Stream GetStreamWithLength(Stream baseStream, long hintLength)
		{
			Stream result = baseStream;
			bool flag = false;
			long num = -1L;
			try
			{
				num = baseStream.Length - baseStream.Position;
			}
			catch (NotSupportedException)
			{
				flag = true;
				num = hintLength;
			}
			if (num < 0)
			{
				throw new AmazonS3Exception("Could not determine content length");
			}
			if (flag)
			{
				result = new PartialReadOnlyWrapperStream(baseStream, num);
			}
			return result;
		}
	}
}
