using System;
using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class GetObjectMetadataRequestMarshaller : IMarshaller<IRequest, GetObjectMetadataRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static GetObjectMetadataRequestMarshaller _instance;

		public static GetObjectMetadataRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new GetObjectMetadataRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((GetObjectMetadataRequest)input);
		}

		public IRequest Marshall(GetObjectMetadataRequest headObjectRequest)
		{
			if (string.IsNullOrEmpty(headObjectRequest.Key))
			{
				throw new ArgumentException("Key is a required property and must be set before making this call.", "GetObjectMetadataRequest.Key");
			}
			IRequest request = new DefaultRequest(headObjectRequest, "AmazonS3");
			request.HttpMethod = "HEAD";
			if (headObjectRequest.IsSetEtagToMatch())
			{
				request.Headers.Add("If-Match", S3Transforms.ToStringValue(headObjectRequest.EtagToMatch));
			}
			if (headObjectRequest.IsSetModifiedSinceDate())
			{
				request.Headers.Add("If-Modified-Since", S3Transforms.ToStringValue(headObjectRequest.ModifiedSinceDate));
			}
			if (headObjectRequest.IsSetEtagToNotMatch())
			{
				request.Headers.Add("If-None-Match", S3Transforms.ToStringValue(headObjectRequest.EtagToNotMatch));
			}
			if (headObjectRequest.IsSetUnmodifiedSinceDate())
			{
				request.Headers.Add("If-Unmodified-Since", S3Transforms.ToStringValue(headObjectRequest.UnmodifiedSinceDate));
			}
			if (headObjectRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", headObjectRequest.ServerSideEncryptionCustomerMethod);
			}
			if (headObjectRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", headObjectRequest.ServerSideEncryptionCustomerProvidedKey);
				if (headObjectRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", headObjectRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(headObjectRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (headObjectRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(headObjectRequest.RequestPayer.ToString()));
			}
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(headObjectRequest.BucketName), S3Transforms.ToStringValue(headObjectRequest.Key));
			if (headObjectRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(headObjectRequest.VersionId));
			}
			if (headObjectRequest.IsSetPartNumber())
			{
				request.AddSubResource("partNumber", S3Transforms.ToStringValue(headObjectRequest.PartNumber.Value));
			}
			request.UseQueryString = true;
			return request;
		}
	}
}
