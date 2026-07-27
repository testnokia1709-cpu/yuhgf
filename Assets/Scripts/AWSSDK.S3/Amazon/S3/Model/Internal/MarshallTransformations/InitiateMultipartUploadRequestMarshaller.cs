using System.Globalization;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.S3.Util;

namespace Amazon.S3.Model.Internal.MarshallTransformations
{
	public class InitiateMultipartUploadRequestMarshaller : IMarshaller<IRequest, InitiateMultipartUploadRequest>, IMarshaller<IRequest, AmazonWebServiceRequest>
	{
		private static InitiateMultipartUploadRequestMarshaller _instance;

		public static InitiateMultipartUploadRequestMarshaller Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new InitiateMultipartUploadRequestMarshaller();
				}
				return _instance;
			}
		}

		public IRequest Marshall(AmazonWebServiceRequest input)
		{
			return Marshall((InitiateMultipartUploadRequest)input);
		}

		public IRequest Marshall(InitiateMultipartUploadRequest initiateMultipartUploadRequest)
		{
			IRequest request = new DefaultRequest(initiateMultipartUploadRequest, "AmazonS3");
			request.HttpMethod = "POST";
			if (initiateMultipartUploadRequest.IsSetCannedACL())
			{
				request.Headers.Add("x-amz-acl", S3Transforms.ToStringValue(initiateMultipartUploadRequest.CannedACL));
			}
			HeadersCollection headers = initiateMultipartUploadRequest.Headers;
			foreach (string key in headers.Keys)
			{
				request.Headers.Add(key, headers[key]);
			}
			HeaderACLRequestMarshaller.Marshall(request, initiateMultipartUploadRequest);
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(initiateMultipartUploadRequest.ServerSideEncryptionMethod));
			}
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", initiateMultipartUploadRequest.ServerSideEncryptionCustomerMethod);
			}
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionCustomerProvidedKey())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-key", initiateMultipartUploadRequest.ServerSideEncryptionCustomerProvidedKey);
				if (initiateMultipartUploadRequest.IsSetServerSideEncryptionCustomerProvidedKeyMD5())
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", initiateMultipartUploadRequest.ServerSideEncryptionCustomerProvidedKeyMD5);
				}
				else
				{
					request.Headers.Add("x-amz-server-side-encryption-customer-key-MD5", AmazonS3Util.ComputeEncodedMD5FromEncodedString(initiateMultipartUploadRequest.ServerSideEncryptionCustomerProvidedKey));
				}
			}
			if (initiateMultipartUploadRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				request.Headers.Add("x-amz-server-side-encryption-aws-kms-key-id", initiateMultipartUploadRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (initiateMultipartUploadRequest.IsSetStorageClass())
			{
				request.Headers.Add("x-amz-storage-class", S3Transforms.ToStringValue(initiateMultipartUploadRequest.StorageClass));
			}
			if (initiateMultipartUploadRequest.IsSetWebsiteRedirectLocation())
			{
				request.Headers.Add("x-amz-website-redirect-location", S3Transforms.ToStringValue(initiateMultipartUploadRequest.WebsiteRedirectLocation));
			}
			if (initiateMultipartUploadRequest.IsSetRequestPayer())
			{
				request.Headers.Add(S3Constants.AmzHeaderRequestPayer, S3Transforms.ToStringValue(initiateMultipartUploadRequest.RequestPayer.ToString()));
			}
			if (initiateMultipartUploadRequest.IsSetTagSet())
			{
				request.Headers.Add(S3Constants.AmzHeaderTagging, AmazonS3Util.TagSetToQueryString(initiateMultipartUploadRequest.TagSet));
			}
			AmazonS3Util.SetMetadataHeaders(request, initiateMultipartUploadRequest.Metadata);
			request.ResourcePath = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", S3Transforms.ToStringValue(initiateMultipartUploadRequest.BucketName), S3Transforms.ToStringValue(initiateMultipartUploadRequest.Key));
			request.AddSubResource("uploads");
			request.UseQueryString = true;
			return request;
		}
	}
}
