using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;
using Amazon.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class CopyPartRequestMarshaller : IMarshaller<IRequest, CopyPartRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static CopyPartRequestMarshaller _instance;

		public static CopyPartRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CopyPartRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CopyPartRequest)input);
		}

		public IRequest Marshall(CopyPartRequest copyPartRequest)
		{
			IRequest request = new DefaultRequest(copyPartRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (copyPartRequest.IsSetSourceBucket())
			{
				request.Headers.Add("x-amz-copy-source", ConstructCopySourceHeaderValue(copyPartRequest.SourceBucket, copyPartRequest.SourceKey, copyPartRequest.SourceVersionId));
			}
			if (copyPartRequest.IsSetETagToMatch())
			{
				request.Headers.Add("x-amz-copy-source-if-match", AWSSDKUtils.Join(copyPartRequest.ETagToMatch));
			}
			if (copyPartRequest.IsSetETagToNotMatch())
			{
				request.Headers.Add("x-amz-copy-source-if-none-match", AWSSDKUtils.Join(copyPartRequest.ETagsToNotMatch));
			}
			if (copyPartRequest.IsSetModifiedSinceDate())
			{
				request.Headers.Add("x-amz-copy-source-if-modified-since", copyPartRequest.ModifiedSinceDate.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture));
			}
			if (copyPartRequest.IsSetUnmodifiedSinceDate())
			{
				request.Headers.Add("x-amz-copy-source-if-unmodified-since", copyPartRequest.UnmodifiedSinceDate.ToUniversalTime().ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture));
			}
			if (copyPartRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", copyPartRequest.ServerSideEncryptionCustomerMethod);
			}
			if (copyPartRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", copyPartRequest.ServerSideEncryptionCustomerProvidedKey);
				if (copyPartRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", copyPartRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyPartRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyPartRequest.IsSetCopySourceServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-algorithm", copyPartRequest.CopySourceServerSideEncryptionCustomerMethod);
			}
			if (copyPartRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-key", copyPartRequest.CopySourceServerSideEncryptionCustomerProvidedKey);
				if (copyPartRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", copyPartRequest.CopySourceServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyPartRequest.CopySourceServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyPartRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				request.Headers.Add("x-amz-server-side-encryption-aws-kms-key-id", copyPartRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (copyPartRequest.IsSetFirstByte() && copyPartRequest.IsSetLastByte())
			{
				request.Headers.Add("x-amz-copy-source-range", ConstructCopySourceRangeHeader(copyPartRequest.FirstByte, copyPartRequest.LastByte));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(copyPartRequest.DestinationBucket), S3Transforms.ToStringValue(copyPartRequest.DestinationKey));
			request.AddSubResource("partNumber", S3Transforms.ToStringValue(copyPartRequest.PartNumber));
			request.AddSubResource("uploadId", S3Transforms.ToStringValue(copyPartRequest.UploadId));
			request.UseQueryString = true;
			return request;
		}

		private static string ConstructCopySourceHeaderValue(string bucket, string key, string version)
		{
			string text;
			if (!string.IsNullOrEmpty(key))
			{
				text = AmazonS3Util.UrlEncode("/" + bucket + "/" + key, true);
				if (!string.IsNullOrEmpty(version))
				{
					text = string.Format(CultureInfo.InvariantCulture, "{0}?versionId={1}", text, AmazonS3Util.UrlEncode(version, true));
				}
			}
			else
			{
				text = AmazonS3Util.UrlEncode(bucket, true);
			}
			return text;
		}

		private static string ConstructCopySourceRangeHeader(long firstByte, long lastByte)
		{
			return string.Format(CultureInfo.InvariantCulture, "bytes={0}-{1}", firstByte, lastByte);
		}
	}
}
