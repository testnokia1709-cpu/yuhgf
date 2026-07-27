using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3.Model;
using Amazon.S3.Util;

namespace Amazon.S3.Internal
{
	public class AmazonS3RetryPolicy : DefaultRetryPolicy
	{
		private const string AWS_KMS_Signature_Error = "AWS KMS managed keys require AWS Signature Version 4";

		private static ICollection<Type> RequestsWith200Error = new HashSet<Type>
		{
			typeof(CopyObjectRequest),
			typeof(CopyPartRequest),
			typeof(CompleteMultipartUploadRequest)
		};

		public AmazonS3RetryPolicy(IClientConfig config)
			: base(config)
		{
		}

		public bool? RetryForExceptionSync(IExecutionContext executionContext, Exception exception)
		{
			AmazonServiceException ex = exception as AmazonServiceException;
			if (ex != null)
			{
				if (ex.StatusCode == HttpStatusCode.OK)
				{
					Type type = executionContext.RequestContext.OriginalRequest.GetType();
					if (RequestsWith200Error.Contains(type))
					{
						return true;
					}
				}
				if (ex.StatusCode == HttpStatusCode.BadRequest)
				{
					if (new Uri(executionContext.RequestContext.ClientConfig.DetermineServiceURL()).Host.Equals("s3.amazonaws.com") && (ex.Message.Contains("AWS4-HMAC-SHA256") || ex.Message.Contains("AWS KMS managed keys require AWS Signature Version 4")))
					{
						base.Logger.InfoFormat("Request {0}: the bucket you are attempting to access should be addressed using a region-specific endpoint. Additional calls will be made to attempt to determine the correct region to be used. For better performance configure your client to use the correct region.", executionContext.RequestContext.RequestName);
						IRequest request = executionContext.RequestContext.Request;
						AmazonS3Uri amazonS3Uri = new AmazonS3Uri(request.Endpoint);
						string uriString = string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}", amazonS3Uri.Bucket, "s3-external-1.amazonaws.com");
						request.Endpoint = new Uri(uriString);
						if (ex.Message.Contains("AWS KMS managed keys require AWS Signature Version 4"))
						{
							request.UseSigV4 = true;
							request.AuthenticationRegion = RegionEndpoint.USEast1.SystemName;
							executionContext.RequestContext.IsSigned = false;
						}
						return true;
					}
					return null;
				}
			}
			return RetryForException(executionContext, exception);
		}
	}
}
