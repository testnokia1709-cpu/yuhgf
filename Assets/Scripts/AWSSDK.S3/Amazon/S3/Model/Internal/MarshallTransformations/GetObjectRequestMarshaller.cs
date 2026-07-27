using System;
using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectRequestMarshaller : IMarshaller<IRequest, GetObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetObjectRequestMarshaller _instance;

		public static GetObjectRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectRequest)input);
		}

		public IRequest Marshall(GetObjectRequest getObjectRequest)
		{
			if (string.IsNullOrEmpty(getObjectRequest.Key))
			{
				throw new ArgumentException("Key is a required property and must be set before making this call.", "GetObjectRequest.Key");
			}
			IRequest request = new DefaultRequest(getObjectRequest, "AmazonS3");
			request.HttpMethod = "GET";
			if (getObjectRequest.IsSetEtagToMatch())
			{
				request.Headers.Add("If-Match", S3Transforms.ToStringValue(getObjectRequest.EtagToMatch));
			}
			if (getObjectRequest.IsSetModifiedSinceDate())
			{
				request.Headers.Add("If-Modified-Since", S3Transforms.ToStringValue(getObjectRequest.ModifiedSinceDate));
			}
			if (getObjectRequest.IsSetEtagToNotMatch())
			{
				request.Headers.Add("If-None-Match", S3Transforms.ToStringValue(getObjectRequest.EtagToNotMatch));
			}
			if (getObjectRequest.IsSetUnmodifiedSinceDate())
			{
				request.Headers.Add("If-Unmodified-Since", S3Transforms.ToStringValue(getObjectRequest.UnmodifiedSinceDate));
			}
			if (getObjectRequest.IsSetByteRange())
			{
				request.Headers.Add("Range", getObjectRequest.ByteRange.FormattedByteRange);
			}
			if (getObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", getObjectRequest.ServerSideEncryptionCustomerMethod);
			}
			if (getObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", getObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (getObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", getObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(getObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (getObjectRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(getObjectRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(getObjectRequest.BucketName), S3Transforms.ToStringValue(getObjectRequest.Key));
			ResponseHeaderOverrides responseHeaderOverrides = getObjectRequest.ResponseHeaderOverrides;
			if (responseHeaderOverrides.CacheControl != null)
			{
				request.Parameters.Add("response-cache-control", S3Transforms.ToStringValue(responseHeaderOverrides.CacheControl));
			}
			if (responseHeaderOverrides.ContentDisposition != null)
			{
				request.Parameters.Add("response-content-disposition", S3Transforms.ToStringValue(responseHeaderOverrides.ContentDisposition));
			}
			if (responseHeaderOverrides.ContentEncoding != null)
			{
				request.Parameters.Add("response-content-encoding", S3Transforms.ToStringValue(responseHeaderOverrides.ContentEncoding));
			}
			if (responseHeaderOverrides.ContentLanguage != null)
			{
				request.Parameters.Add("response-content-language", S3Transforms.ToStringValue(responseHeaderOverrides.ContentLanguage));
			}
			if (responseHeaderOverrides.ContentType != null)
			{
				request.Parameters.Add("response-content-type", S3Transforms.ToStringValue(responseHeaderOverrides.ContentType));
			}
			if (getObjectRequest.IsSetResponseExpires())
			{
				request.Parameters.Add("response-expires", S3Transforms.ToStringValue(getObjectRequest.ResponseExpires));
			}
			if (getObjectRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(getObjectRequest.VersionId));
			}
			if (getObjectRequest.IsSetPartNumber())
			{
				request.AddSubResource("partNumber", S3Transforms.ToStringValue(getObjectRequest.PartNumber.Value));
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
