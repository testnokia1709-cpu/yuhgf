using System;
using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class CopyObjectRequestMarshaller : IMarshaller<IRequest, CopyObjectRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static CopyObjectRequestMarshaller _instance;

		public static CopyObjectRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new CopyObjectRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((CopyObjectRequest)input);
		}

		public IRequest Marshall(CopyObjectRequest copyObjectRequest)
		{
			IRequest request = new DefaultRequest(copyObjectRequest, "AmazonS3");
			request.HttpMethod = "PUT";
			if (copyObjectRequest.IsSetCannedACL())
			{
				request.Headers.Add("x-amz-acl", S3Transforms.ToStringValue(copyObjectRequest.CannedACL));
			}
			HeadersCollection headers = copyObjectRequest.Headers;
			foreach (string key in headers.Keys)
			{
				request.Headers[key] = headers[key];
			}
			HeaderACLRequestMarshaller.Marshall(request, copyObjectRequest);
			if (copyObjectRequest.IsSetSourceBucket())
			{
				request.Headers.Add("x-amz-copy-source", ConstructCopySourceHeaderValue(copyObjectRequest.SourceBucket, copyObjectRequest.SourceKey, copyObjectRequest.SourceVersionId));
			}
			if (copyObjectRequest.IsSetETagToMatch())
			{
				request.Headers.Add("x-amz-copy-source-if-match", S3Transforms.ToStringValue(copyObjectRequest.ETagToMatch));
			}
			if (copyObjectRequest.IsSetModifiedSinceDate())
			{
				request.Headers.Add("x-amz-copy-source-if-modified-since", S3Transforms.ToStringValue(copyObjectRequest.ModifiedSinceDate));
			}
			if (copyObjectRequest.IsSetETagToNotMatch())
			{
				request.Headers.Add("x-amz-copy-source-if-none-match", S3Transforms.ToStringValue(copyObjectRequest.ETagToNotMatch));
			}
			if (copyObjectRequest.IsSetUnmodifiedSinceDate())
			{
				request.Headers.Add("x-amz-copy-source-if-unmodified-since", S3Transforms.ToStringValue(copyObjectRequest.UnmodifiedSinceDate));
			}
			if (copyObjectRequest.IsSetTagSet())
			{
				request.Headers.Add(S3Constants.AmzHeaderTagging, AmazonS3Util.TagSetToQueryString(copyObjectRequest.TagSet));
				request.Headers.Add(S3Constants.AmzHeaderTaggingDirective, TaggingDirective.REPLACE.Value);
			}
			else
			{
				request.Headers.Add(S3Constants.AmzHeaderTaggingDirective, TaggingDirective.COPY.Value);
			}
			request.Headers.Add("x-amz-metadata-directive", S3Transforms.ToStringValue(copyObjectRequest.MetadataDirective.ToString()));
			if (copyObjectRequest.IsSetServerSideEncryptionMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(copyObjectRequest.ServerSideEncryptionMethod));
			}
			if (copyObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", copyObjectRequest.ServerSideEncryptionCustomerMethod);
			}
			if (copyObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", copyObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (copyObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", copyObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyObjectRequest.IsSetCopySourceServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-algorithm", copyObjectRequest.CopySourceServerSideEncryptionCustomerMethod);
			}
			if (copyObjectRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-key", copyObjectRequest.CopySourceServerSideEncryptionCustomerProvidedKey);
				if (copyObjectRequest.IsSetCopySourceServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", copyObjectRequest.CopySourceServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-copy-source-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(copyObjectRequest.CopySourceServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (copyObjectRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				request.Headers.Add("x-amz-server-side-encryption-aws-kms-key-id", copyObjectRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (copyObjectRequest.IsSetStorageClass())
			{
				request.Headers.Add("x-amz-storage-class", S3Transforms.ToStringValue(copyObjectRequest.StorageClass));
			}
			if (copyObjectRequest.IsSetWebsiteRedirectLocation())
			{
				request.Headers.Add("x-amz-website-redirect-location", S3Transforms.ToStringValue(copyObjectRequest.WebsiteRedirectLocation));
			}
			if (copyObjectRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(copyObjectRequest.RequestPayer.ToString()));
			}
			AmazonS3Util.SetMetadataHeaders(request, copyObjectRequest.Metadata);
			string value = (copyObjectRequest.DestinationKey.StartsWith("/", StringComparison.Ordinal) ? copyObjectRequest.DestinationKey.Substring(1) : copyObjectRequest.DestinationKey);
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(copyObjectRequest.DestinationBucket), S3Transforms.ToStringValue(value));
			request.UseQueryString = true;
			return request;
		}

		private static string ConstructCopySourceHeaderValue(string bucket, string key, string version)
		{
			string text2;
			if (!string.IsNullOrEmpty(key))
			{
				string text = (key.StartsWith("/", StringComparison.Ordinal) ? key.Substring(1) : key);
				text2 = AmazonS3Util.UrlEncode("/" + bucket + "/" + text, true);
				if (!string.IsNullOrEmpty(version))
				{
					text2 = string.Format(CultureInfo.InvariantCulture, "{0}?versionId={1}", text2, AmazonS3Util.UrlEncode(version, true));
				}
			}
			else
			{
				text2 = AmazonS3Util.UrlEncode(bucket, true);
			}
			return text2;
		}
	}
}
