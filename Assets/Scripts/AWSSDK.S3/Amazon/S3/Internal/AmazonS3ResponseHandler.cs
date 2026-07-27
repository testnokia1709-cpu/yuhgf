using System;
using System.Linq;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;
using Amazon.Util;

namespace Amazon.S3.Internal
{
	public class AmazonS3ResponseHandler : PipelineHandler
	{
		private static char[] etagTrimChars = new char[1] { '"' };

		public override void InvokeSync(IExecutionContext executionContext)
		{
			base.InvokeSync(executionContext);
			PostInvoke(executionContext);
		}

		protected override void InvokeAsyncCallback(IAsyncExecutionContext executionContext)
		{
			if (executionContext.ResponseContext.AsyncResult.Exception == null)
			{
				PostInvoke(ExecutionContext.CreateFromAsyncContext(executionContext));
			}
			base.InvokeAsyncCallback(executionContext);
		}

		protected virtual void PostInvoke(IExecutionContext executionContext)
		{
			ProcessResponseHandlers(executionContext);
		}

		private static void ProcessResponseHandlers(IExecutionContext executionContext)
		{
			AmazonWebServiceResponse response = executionContext.ResponseContext.Response;
			IRequest request = executionContext.RequestContext.Request;
			bool flag = HasSSEHeaders(executionContext.ResponseContext.HttpResponse);
			GetObjectResponse getObjectResponse = response as GetObjectResponse;
			if (getObjectResponse != null)
			{
				GetObjectRequest getObjectRequest = request.OriginalRequest as GetObjectRequest;
				getObjectResponse.BucketName = getObjectRequest.BucketName;
				getObjectResponse.Key = getObjectRequest.Key;
				if (!string.IsNullOrEmpty(getObjectResponse.ETag) && !getObjectResponse.ETag.Contains("-") && !flag && getObjectRequest.ByteRange == null)
				{
					HashStream responseStream = new MD5Stream(expectedHash: AWSSDKUtils.HexStringToBytes(getObjectResponse.ETag.Trim(etagTrimChars)), baseStream: getObjectResponse.ResponseStream, expectedLength: getObjectResponse.ContentLength);
					getObjectResponse.ResponseStream = responseStream;
				}
			}
			DeleteObjectsResponse deleteObjectsResponse = response as DeleteObjectsResponse;
			if (deleteObjectsResponse != null && deleteObjectsResponse.DeleteErrors != null && deleteObjectsResponse.DeleteErrors.Count > 0)
			{
				throw new DeleteObjectsException(deleteObjectsResponse);
			}
			PutObjectResponse putObjectResponse = response as PutObjectResponse;
			PutObjectRequest putObjectRequest = request.OriginalRequest as PutObjectRequest;
			if (putObjectRequest != null)
			{
				HashStream hashStream = putObjectRequest.InputStream as HashStream;
				if (hashStream != null)
				{
					if (putObjectResponse != null && !flag)
					{
						hashStream.CalculateHash();
						CompareHashes(putObjectResponse.ETag, hashStream.CalculatedHash);
					}
					putObjectRequest.InputStream = hashStream.GetNonWrapperBaseStream();
				}
			}
			ListObjectsResponse listObjectsResponse = response as ListObjectsResponse;
			if (listObjectsResponse != null && listObjectsResponse.IsTruncated && string.IsNullOrEmpty(listObjectsResponse.NextMarker) && listObjectsResponse.S3Objects.Count > 0)
			{
				listObjectsResponse.NextMarker = listObjectsResponse.S3Objects.Last().Key;
			}
			UploadPartRequest uploadPartRequest = request.OriginalRequest as UploadPartRequest;
			UploadPartResponse uploadPartResponse = response as UploadPartResponse;
			if (uploadPartRequest != null)
			{
				if (uploadPartResponse != null)
				{
					uploadPartResponse.PartNumber = uploadPartRequest.PartNumber;
				}
				HashStream hashStream2 = uploadPartRequest.InputStream as HashStream;
				if (hashStream2 != null)
				{
					if (uploadPartResponse != null && !flag)
					{
						hashStream2.CalculateHash();
						CompareHashes(uploadPartResponse.ETag, hashStream2.CalculatedHash);
					}
					uploadPartRequest.InputStream = hashStream2.GetNonWrapperBaseStream();
				}
			}
			CopyPartResponse copyPartResponse = response as CopyPartResponse;
			if (copyPartResponse != null)
			{
				copyPartResponse.PartNumber = ((CopyPartRequest)request.OriginalRequest).PartNumber;
			}
			AmazonS3Client.CleanupRequest(request.OriginalRequest);
		}

		private static bool HasSSEHeaders(IWebResponseData webResponseData)
		{
			bool num = !string.IsNullOrEmpty(webResponseData.GetHeaderValue("x-amz-server-side-encryption-customer-algorithm"));
			bool flag = !string.IsNullOrEmpty(webResponseData.GetHeaderValue("x-amz-server-side-encryption-aws-kms-key-id"));
			return num || flag;
		}

		private static void CompareHashes(string etag, byte[] hash)
		{
			if (!string.IsNullOrEmpty(etag) && !etag.Contains("-"))
			{
				etag = etag.Trim(etagTrimChars);
				string b = AWSSDKUtils.BytesToHexString(hash);
				if (!string.Equals(etag, b, StringComparison.OrdinalIgnoreCase))
				{
					throw new AmazonClientException("Expected hash not equal to calculated hash");
				}
			}
		}
	}
}
