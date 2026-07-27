using System;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Model;

namespace Amazon.S3.Util
{
	public static class BucketRegionDetector
	{
		private const int BucketRegionCacheMaxEntries = 300;

		private const string AuthorizationHeaderMalformedErrorCode = "AuthorizationHeaderMalformed";

		public static LruCache<string, RegionEndpoint> BucketRegionCache { get; private set; }

		static BucketRegionDetector()
		{
			BucketRegionCache = new LruCache<string, RegionEndpoint>(300);
		}

		internal static string GetCorrectRegion(AmazonS3Uri requestedBucketUri, HttpStatusCode headBucketStatusCode, string xAmzBucketRegionHeaderValue)
		{
			if (xAmzBucketRegionHeaderValue != null && headBucketStatusCode == HttpStatusCode.BadRequest)
			{
				return CheckRegionAndUpdateCache(requestedBucketUri, xAmzBucketRegionHeaderValue);
			}
			return null;
		}

		private static string GetCorrectRegion(AmazonS3Uri requestedBucketUri, AmazonServiceException serviceException)
		{
			string text = null;
			string text2 = null;
			AmazonS3Exception ex = serviceException as AmazonS3Exception;
			if (ex != null)
			{
				if (string.Equals(ex.ErrorCode, "AuthorizationHeaderMalformed", StringComparison.Ordinal))
				{
					text = CheckRegionAndUpdateCache(requestedBucketUri, ex.Region);
				}
				if (text == null)
				{
					HttpErrorResponseException ex2 = ex.InnerException as HttpErrorResponseException;
					if (ex2 != null && ex2.Response != null && ex2.Response.IsHeaderPresent("x-amz-bucket-region"))
					{
						text2 = CheckRegionAndUpdateCache(requestedBucketUri, ex2.Response.GetHeaderValue("x-amz-bucket-region"));
					}
				}
			}
			return text ?? text2;
		}

		private static string CheckRegionAndUpdateCache(AmazonS3Uri requestedBucketUri, string actualRegion)
		{
			string a = ((requestedBucketUri.Region == null) ? null : requestedBucketUri.Region.SystemName);
			if (actualRegion != null && !string.Equals(a, actualRegion, StringComparison.Ordinal))
			{
				BucketRegionCache.AddOrUpdate(requestedBucketUri.Bucket, RegionEndpoint.GetBySystemName(actualRegion));
				return actualRegion;
			}
			return null;
		}

		private static string GetHeadBucketPreSignedUrl(string bucketName, ImmutableCredentials credentials)
		{
			using (AmazonS3Client amazonS3Client = GetUsEast1ClientFromCredentials(credentials))
			{
				GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
				{
					BucketName = bucketName,
					Expires = amazonS3Client.Config.CorrectedUtcNow.ToLocalTime().AddDays(1.0),
					Verb = HttpVerb.HEAD,
					Protocol = Protocol.HTTP
				};
				return amazonS3Client.GetPreSignedURLInternal(request, false);
			}
		}

		private static AmazonS3Client GetUsEast1ClientFromCredentials(ImmutableCredentials credentials)
		{
			if (credentials.UseToken)
			{
				return new AmazonS3Client(credentials.AccessKey, credentials.SecretKey, credentials.Token, RegionEndpoint.USEast1);
			}
			return new AmazonS3Client(credentials.AccessKey, credentials.SecretKey, RegionEndpoint.USEast1);
		}
	}
}
