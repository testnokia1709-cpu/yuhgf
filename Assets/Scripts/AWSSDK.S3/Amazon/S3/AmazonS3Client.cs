using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Transform;
using Amazon.Runtime.Internal.Util;
using Amazon.S3.Internal;
using Amazon.S3.Model;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Amazon.S3.Util;
using Amazon.Util;

namespace Amazon.S3
{
	public class AmazonS3Client : AmazonServiceClient, IAmazonS3, IAmazonService, IDisposable
	{
		IClientConfig IAmazonService.Config
		{
			get
			{
				return base.Config;
			}
		}

		internal string GetPreSignedURLInternal(GetPreSignedUrlRequest request, bool useSigV2Fallback = true)
		{
			if (base.Credentials == null)
			{
				throw new AmazonS3Exception("Credentials must be specified, cannot call method anonymously");
			}
			if (request == null)
			{
				throw new ArgumentNullException("request", "The PreSignedUrlRequest specified is null!");
			}
			if (!request.IsSetExpires())
			{
				throw new InvalidOperationException("The Expires specified is null!");
			}
			bool flag = AWSConfigsS3.UseSignatureVersion4;
			string text = AWS4Signer.DetermineSigningRegion(base.Config, "s3", null, null);
			if (flag && string.IsNullOrEmpty(text))
			{
				throw new InvalidOperationException("To use AWS4 signing, a region must be specified in the client configuration using the AuthenticationRegion or Region properties, or be determinable from the service URL.");
			}
			RegionEndpoint bySystemName = RegionEndpoint.GetBySystemName(text);
			if (bySystemName.GetEndpointForService("s3").SignatureVersionOverride == "4" || bySystemName.GetEndpointForService("s3").SignatureVersionOverride == null)
			{
				flag = true;
			}
			bool flag2 = useSigV2Fallback && !AWSConfigsS3.UseSigV4SetExplicitly;
			if (bySystemName == RegionEndpoint.USEast1 && flag2)
			{
				flag = false;
			}
			if (flag && GetSecondsUntilExpiration(base.Config, request, flag) > 604800 && bySystemName.GetEndpointForService("s3").SignatureVersionOverride == "2")
			{
				flag = false;
			}
			ImmutableCredentials credentials = base.Credentials.GetCredentials();
			IRequest request2 = Marshall(base.Config, request, credentials.AccessKey, credentials.Token, flag);
			request2.Endpoint = EndpointResolver.DetermineEndpoint(base.Config, request2);
			AmazonS3PostMarshallHandler.ProcessRequestHandlers(new Amazon.Runtime.Internal.ExecutionContext(new RequestContext(true, new NullSigner())
			{
				Request = request2,
				ClientConfig = base.Config
			}, null));
			RequestMetrics metrics = new RequestMetrics();
			string text2;
			if (flag)
			{
				AWS4SigningResult aWS4SigningResult = new AWS4PreSignedUrlSigner().SignRequest(request2, base.Config, metrics, credentials.AccessKey, credentials.SecretKey);
				text2 = "&" + aWS4SigningResult.ForQueryParameters;
			}
			else
			{
				S3Signer.SignRequest(request2, metrics, credentials.AccessKey, credentials.SecretKey);
				text2 = request2.Headers["Authorization"];
				text2 = text2.Substring(text2.IndexOf(":", StringComparison.Ordinal) + 1);
				text2 = "&Signature=" + AmazonS3Util.UrlEncode(text2, false);
			}
			string text3 = AmazonServiceClient.ComposeUrl(request2).AbsoluteUri + text2;
			Protocol protocol = DetermineProtocol();
			if (request.Protocol != protocol)
			{
				switch (protocol)
				{
				case Protocol.HTTP:
					text3 = text3.Replace("http://", "https://");
					break;
				case Protocol.HTTPS:
					text3 = text3.Replace("https://", "http://");
					break;
				}
			}
			return text3;
		}

		private static IRequest Marshall(IClientConfig config, GetPreSignedUrlRequest getPreSignedUrlRequest, string accessKey, string token, bool aws4Signing)
		{
			IRequest request = new DefaultRequest(getPreSignedUrlRequest, "AmazonS3");
			request.HttpMethod = getPreSignedUrlRequest.Verb.ToString();
			HeadersCollection headers = getPreSignedUrlRequest.Headers;
			foreach (string key in headers.Keys)
			{
				request.Headers[key] = headers[key];
			}
			AmazonS3Util.SetMetadataHeaders(request, getPreSignedUrlRequest.Metadata);
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers["x-amz-security-token"] = token;
			}
			if (getPreSignedUrlRequest.ServerSideEncryptionMethod != null && getPreSignedUrlRequest.ServerSideEncryptionMethod != ServerSideEncryptionMethod.None)
			{
				request.Headers.Add("x-amz-server-side-encryption", S3Transforms.ToStringValue(getPreSignedUrlRequest.ServerSideEncryptionMethod));
			}
			if (getPreSignedUrlRequest.IsSetServerSideEncryptionCustomerMethod())
			{
				request.Headers.Add("x-amz-server-side-encryption-customer-algorithm", getPreSignedUrlRequest.ServerSideEncryptionCustomerMethod);
			}
			if (getPreSignedUrlRequest.IsSetServerSideEncryptionKeyManagementServiceKeyId())
			{
				request.Headers.Add("x-amz-server-side-encryption-aws-kms-key-id", getPreSignedUrlRequest.ServerSideEncryptionKeyManagementServiceKeyId);
			}
			if (getPreSignedUrlRequest.IsSetRequestPayer() && getPreSignedUrlRequest.RequestPayer == RequestPayer.Requester)
			{
				request.Parameters.Add("x-amz-request-payer", RequestPayer.Requester.Value);
			}
			IDictionary<string, string> parameters = request.Parameters;
			StringBuilder stringBuilder = new StringBuilder("/");
			if (!string.IsNullOrEmpty(getPreSignedUrlRequest.BucketName))
			{
				stringBuilder.Append(S3Transforms.ToStringValue(getPreSignedUrlRequest.BucketName));
			}
			if (!string.IsNullOrEmpty(getPreSignedUrlRequest.Key))
			{
				if (stringBuilder.Length > 1)
				{
					stringBuilder.Append("/");
				}
				stringBuilder.Append(S3Transforms.ToStringValue(getPreSignedUrlRequest.Key));
			}
			long secondsUntilExpiration = GetSecondsUntilExpiration(config, getPreSignedUrlRequest, aws4Signing);
			if (aws4Signing && secondsUntilExpiration > 604800)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The maximum expiry period for a presigned url using AWS4 signing is {0} seconds", 604800L));
			}
			parameters.Add(aws4Signing ? "X-Amz-Expires" : "Expires", secondsUntilExpiration.ToString(CultureInfo.InvariantCulture));
			if (!string.IsNullOrEmpty(token))
			{
				parameters.Add("x-amz-security-token", token);
			}
			if (!aws4Signing)
			{
				parameters.Add("AWSAccessKeyId", accessKey);
			}
			if (getPreSignedUrlRequest.IsSetVersionId())
			{
				request.AddSubResource("versionId", S3Transforms.ToStringValue(getPreSignedUrlRequest.VersionId));
			}
			ResponseHeaderOverrides responseHeaderOverrides = getPreSignedUrlRequest.ResponseHeaderOverrides;
			if (!string.IsNullOrEmpty(responseHeaderOverrides.CacheControl))
			{
				parameters.Add("response-cache-control", responseHeaderOverrides.CacheControl);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentType))
			{
				parameters.Add("response-content-type", responseHeaderOverrides.ContentType);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentLanguage))
			{
				parameters.Add("response-content-language", responseHeaderOverrides.ContentLanguage);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.Expires))
			{
				parameters.Add("response-expires", responseHeaderOverrides.Expires);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentDisposition))
			{
				parameters.Add("response-content-disposition", responseHeaderOverrides.ContentDisposition);
			}
			if (!string.IsNullOrEmpty(responseHeaderOverrides.ContentEncoding))
			{
				parameters.Add("response-content-encoding", responseHeaderOverrides.ContentEncoding);
			}
			foreach (string key2 in getPreSignedUrlRequest.Parameters.Keys)
			{
				parameters.Add(key2, getPreSignedUrlRequest.Parameters[key2]);
			}
			request.ResourcePath = stringBuilder.ToString();
			request.UseQueryString = true;
			return request;
		}

		private static long GetSecondsUntilExpiration(IClientConfig config, GetPreSignedUrlRequest request, bool aws4Signing)
		{
			DateTime dateTime = (aws4Signing ? config.CorrectedUtcNow : new DateTime(1970, 1, 1));
			return Convert.ToInt64((request.Expires.ToUniversalTime() - dateTime).TotalSeconds);
		}

		private Protocol DetermineProtocol()
		{
			if (!base.Config.DetermineServiceURL().StartsWith("https", StringComparison.OrdinalIgnoreCase))
			{
				return Protocol.HTTP;
			}
			return Protocol.HTTPS;
		}

		internal static void CleanupRequest(AmazonWebServiceRequest request)
		{
			PutObjectRequest putObjectRequest = request as PutObjectRequest;
			if (putObjectRequest != null)
			{
				if (putObjectRequest.InputStream != null && (!string.IsNullOrEmpty(putObjectRequest.FilePath) || putObjectRequest.AutoCloseStream))
				{
					putObjectRequest.InputStream.Dispose();
				}
				if (!string.IsNullOrEmpty(putObjectRequest.FilePath) || !string.IsNullOrEmpty(putObjectRequest.ContentBody))
				{
					putObjectRequest.InputStream = null;
				}
			}
			UploadPartRequest uploadPartRequest = request as UploadPartRequest;
			if (uploadPartRequest != null)
			{
				if (uploadPartRequest.IsSetFilePath() && uploadPartRequest.InputStream != null)
				{
					uploadPartRequest.InputStream.Dispose();
				}
				if (uploadPartRequest.IsSetFilePath())
				{
					uploadPartRequest.InputStream = null;
				}
			}
		}

		public void GetPreSignedURLAsync(GetPreSignedUrlRequest request, AmazonServiceCallback<GetPreSignedUrlRequest, GetPreSignedUrlResponse> callback, AsyncOptions options = null)
		{
			options = options ?? new AsyncOptions();
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					callback(new AmazonServiceResult<GetPreSignedUrlRequest, GetPreSignedUrlResponse>(request, new GetPreSignedUrlResponse(GetPreSignedURLInternal(request)), null, options.State));
				}
				catch (Exception exception)
				{
					callback(new AmazonServiceResult<GetPreSignedUrlRequest, GetPreSignedUrlResponse>(request, null, exception, options.State));
				}
			});
		}

		public void PostObjectAsync(PostObjectRequest request, AmazonServiceCallback<PostObjectRequest, PostObjectResponse> callback, AsyncOptions options = null)
		{
			options = options ?? new AsyncOptions();
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
			{
				AmazonServiceResult<PostObjectRequest, PostObjectResponse> responseObject = new AmazonServiceResult<PostObjectRequest, PostObjectResponse>((PostObjectRequest)req, (PostObjectResponse)res, ex, ao.State);
				if (callback != null)
				{
					callback(responseObject);
				}
			};
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					InferContentType(request);
					if (request.SignedPolicy == null)
					{
						CreateSignedPolicy(request);
					}
					PostObject(request, options, callbackHelper);
				}
				catch (Exception exception)
				{
					callback(new AmazonServiceResult<PostObjectRequest, PostObjectResponse>(request, null, exception, options.State));
				}
			});
		}

		private void InferContentType(PostObjectRequest request)
		{
			if (string.IsNullOrEmpty(request.Headers.ContentType))
			{
				if (request.Key.IndexOf('.') > -1)
				{
					request.Headers.ContentType = AmazonS3Util.MimeTypeFromExtension(request.Key.Substring(request.Key.LastIndexOf('.')));
				}
				else if (!string.IsNullOrEmpty(request.Path) && request.Path.IndexOf('.') > -1)
				{
					request.Headers.ContentType = AmazonS3Util.MimeTypeFromExtension(request.Key.Substring(request.Path.LastIndexOf('.')));
				}
				else
				{
					request.Headers.ContentType = "application/octet-stream";
				}
			}
		}

		private void CreateSignedPolicy(PostObjectRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> item in request.Metadata)
			{
				string arg = (item.Key.StartsWith(S3Constants.PostFormDataXAmzPrefix, StringComparison.Ordinal) ? item.Key : (S3Constants.PostFormDataMetaPrefix + item.Key));
				stringBuilder.Append(string.Format(",{{\"{0}\": \"{1}\"}}", arg, item.Value));
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			foreach (string key in request.Headers.Keys)
			{
				stringBuilder2.Append(string.Format(",{{\"{0}\": \"{1}\"}}", key, request.Headers[key]));
			}
			string text = null;
			int num = request.Key.LastIndexOf('/');
			text = ((num != -1) ? ("{\"expiration\": \"" + AWSSDKUtils.CorrectedUtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" + request.Bucket + "\"},[\"starts-with\", \"$key\", \"" + request.Key.Substring(0, num) + "/\"],{\"acl\": \"" + request.CannedACL.Value + "\"},[\"eq\", \"$Content-Type\", \"" + request.Headers.ContentType + "\"]" + stringBuilder.ToString() + stringBuilder2.ToString() + "]}") : ("{\"expiration\": \"" + AWSSDKUtils.CorrectedUtcNow.AddHours(24.0).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" + request.Bucket + "\"},[\"starts-with\", \"$key\", \"\"],{\"acl\": \"" + request.CannedACL.Value + "\"},[\"eq\", \"$Content-Type\", \"" + request.Headers.ContentType + "\"]" + stringBuilder.ToString() + stringBuilder2.ToString() + "]}"));
			if (base.Config.SignatureVersion == "2")
			{
				request.SignedPolicy = S3PostUploadSignedPolicy.GetSignedPolicy(text, base.Credentials);
			}
			else
			{
				request.SignedPolicy = S3PostUploadSignedPolicy.GetSignedPolicyV4(text, base.Credentials, request.Region);
			}
		}

		private void PostObject(PostObjectRequest request, AsyncOptions options, Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper)
		{
			string text = (request.Region.Equals(RegionEndpoint.USEast1) ? "s3" : ("s3-" + request.Region.SystemName));
			IDictionary<string, string> dictionary = new Dictionary<string, string>();
			string uriString = ((request.Bucket.IndexOf('.') <= -1) ? string.Format(CultureInfo.InvariantCulture, "https://{0}.{1}.amazonaws.com", request.Bucket, text) : string.Format(CultureInfo.InvariantCulture, "https://{0}.amazonaws.com/{1}/", text, request.Bucket));
			Uri requestUri = new Uri(uriString);
			IHttpRequest<string> httpRequest = null;
			httpRequest = ((AWSConfigs.HttpClient != AWSConfigs.HttpClientOption.UnityWWW) ? ((IHttpRequest<string>)new UnityRequest(requestUri)) : ((IHttpRequest<string>)new UnityWwwRequest(requestUri)));
			string text2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace('=', 'z');
			dictionary["Content-Type"] = string.Format(CultureInfo.InvariantCulture, "multipart/form-data; boundary={0}", text2);
			dictionary["User-Agent"] = "User-Agent";
			httpRequest.Method = "POST";
			using (MemoryStream memoryStream = new MemoryStream())
			{
				request.WriteFormData(text2, memoryStream);
				byte[] bytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "--{0}\r\nContent-Disposition: form-data; name=\"file\"\r\n\r\n", text2));
				memoryStream.Write(bytes, 0, bytes.Length);
				using (Stream stream = ((request.Path == null) ? request.InputStream : File.OpenRead(request.Path)))
				{
					byte[] buffer = new byte[1024];
					int count;
					while ((count = stream.Read(buffer, 0, 1024)) > 0)
					{
						memoryStream.Write(buffer, 0, count);
					}
				}
				byte[] bytes2 = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "\r\n--{0}--", text2));
				memoryStream.Write(bytes2, 0, bytes2.Length);
				httpRequest.WriteToRequestBody(null, memoryStream.ToArray(), dictionary);
				EventHandler<StreamTransferProgressArgs> streamUploadProgressCallback = ((IAmazonWebServiceRequest)request).StreamUploadProgressCallback;
				if (streamUploadProgressCallback != null)
				{
					httpRequest.SetupProgressListeners(memoryStream, 0L, request, streamUploadProgressCallback);
				}
			}
			AsyncExecutionContext asyncExecutionContext = new AsyncExecutionContext(new AsyncRequestContext(base.Config.LogMetrics, new NullSigner())
			{
				ClientConfig = base.Config,
				OriginalRequest = request,
				Action = callbackHelper,
				AsyncOptions = options,
				IsAsync = true
			}, new AsyncResponseContext());
			httpRequest.SetRequestHeaders(dictionary);
			asyncExecutionContext.RuntimeState = httpRequest;
			asyncExecutionContext.ResponseContext.AsyncResult = new RuntimeAsyncResult(asyncExecutionContext.RequestContext.Callback, asyncExecutionContext.RequestContext.State);
			asyncExecutionContext.ResponseContext.AsyncResult.AsyncOptions = asyncExecutionContext.RequestContext.AsyncOptions;
			asyncExecutionContext.ResponseContext.AsyncResult.Action = asyncExecutionContext.RequestContext.Action;
			asyncExecutionContext.ResponseContext.AsyncResult.Request = asyncExecutionContext.RequestContext.OriginalRequest;
			httpRequest.BeginGetResponse(ProcessPostResponse, asyncExecutionContext);
		}

		private void ProcessPostResponse(IAsyncResult result)
		{
			IAsyncExecutionContext asyncExecutionContext = null;
			IHttpRequest<string> httpRequest = null;
			try
			{
				asyncExecutionContext = result.AsyncState as IAsyncExecutionContext;
				httpRequest = asyncExecutionContext.RuntimeState as IHttpRequest<string>;
				IWebResponseData httpResponse = httpRequest.EndGetResponse(result);
				asyncExecutionContext.ResponseContext.HttpResponse = httpResponse;
			}
			catch (Exception exception)
			{
				asyncExecutionContext.ResponseContext.AsyncResult.Exception = exception;
			}
			finally
			{
				httpRequest.Dispose();
			}
			PostResponseHelper(result);
		}

		private void PostResponseHelper(IAsyncResult result)
		{
			IAsyncExecutionContext asyncExecutionContext = result.AsyncState as IAsyncExecutionContext;
			IWebResponseData httpResponse = asyncExecutionContext.ResponseContext.HttpResponse;
			RuntimeAsyncResult asyncResult = asyncExecutionContext.ResponseContext.AsyncResult;
			if (asyncExecutionContext.ResponseContext.AsyncResult.Exception == null)
			{
				PostObjectResponse postObjectResponse = new PostObjectResponse();
				postObjectResponse.HttpStatusCode = httpResponse.StatusCode;
				postObjectResponse.ContentLength = httpResponse.ContentLength;
				if (httpResponse.IsHeaderPresent("x-amz-request-id"))
				{
					postObjectResponse.RequestId = httpResponse.GetHeaderValue("x-amz-request-id");
				}
				if (httpResponse.IsHeaderPresent("x-amz-id-2"))
				{
					postObjectResponse.HostId = httpResponse.GetHeaderValue("x-amz-id-2");
				}
				if (httpResponse.IsHeaderPresent("x-amz-version-id"))
				{
					postObjectResponse.VersionId = httpResponse.GetHeaderValue("x-amz-version-id");
				}
				PostObjectRequest request = asyncExecutionContext.RequestContext.OriginalRequest as PostObjectRequest;
				asyncResult.Request = request;
				asyncResult.Response = postObjectResponse;
			}
			asyncResult.Exception = asyncExecutionContext.ResponseContext.AsyncResult.Exception;
			asyncResult.Action = asyncExecutionContext.RequestContext.Action;
			asyncResult.InvokeCallback();
		}

		public AmazonS3Client(AWSCredentials credentials)
			: this(credentials, new AmazonS3Config())
		{
		}

		public AmazonS3Client(AWSCredentials credentials, RegionEndpoint region)
			: this(credentials, new AmazonS3Config
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonS3Client(AWSCredentials credentials, AmazonS3Config clientConfig)
			: base(credentials, clientConfig)
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonS3Config())
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonS3Config
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, AmazonS3Config clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, clientConfig)
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonS3Config())
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonS3Config
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonS3Client(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonS3Config clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
		{
		}

		protected override AbstractAWSSigner CreateSigner()
		{
			return new S3Signer();
		}

		protected override void CustomizeRuntimePipeline(RuntimePipeline pipeline)
		{
			pipeline.AddHandlerBefore<Marshaller>(new AmazonS3PreMarshallHandler());
			pipeline.AddHandlerAfter<Marshaller>(new AmazonS3PostMarshallHandler());
			pipeline.AddHandlerBefore<EndpointResolver>(new AmazonS3HttpDeleteHandler());
			pipeline.AddHandlerAfter<EndpointResolver>(new AmazonS3KmsHandler());
			pipeline.AddHandlerBefore<Unmarshaller>(new AmazonS3ResponseHandler());
			pipeline.AddHandlerAfter<ErrorCallbackHandler>(new AmazonS3ExceptionHandler());
			pipeline.AddHandlerAfter<Unmarshaller>(new AmazonS3RedirectHandler());
			pipeline.ReplaceHandler<RetryHandler>(new RetryHandler(new AmazonS3RetryPolicy(base.Config)));
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		public virtual void AbortMultipartUploadAsync(string bucketName, string key, string uploadId, AmazonServiceCallback<AbortMultipartUploadRequest, AbortMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			AbortMultipartUploadRequest abortMultipartUploadRequest = new AbortMultipartUploadRequest();
			abortMultipartUploadRequest.BucketName = bucketName;
			abortMultipartUploadRequest.Key = key;
			abortMultipartUploadRequest.UploadId = uploadId;
			AbortMultipartUploadAsync(abortMultipartUploadRequest, callback, options);
		}

		public virtual void AbortMultipartUploadAsync(AbortMultipartUploadRequest request, AmazonServiceCallback<AbortMultipartUploadRequest, AbortMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("AbortMultipartUpload is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			AbortMultipartUploadRequestMarshaller instance = AbortMultipartUploadRequestMarshaller.Instance;
			AbortMultipartUploadResponseUnmarshaller instance2 = AbortMultipartUploadResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AbortMultipartUploadRequest, AbortMultipartUploadResponse> responseObject = new AmazonServiceResult<AbortMultipartUploadRequest, AbortMultipartUploadResponse>((AbortMultipartUploadRequest)req, (AbortMultipartUploadResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void CompleteMultipartUploadAsync(CompleteMultipartUploadRequest request, AmazonServiceCallback<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("CompleteMultipartUpload is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			CompleteMultipartUploadRequestMarshaller instance = CompleteMultipartUploadRequestMarshaller.Instance;
			CompleteMultipartUploadResponseUnmarshaller instance2 = CompleteMultipartUploadResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse> responseObject = new AmazonServiceResult<CompleteMultipartUploadRequest, CompleteMultipartUploadResponse>((CompleteMultipartUploadRequest)req, (CompleteMultipartUploadResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void CopyObjectAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null)
		{
			CopyObjectRequest copyObjectRequest = new CopyObjectRequest();
			copyObjectRequest.SourceBucket = sourceBucket;
			copyObjectRequest.SourceKey = sourceKey;
			copyObjectRequest.DestinationBucket = destinationBucket;
			copyObjectRequest.DestinationKey = destinationKey;
			CopyObjectAsync(copyObjectRequest, callback, options);
		}

		public virtual void CopyObjectAsync(string sourceBucket, string sourceKey, string sourceVersionId, string destinationBucket, string destinationKey, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null)
		{
			CopyObjectRequest copyObjectRequest = new CopyObjectRequest();
			copyObjectRequest.SourceBucket = sourceBucket;
			copyObjectRequest.SourceKey = sourceKey;
			copyObjectRequest.SourceVersionId = sourceVersionId;
			copyObjectRequest.DestinationBucket = destinationBucket;
			copyObjectRequest.DestinationKey = destinationKey;
			CopyObjectAsync(copyObjectRequest, callback, options);
		}

		public virtual void CopyObjectAsync(CopyObjectRequest request, AmazonServiceCallback<CopyObjectRequest, CopyObjectResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("CopyObject is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			CopyObjectRequestMarshaller instance = CopyObjectRequestMarshaller.Instance;
			CopyObjectResponseUnmarshaller instance2 = CopyObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CopyObjectRequest, CopyObjectResponse> responseObject = new AmazonServiceResult<CopyObjectRequest, CopyObjectResponse>((CopyObjectRequest)req, (CopyObjectResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void CopyPartAsync(string sourceBucket, string sourceKey, string destinationBucket, string destinationKey, string uploadId, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null)
		{
			CopyPartRequest copyPartRequest = new CopyPartRequest();
			copyPartRequest.SourceBucket = sourceBucket;
			copyPartRequest.SourceKey = sourceKey;
			copyPartRequest.DestinationBucket = destinationBucket;
			copyPartRequest.DestinationKey = destinationKey;
			copyPartRequest.UploadId = uploadId;
			CopyPartAsync(copyPartRequest, callback, options);
		}

		public virtual void CopyPartAsync(string sourceBucket, string sourceKey, string sourceVersionId, string destinationBucket, string destinationKey, string uploadId, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null)
		{
			CopyPartRequest copyPartRequest = new CopyPartRequest();
			copyPartRequest.SourceBucket = sourceBucket;
			copyPartRequest.SourceKey = sourceKey;
			copyPartRequest.SourceVersionId = sourceVersionId;
			copyPartRequest.DestinationBucket = destinationBucket;
			copyPartRequest.DestinationKey = destinationKey;
			copyPartRequest.UploadId = uploadId;
			CopyPartAsync(copyPartRequest, callback, options);
		}

		public virtual void CopyPartAsync(CopyPartRequest request, AmazonServiceCallback<CopyPartRequest, CopyPartResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("CopyPart is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			CopyPartRequestMarshaller instance = CopyPartRequestMarshaller.Instance;
			CopyPartResponseUnmarshaller instance2 = CopyPartResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CopyPartRequest, CopyPartResponse> responseObject = new AmazonServiceResult<CopyPartRequest, CopyPartResponse>((CopyPartRequest)req, (CopyPartResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketAsync(string bucketName, AmazonServiceCallback<DeleteBucketRequest, DeleteBucketResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketRequest deleteBucketRequest = new DeleteBucketRequest();
			deleteBucketRequest.BucketName = bucketName;
			DeleteBucketAsync(deleteBucketRequest, callback, options);
		}

		public virtual void DeleteBucketAsync(DeleteBucketRequest request, AmazonServiceCallback<DeleteBucketRequest, DeleteBucketResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucket is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketRequestMarshaller instance = DeleteBucketRequestMarshaller.Instance;
			DeleteBucketResponseUnmarshaller instance2 = DeleteBucketResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketRequest, DeleteBucketResponse> responseObject = new AmazonServiceResult<DeleteBucketRequest, DeleteBucketResponse>((DeleteBucketRequest)req, (DeleteBucketResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketAnalyticsConfigurationAsync(DeleteBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketAnalyticsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketAnalyticsConfigurationRequestMarshaller instance = DeleteBucketAnalyticsConfigurationRequestMarshaller.Instance;
			DeleteBucketAnalyticsConfigurationResponseUnmarshaller instance2 = DeleteBucketAnalyticsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse> responseObject = new AmazonServiceResult<DeleteBucketAnalyticsConfigurationRequest, DeleteBucketAnalyticsConfigurationResponse>((DeleteBucketAnalyticsConfigurationRequest)req, (DeleteBucketAnalyticsConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketEncryptionAsync(DeleteBucketEncryptionRequest request, AmazonServiceCallback<DeleteBucketEncryptionRequest, DeleteBucketEncryptionResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketEncryption is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketEncryptionRequestMarshaller instance = DeleteBucketEncryptionRequestMarshaller.Instance;
			DeleteBucketEncryptionResponseUnmarshaller instance2 = DeleteBucketEncryptionResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketEncryptionRequest, DeleteBucketEncryptionResponse> responseObject = new AmazonServiceResult<DeleteBucketEncryptionRequest, DeleteBucketEncryptionResponse>((DeleteBucketEncryptionRequest)req, (DeleteBucketEncryptionResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketInventoryConfigurationAsync(DeleteBucketInventoryConfigurationRequest request, AmazonServiceCallback<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketInventoryConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketInventoryConfigurationRequestMarshaller instance = DeleteBucketInventoryConfigurationRequestMarshaller.Instance;
			DeleteBucketInventoryConfigurationResponseUnmarshaller instance2 = DeleteBucketInventoryConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse> responseObject = new AmazonServiceResult<DeleteBucketInventoryConfigurationRequest, DeleteBucketInventoryConfigurationResponse>((DeleteBucketInventoryConfigurationRequest)req, (DeleteBucketInventoryConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketMetricsConfigurationAsync(DeleteBucketMetricsConfigurationRequest request, AmazonServiceCallback<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketMetricsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketMetricsConfigurationRequestMarshaller instance = DeleteBucketMetricsConfigurationRequestMarshaller.Instance;
			DeleteBucketMetricsConfigurationResponseUnmarshaller instance2 = DeleteBucketMetricsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse> responseObject = new AmazonServiceResult<DeleteBucketMetricsConfigurationRequest, DeleteBucketMetricsConfigurationResponse>((DeleteBucketMetricsConfigurationRequest)req, (DeleteBucketMetricsConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketPolicyAsync(string bucketName, AmazonServiceCallback<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketPolicyRequest deleteBucketPolicyRequest = new DeleteBucketPolicyRequest();
			deleteBucketPolicyRequest.BucketName = bucketName;
			DeleteBucketPolicyAsync(deleteBucketPolicyRequest, callback, options);
		}

		public virtual void DeleteBucketPolicyAsync(DeleteBucketPolicyRequest request, AmazonServiceCallback<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketPolicy is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketPolicyRequestMarshaller instance = DeleteBucketPolicyRequestMarshaller.Instance;
			DeleteBucketPolicyResponseUnmarshaller instance2 = DeleteBucketPolicyResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse> responseObject = new AmazonServiceResult<DeleteBucketPolicyRequest, DeleteBucketPolicyResponse>((DeleteBucketPolicyRequest)req, (DeleteBucketPolicyResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketReplicationAsync(DeleteBucketReplicationRequest request, AmazonServiceCallback<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketReplication is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketReplicationRequestMarshaller instance = DeleteBucketReplicationRequestMarshaller.Instance;
			DeleteBucketReplicationResponseUnmarshaller instance2 = DeleteBucketReplicationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse> responseObject = new AmazonServiceResult<DeleteBucketReplicationRequest, DeleteBucketReplicationResponse>((DeleteBucketReplicationRequest)req, (DeleteBucketReplicationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketTaggingAsync(string bucketName, AmazonServiceCallback<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketTaggingRequest deleteBucketTaggingRequest = new DeleteBucketTaggingRequest();
			deleteBucketTaggingRequest.BucketName = bucketName;
			DeleteBucketTaggingAsync(deleteBucketTaggingRequest, callback, options);
		}

		public virtual void DeleteBucketTaggingAsync(DeleteBucketTaggingRequest request, AmazonServiceCallback<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketTaggingRequestMarshaller instance = DeleteBucketTaggingRequestMarshaller.Instance;
			DeleteBucketTaggingResponseUnmarshaller instance2 = DeleteBucketTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse> responseObject = new AmazonServiceResult<DeleteBucketTaggingRequest, DeleteBucketTaggingResponse>((DeleteBucketTaggingRequest)req, (DeleteBucketTaggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteBucketWebsiteAsync(string bucketName, AmazonServiceCallback<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			DeleteBucketWebsiteRequest deleteBucketWebsiteRequest = new DeleteBucketWebsiteRequest();
			deleteBucketWebsiteRequest.BucketName = bucketName;
			DeleteBucketWebsiteAsync(deleteBucketWebsiteRequest, callback, options);
		}

		public virtual void DeleteBucketWebsiteAsync(DeleteBucketWebsiteRequest request, AmazonServiceCallback<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteBucketWebsite is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteBucketWebsiteRequestMarshaller instance = DeleteBucketWebsiteRequestMarshaller.Instance;
			DeleteBucketWebsiteResponseUnmarshaller instance2 = DeleteBucketWebsiteResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse> responseObject = new AmazonServiceResult<DeleteBucketWebsiteRequest, DeleteBucketWebsiteResponse>((DeleteBucketWebsiteRequest)req, (DeleteBucketWebsiteResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteCORSConfigurationAsync(string bucketName, AmazonServiceCallback<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			DeleteCORSConfigurationRequest deleteCORSConfigurationRequest = new DeleteCORSConfigurationRequest();
			deleteCORSConfigurationRequest.BucketName = bucketName;
			DeleteCORSConfigurationAsync(deleteCORSConfigurationRequest, callback, options);
		}

		public virtual void DeleteCORSConfigurationAsync(DeleteCORSConfigurationRequest request, AmazonServiceCallback<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteCORSConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteCORSConfigurationRequestMarshaller instance = DeleteCORSConfigurationRequestMarshaller.Instance;
			DeleteCORSConfigurationResponseUnmarshaller instance2 = DeleteCORSConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse> responseObject = new AmazonServiceResult<DeleteCORSConfigurationRequest, DeleteCORSConfigurationResponse>((DeleteCORSConfigurationRequest)req, (DeleteCORSConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteLifecycleConfigurationAsync(string bucketName, AmazonServiceCallback<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			DeleteLifecycleConfigurationRequest deleteLifecycleConfigurationRequest = new DeleteLifecycleConfigurationRequest();
			deleteLifecycleConfigurationRequest.BucketName = bucketName;
			DeleteLifecycleConfigurationAsync(deleteLifecycleConfigurationRequest, callback, options);
		}

		public virtual void DeleteLifecycleConfigurationAsync(DeleteLifecycleConfigurationRequest request, AmazonServiceCallback<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteLifecycleConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteLifecycleConfigurationRequestMarshaller instance = DeleteLifecycleConfigurationRequestMarshaller.Instance;
			DeleteLifecycleConfigurationResponseUnmarshaller instance2 = DeleteLifecycleConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse> responseObject = new AmazonServiceResult<DeleteLifecycleConfigurationRequest, DeleteLifecycleConfigurationResponse>((DeleteLifecycleConfigurationRequest)req, (DeleteLifecycleConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteObjectAsync(string bucketName, string key, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null)
		{
			DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest();
			deleteObjectRequest.BucketName = bucketName;
			deleteObjectRequest.Key = key;
			DeleteObjectAsync(deleteObjectRequest, callback, options);
		}

		public virtual void DeleteObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null)
		{
			DeleteObjectRequest deleteObjectRequest = new DeleteObjectRequest();
			deleteObjectRequest.BucketName = bucketName;
			deleteObjectRequest.Key = key;
			deleteObjectRequest.VersionId = versionId;
			DeleteObjectAsync(deleteObjectRequest, callback, options);
		}

		public virtual void DeleteObjectAsync(DeleteObjectRequest request, AmazonServiceCallback<DeleteObjectRequest, DeleteObjectResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteObject is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteObjectRequestMarshaller instance = DeleteObjectRequestMarshaller.Instance;
			DeleteObjectResponseUnmarshaller instance2 = DeleteObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteObjectRequest, DeleteObjectResponse> responseObject = new AmazonServiceResult<DeleteObjectRequest, DeleteObjectResponse>((DeleteObjectRequest)req, (DeleteObjectResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteObjectsAsync(DeleteObjectsRequest request, AmazonServiceCallback<DeleteObjectsRequest, DeleteObjectsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteObjectsRequestMarshaller instance = DeleteObjectsRequestMarshaller.Instance;
			DeleteObjectsResponseUnmarshaller instance2 = DeleteObjectsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteObjectsRequest, DeleteObjectsResponse> responseObject = new AmazonServiceResult<DeleteObjectsRequest, DeleteObjectsResponse>((DeleteObjectsRequest)req, (DeleteObjectsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void DeleteObjectTaggingAsync(DeleteObjectTaggingRequest request, AmazonServiceCallback<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("DeleteObjectTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteObjectTaggingRequestMarshaller instance = DeleteObjectTaggingRequestMarshaller.Instance;
			DeleteObjectTaggingResponseUnmarshaller instance2 = DeleteObjectTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse> responseObject = new AmazonServiceResult<DeleteObjectTaggingRequest, DeleteObjectTaggingResponse>((DeleteObjectTaggingRequest)req, (DeleteObjectTaggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetACLAsync(string bucketName, AmazonServiceCallback<GetACLRequest, GetACLResponse> callback, AsyncOptions options = null)
		{
			GetACLRequest getACLRequest = new GetACLRequest();
			getACLRequest.BucketName = bucketName;
			GetACLAsync(getACLRequest, callback, options);
		}

		public virtual void GetACLAsync(GetACLRequest request, AmazonServiceCallback<GetACLRequest, GetACLResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetACLRequestMarshaller instance = GetACLRequestMarshaller.Instance;
			GetACLResponseUnmarshaller instance2 = GetACLResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetACLRequest, GetACLResponse> responseObject = new AmazonServiceResult<GetACLRequest, GetACLResponse>((GetACLRequest)req, (GetACLResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketAccelerateConfigurationAsync(string bucketName, AmazonServiceCallback<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null)
		{
			GetBucketAccelerateConfigurationRequest getBucketAccelerateConfigurationRequest = new GetBucketAccelerateConfigurationRequest();
			getBucketAccelerateConfigurationRequest.BucketName = bucketName;
			GetBucketAccelerateConfigurationAsync(getBucketAccelerateConfigurationRequest, callback, options);
		}

		public virtual void GetBucketAccelerateConfigurationAsync(GetBucketAccelerateConfigurationRequest request, AmazonServiceCallback<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketAccelerateConfigurationRequestMarshaller instance = GetBucketAccelerateConfigurationRequestMarshaller.Instance;
			GetBucketAccelerateConfigurationResponseUnmarshaller instance2 = GetBucketAccelerateConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse> responseObject = new AmazonServiceResult<GetBucketAccelerateConfigurationRequest, GetBucketAccelerateConfigurationResponse>((GetBucketAccelerateConfigurationRequest)req, (GetBucketAccelerateConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketAnalyticsConfigurationAsync(GetBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketAnalyticsConfigurationRequestMarshaller instance = GetBucketAnalyticsConfigurationRequestMarshaller.Instance;
			GetBucketAnalyticsConfigurationResponseUnmarshaller instance2 = GetBucketAnalyticsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse> responseObject = new AmazonServiceResult<GetBucketAnalyticsConfigurationRequest, GetBucketAnalyticsConfigurationResponse>((GetBucketAnalyticsConfigurationRequest)req, (GetBucketAnalyticsConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketEncryptionAsync(GetBucketEncryptionRequest request, AmazonServiceCallback<GetBucketEncryptionRequest, GetBucketEncryptionResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketEncryptionRequestMarshaller instance = GetBucketEncryptionRequestMarshaller.Instance;
			GetBucketEncryptionResponseUnmarshaller instance2 = GetBucketEncryptionResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketEncryptionRequest, GetBucketEncryptionResponse> responseObject = new AmazonServiceResult<GetBucketEncryptionRequest, GetBucketEncryptionResponse>((GetBucketEncryptionRequest)req, (GetBucketEncryptionResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketInventoryConfigurationAsync(GetBucketInventoryConfigurationRequest request, AmazonServiceCallback<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketInventoryConfigurationRequestMarshaller instance = GetBucketInventoryConfigurationRequestMarshaller.Instance;
			GetBucketInventoryConfigurationResponseUnmarshaller instance2 = GetBucketInventoryConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse> responseObject = new AmazonServiceResult<GetBucketInventoryConfigurationRequest, GetBucketInventoryConfigurationResponse>((GetBucketInventoryConfigurationRequest)req, (GetBucketInventoryConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketLocationAsync(string bucketName, AmazonServiceCallback<GetBucketLocationRequest, GetBucketLocationResponse> callback, AsyncOptions options = null)
		{
			GetBucketLocationRequest getBucketLocationRequest = new GetBucketLocationRequest();
			getBucketLocationRequest.BucketName = bucketName;
			GetBucketLocationAsync(getBucketLocationRequest, callback, options);
		}

		public virtual void GetBucketLocationAsync(GetBucketLocationRequest request, AmazonServiceCallback<GetBucketLocationRequest, GetBucketLocationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketLocationRequestMarshaller instance = GetBucketLocationRequestMarshaller.Instance;
			GetBucketLocationResponseUnmarshaller instance2 = GetBucketLocationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketLocationRequest, GetBucketLocationResponse> responseObject = new AmazonServiceResult<GetBucketLocationRequest, GetBucketLocationResponse>((GetBucketLocationRequest)req, (GetBucketLocationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketLoggingAsync(string bucketName, AmazonServiceCallback<GetBucketLoggingRequest, GetBucketLoggingResponse> callback, AsyncOptions options = null)
		{
			GetBucketLoggingRequest getBucketLoggingRequest = new GetBucketLoggingRequest();
			getBucketLoggingRequest.BucketName = bucketName;
			GetBucketLoggingAsync(getBucketLoggingRequest, callback, options);
		}

		public virtual void GetBucketLoggingAsync(GetBucketLoggingRequest request, AmazonServiceCallback<GetBucketLoggingRequest, GetBucketLoggingResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketLoggingRequestMarshaller instance = GetBucketLoggingRequestMarshaller.Instance;
			GetBucketLoggingResponseUnmarshaller instance2 = GetBucketLoggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketLoggingRequest, GetBucketLoggingResponse> responseObject = new AmazonServiceResult<GetBucketLoggingRequest, GetBucketLoggingResponse>((GetBucketLoggingRequest)req, (GetBucketLoggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketMetricsConfigurationAsync(GetBucketMetricsConfigurationRequest request, AmazonServiceCallback<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketMetricsConfigurationRequestMarshaller instance = GetBucketMetricsConfigurationRequestMarshaller.Instance;
			GetBucketMetricsConfigurationResponseUnmarshaller instance2 = GetBucketMetricsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse> responseObject = new AmazonServiceResult<GetBucketMetricsConfigurationRequest, GetBucketMetricsConfigurationResponse>((GetBucketMetricsConfigurationRequest)req, (GetBucketMetricsConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketNotificationAsync(string bucketName, AmazonServiceCallback<GetBucketNotificationRequest, GetBucketNotificationResponse> callback, AsyncOptions options = null)
		{
			GetBucketNotificationRequest getBucketNotificationRequest = new GetBucketNotificationRequest();
			getBucketNotificationRequest.BucketName = bucketName;
			GetBucketNotificationAsync(getBucketNotificationRequest, callback, options);
		}

		public virtual void GetBucketNotificationAsync(GetBucketNotificationRequest request, AmazonServiceCallback<GetBucketNotificationRequest, GetBucketNotificationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketNotificationRequestMarshaller instance = GetBucketNotificationRequestMarshaller.Instance;
			GetBucketNotificationResponseUnmarshaller instance2 = GetBucketNotificationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketNotificationRequest, GetBucketNotificationResponse> responseObject = new AmazonServiceResult<GetBucketNotificationRequest, GetBucketNotificationResponse>((GetBucketNotificationRequest)req, (GetBucketNotificationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketPolicyAsync(string bucketName, AmazonServiceCallback<GetBucketPolicyRequest, GetBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			GetBucketPolicyRequest getBucketPolicyRequest = new GetBucketPolicyRequest();
			getBucketPolicyRequest.BucketName = bucketName;
			GetBucketPolicyAsync(getBucketPolicyRequest, callback, options);
		}

		public virtual void GetBucketPolicyAsync(GetBucketPolicyRequest request, AmazonServiceCallback<GetBucketPolicyRequest, GetBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketPolicyRequestMarshaller instance = GetBucketPolicyRequestMarshaller.Instance;
			GetBucketPolicyResponseUnmarshaller instance2 = GetBucketPolicyResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketPolicyRequest, GetBucketPolicyResponse> responseObject = new AmazonServiceResult<GetBucketPolicyRequest, GetBucketPolicyResponse>((GetBucketPolicyRequest)req, (GetBucketPolicyResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketReplicationAsync(GetBucketReplicationRequest request, AmazonServiceCallback<GetBucketReplicationRequest, GetBucketReplicationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("GetBucketReplication is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketReplicationRequestMarshaller instance = GetBucketReplicationRequestMarshaller.Instance;
			GetBucketReplicationResponseUnmarshaller instance2 = GetBucketReplicationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketReplicationRequest, GetBucketReplicationResponse> responseObject = new AmazonServiceResult<GetBucketReplicationRequest, GetBucketReplicationResponse>((GetBucketReplicationRequest)req, (GetBucketReplicationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketRequestPaymentAsync(string bucketName, AmazonServiceCallback<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			GetBucketRequestPaymentRequest getBucketRequestPaymentRequest = new GetBucketRequestPaymentRequest();
			getBucketRequestPaymentRequest.BucketName = bucketName;
			GetBucketRequestPaymentAsync(getBucketRequestPaymentRequest, callback, options);
		}

		public virtual void GetBucketRequestPaymentAsync(GetBucketRequestPaymentRequest request, AmazonServiceCallback<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketRequestPaymentRequestMarshaller instance = GetBucketRequestPaymentRequestMarshaller.Instance;
			GetBucketRequestPaymentResponseUnmarshaller instance2 = GetBucketRequestPaymentResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse> responseObject = new AmazonServiceResult<GetBucketRequestPaymentRequest, GetBucketRequestPaymentResponse>((GetBucketRequestPaymentRequest)req, (GetBucketRequestPaymentResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketTaggingAsync(GetBucketTaggingRequest request, AmazonServiceCallback<GetBucketTaggingRequest, GetBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketTaggingRequestMarshaller instance = GetBucketTaggingRequestMarshaller.Instance;
			GetBucketTaggingResponseUnmarshaller instance2 = GetBucketTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketTaggingRequest, GetBucketTaggingResponse> responseObject = new AmazonServiceResult<GetBucketTaggingRequest, GetBucketTaggingResponse>((GetBucketTaggingRequest)req, (GetBucketTaggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketVersioningAsync(string bucketName, AmazonServiceCallback<GetBucketVersioningRequest, GetBucketVersioningResponse> callback, AsyncOptions options = null)
		{
			GetBucketVersioningRequest getBucketVersioningRequest = new GetBucketVersioningRequest();
			getBucketVersioningRequest.BucketName = bucketName;
			GetBucketVersioningAsync(getBucketVersioningRequest, callback, options);
		}

		public virtual void GetBucketVersioningAsync(GetBucketVersioningRequest request, AmazonServiceCallback<GetBucketVersioningRequest, GetBucketVersioningResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketVersioningRequestMarshaller instance = GetBucketVersioningRequestMarshaller.Instance;
			GetBucketVersioningResponseUnmarshaller instance2 = GetBucketVersioningResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketVersioningRequest, GetBucketVersioningResponse> responseObject = new AmazonServiceResult<GetBucketVersioningRequest, GetBucketVersioningResponse>((GetBucketVersioningRequest)req, (GetBucketVersioningResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetBucketWebsiteAsync(string bucketName, AmazonServiceCallback<GetBucketWebsiteRequest, GetBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			GetBucketWebsiteRequest getBucketWebsiteRequest = new GetBucketWebsiteRequest();
			getBucketWebsiteRequest.BucketName = bucketName;
			GetBucketWebsiteAsync(getBucketWebsiteRequest, callback, options);
		}

		public virtual void GetBucketWebsiteAsync(GetBucketWebsiteRequest request, AmazonServiceCallback<GetBucketWebsiteRequest, GetBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetBucketWebsiteRequestMarshaller instance = GetBucketWebsiteRequestMarshaller.Instance;
			GetBucketWebsiteResponseUnmarshaller instance2 = GetBucketWebsiteResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetBucketWebsiteRequest, GetBucketWebsiteResponse> responseObject = new AmazonServiceResult<GetBucketWebsiteRequest, GetBucketWebsiteResponse>((GetBucketWebsiteRequest)req, (GetBucketWebsiteResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetCORSConfigurationAsync(string bucketName, AmazonServiceCallback<GetCORSConfigurationRequest, GetCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			GetCORSConfigurationRequest getCORSConfigurationRequest = new GetCORSConfigurationRequest();
			getCORSConfigurationRequest.BucketName = bucketName;
			GetCORSConfigurationAsync(getCORSConfigurationRequest, callback, options);
		}

		public virtual void GetCORSConfigurationAsync(GetCORSConfigurationRequest request, AmazonServiceCallback<GetCORSConfigurationRequest, GetCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetCORSConfigurationRequestMarshaller instance = GetCORSConfigurationRequestMarshaller.Instance;
			GetCORSConfigurationResponseUnmarshaller instance2 = GetCORSConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetCORSConfigurationRequest, GetCORSConfigurationResponse> responseObject = new AmazonServiceResult<GetCORSConfigurationRequest, GetCORSConfigurationResponse>((GetCORSConfigurationRequest)req, (GetCORSConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetLifecycleConfigurationAsync(string bucketName, AmazonServiceCallback<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			GetLifecycleConfigurationRequest getLifecycleConfigurationRequest = new GetLifecycleConfigurationRequest();
			getLifecycleConfigurationRequest.BucketName = bucketName;
			GetLifecycleConfigurationAsync(getLifecycleConfigurationRequest, callback, options);
		}

		public virtual void GetLifecycleConfigurationAsync(GetLifecycleConfigurationRequest request, AmazonServiceCallback<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetLifecycleConfigurationRequestMarshaller instance = GetLifecycleConfigurationRequestMarshaller.Instance;
			GetLifecycleConfigurationResponseUnmarshaller instance2 = GetLifecycleConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse> responseObject = new AmazonServiceResult<GetLifecycleConfigurationRequest, GetLifecycleConfigurationResponse>((GetLifecycleConfigurationRequest)req, (GetLifecycleConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetObjectAsync(string bucketName, string key, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null)
		{
			GetObjectRequest getObjectRequest = new GetObjectRequest();
			getObjectRequest.BucketName = bucketName;
			getObjectRequest.Key = key;
			GetObjectAsync(getObjectRequest, callback, options);
		}

		public virtual void GetObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null)
		{
			GetObjectRequest getObjectRequest = new GetObjectRequest();
			getObjectRequest.BucketName = bucketName;
			getObjectRequest.Key = key;
			getObjectRequest.VersionId = versionId;
			GetObjectAsync(getObjectRequest, callback, options);
		}

		public virtual void GetObjectAsync(GetObjectRequest request, AmazonServiceCallback<GetObjectRequest, GetObjectResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetObjectRequestMarshaller instance = GetObjectRequestMarshaller.Instance;
			GetObjectResponseUnmarshaller instance2 = GetObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectRequest, GetObjectResponse> responseObject = new AmazonServiceResult<GetObjectRequest, GetObjectResponse>((GetObjectRequest)req, (GetObjectResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetObjectMetadataAsync(string bucketName, string key, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null)
		{
			GetObjectMetadataRequest getObjectMetadataRequest = new GetObjectMetadataRequest();
			getObjectMetadataRequest.BucketName = bucketName;
			getObjectMetadataRequest.Key = key;
			GetObjectMetadataAsync(getObjectMetadataRequest, callback, options);
		}

		public virtual void GetObjectMetadataAsync(string bucketName, string key, string versionId, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null)
		{
			GetObjectMetadataRequest getObjectMetadataRequest = new GetObjectMetadataRequest();
			getObjectMetadataRequest.BucketName = bucketName;
			getObjectMetadataRequest.Key = key;
			getObjectMetadataRequest.VersionId = versionId;
			GetObjectMetadataAsync(getObjectMetadataRequest, callback, options);
		}

		public virtual void GetObjectMetadataAsync(GetObjectMetadataRequest request, AmazonServiceCallback<GetObjectMetadataRequest, GetObjectMetadataResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("GetObjectMetadata is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			GetObjectMetadataRequestMarshaller instance = GetObjectMetadataRequestMarshaller.Instance;
			GetObjectMetadataResponseUnmarshaller instance2 = GetObjectMetadataResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectMetadataRequest, GetObjectMetadataResponse> responseObject = new AmazonServiceResult<GetObjectMetadataRequest, GetObjectMetadataResponse>((GetObjectMetadataRequest)req, (GetObjectMetadataResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetObjectTaggingAsync(GetObjectTaggingRequest request, AmazonServiceCallback<GetObjectTaggingRequest, GetObjectTaggingResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetObjectTaggingRequestMarshaller instance = GetObjectTaggingRequestMarshaller.Instance;
			GetObjectTaggingResponseUnmarshaller instance2 = GetObjectTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectTaggingRequest, GetObjectTaggingResponse> responseObject = new AmazonServiceResult<GetObjectTaggingRequest, GetObjectTaggingResponse>((GetObjectTaggingRequest)req, (GetObjectTaggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void GetObjectTorrentAsync(string bucketName, string key, AmazonServiceCallback<GetObjectTorrentRequest, GetObjectTorrentResponse> callback, AsyncOptions options = null)
		{
			GetObjectTorrentRequest getObjectTorrentRequest = new GetObjectTorrentRequest();
			getObjectTorrentRequest.BucketName = bucketName;
			getObjectTorrentRequest.Key = key;
			GetObjectTorrentAsync(getObjectTorrentRequest, callback, options);
		}

		public virtual void GetObjectTorrentAsync(GetObjectTorrentRequest request, AmazonServiceCallback<GetObjectTorrentRequest, GetObjectTorrentResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetObjectTorrentRequestMarshaller instance = GetObjectTorrentRequestMarshaller.Instance;
			GetObjectTorrentResponseUnmarshaller instance2 = GetObjectTorrentResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetObjectTorrentRequest, GetObjectTorrentResponse> responseObject = new AmazonServiceResult<GetObjectTorrentRequest, GetObjectTorrentResponse>((GetObjectTorrentRequest)req, (GetObjectTorrentResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual void HeadBucketAsync(HeadBucketRequest request, AmazonServiceCallback<HeadBucketRequest, HeadBucketResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("HeadBucket is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			HeadBucketRequestMarshaller instance = HeadBucketRequestMarshaller.Instance;
			HeadBucketResponseUnmarshaller instance2 = HeadBucketResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<HeadBucketRequest, HeadBucketResponse> responseObject = new AmazonServiceResult<HeadBucketRequest, HeadBucketResponse>((HeadBucketRequest)req, (HeadBucketResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void InitiateMultipartUploadAsync(string bucketName, string key, AmazonServiceCallback<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			InitiateMultipartUploadRequest initiateMultipartUploadRequest = new InitiateMultipartUploadRequest();
			initiateMultipartUploadRequest.BucketName = bucketName;
			initiateMultipartUploadRequest.Key = key;
			InitiateMultipartUploadAsync(initiateMultipartUploadRequest, callback, options);
		}

		public virtual void InitiateMultipartUploadAsync(InitiateMultipartUploadRequest request, AmazonServiceCallback<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("InitiateMultipartUpload is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			InitiateMultipartUploadRequestMarshaller instance = InitiateMultipartUploadRequestMarshaller.Instance;
			InitiateMultipartUploadResponseUnmarshaller instance2 = InitiateMultipartUploadResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse> responseObject = new AmazonServiceResult<InitiateMultipartUploadRequest, InitiateMultipartUploadResponse>((InitiateMultipartUploadRequest)req, (InitiateMultipartUploadResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListBucketAnalyticsConfigurationsAsync(ListBucketAnalyticsConfigurationsRequest request, AmazonServiceCallback<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListBucketAnalyticsConfigurationsRequestMarshaller instance = ListBucketAnalyticsConfigurationsRequestMarshaller.Instance;
			ListBucketAnalyticsConfigurationsResponseUnmarshaller instance2 = ListBucketAnalyticsConfigurationsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse> responseObject = new AmazonServiceResult<ListBucketAnalyticsConfigurationsRequest, ListBucketAnalyticsConfigurationsResponse>((ListBucketAnalyticsConfigurationsRequest)req, (ListBucketAnalyticsConfigurationsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListBucketInventoryConfigurationsAsync(ListBucketInventoryConfigurationsRequest request, AmazonServiceCallback<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListBucketInventoryConfigurationsRequestMarshaller instance = ListBucketInventoryConfigurationsRequestMarshaller.Instance;
			ListBucketInventoryConfigurationsResponseUnmarshaller instance2 = ListBucketInventoryConfigurationsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse> responseObject = new AmazonServiceResult<ListBucketInventoryConfigurationsRequest, ListBucketInventoryConfigurationsResponse>((ListBucketInventoryConfigurationsRequest)req, (ListBucketInventoryConfigurationsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListBucketMetricsConfigurationsAsync(ListBucketMetricsConfigurationsRequest request, AmazonServiceCallback<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListBucketMetricsConfigurationsRequestMarshaller instance = ListBucketMetricsConfigurationsRequestMarshaller.Instance;
			ListBucketMetricsConfigurationsResponseUnmarshaller instance2 = ListBucketMetricsConfigurationsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse> responseObject = new AmazonServiceResult<ListBucketMetricsConfigurationsRequest, ListBucketMetricsConfigurationsResponse>((ListBucketMetricsConfigurationsRequest)req, (ListBucketMetricsConfigurationsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListBucketsAsync(AmazonServiceCallback<ListBucketsRequest, ListBucketsResponse> callback, AsyncOptions options = null)
		{
			ListBucketsAsync(new ListBucketsRequest(), callback, options);
		}

		public virtual void ListBucketsAsync(ListBucketsRequest request, AmazonServiceCallback<ListBucketsRequest, ListBucketsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListBucketsRequestMarshaller instance = ListBucketsRequestMarshaller.Instance;
			ListBucketsResponseUnmarshaller instance2 = ListBucketsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListBucketsRequest, ListBucketsResponse> responseObject = new AmazonServiceResult<ListBucketsRequest, ListBucketsResponse>((ListBucketsRequest)req, (ListBucketsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListMultipartUploadsAsync(string bucketName, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null)
		{
			ListMultipartUploadsRequest listMultipartUploadsRequest = new ListMultipartUploadsRequest();
			listMultipartUploadsRequest.BucketName = bucketName;
			ListMultipartUploadsAsync(listMultipartUploadsRequest, callback, options);
		}

		public virtual void ListMultipartUploadsAsync(string bucketName, string prefix, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null)
		{
			ListMultipartUploadsRequest listMultipartUploadsRequest = new ListMultipartUploadsRequest();
			listMultipartUploadsRequest.BucketName = bucketName;
			listMultipartUploadsRequest.Prefix = prefix;
			ListMultipartUploadsAsync(listMultipartUploadsRequest, callback, options);
		}

		public virtual void ListMultipartUploadsAsync(ListMultipartUploadsRequest request, AmazonServiceCallback<ListMultipartUploadsRequest, ListMultipartUploadsResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("ListMultipartUploads is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			ListMultipartUploadsRequestMarshaller instance = ListMultipartUploadsRequestMarshaller.Instance;
			ListMultipartUploadsResponseUnmarshaller instance2 = ListMultipartUploadsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListMultipartUploadsRequest, ListMultipartUploadsResponse> responseObject = new AmazonServiceResult<ListMultipartUploadsRequest, ListMultipartUploadsResponse>((ListMultipartUploadsRequest)req, (ListMultipartUploadsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListObjectsAsync(string bucketName, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null)
		{
			ListObjectsRequest listObjectsRequest = new ListObjectsRequest();
			listObjectsRequest.BucketName = bucketName;
			ListObjectsAsync(listObjectsRequest, callback, options);
		}

		public virtual void ListObjectsAsync(string bucketName, string prefix, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null)
		{
			ListObjectsRequest listObjectsRequest = new ListObjectsRequest();
			listObjectsRequest.BucketName = bucketName;
			listObjectsRequest.Prefix = prefix;
			ListObjectsAsync(listObjectsRequest, callback, options);
		}

		public virtual void ListObjectsAsync(ListObjectsRequest request, AmazonServiceCallback<ListObjectsRequest, ListObjectsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListObjectsRequestMarshaller instance = ListObjectsRequestMarshaller.Instance;
			ListObjectsResponseUnmarshaller instance2 = ListObjectsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListObjectsRequest, ListObjectsResponse> responseObject = new AmazonServiceResult<ListObjectsRequest, ListObjectsResponse>((ListObjectsRequest)req, (ListObjectsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListObjectsV2Async(ListObjectsV2Request request, AmazonServiceCallback<ListObjectsV2Request, ListObjectsV2Response> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListObjectsV2RequestMarshaller instance = ListObjectsV2RequestMarshaller.Instance;
			ListObjectsV2ResponseUnmarshaller instance2 = ListObjectsV2ResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListObjectsV2Request, ListObjectsV2Response> responseObject = new AmazonServiceResult<ListObjectsV2Request, ListObjectsV2Response>((ListObjectsV2Request)req, (ListObjectsV2Response)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListPartsAsync(string bucketName, string key, string uploadId, AmazonServiceCallback<ListPartsRequest, ListPartsResponse> callback, AsyncOptions options = null)
		{
			ListPartsRequest listPartsRequest = new ListPartsRequest();
			listPartsRequest.BucketName = bucketName;
			listPartsRequest.Key = key;
			listPartsRequest.UploadId = uploadId;
			ListPartsAsync(listPartsRequest, callback, options);
		}

		public virtual void ListPartsAsync(ListPartsRequest request, AmazonServiceCallback<ListPartsRequest, ListPartsResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("ListParts is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			ListPartsRequestMarshaller instance = ListPartsRequestMarshaller.Instance;
			ListPartsResponseUnmarshaller instance2 = ListPartsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListPartsRequest, ListPartsResponse> responseObject = new AmazonServiceResult<ListPartsRequest, ListPartsResponse>((ListPartsRequest)req, (ListPartsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void ListVersionsAsync(string bucketName, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null)
		{
			ListVersionsRequest listVersionsRequest = new ListVersionsRequest();
			listVersionsRequest.BucketName = bucketName;
			ListVersionsAsync(listVersionsRequest, callback, options);
		}

		public virtual void ListVersionsAsync(string bucketName, string prefix, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null)
		{
			ListVersionsRequest listVersionsRequest = new ListVersionsRequest();
			listVersionsRequest.BucketName = bucketName;
			listVersionsRequest.Prefix = prefix;
			ListVersionsAsync(listVersionsRequest, callback, options);
		}

		public virtual void ListVersionsAsync(ListVersionsRequest request, AmazonServiceCallback<ListVersionsRequest, ListVersionsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListVersionsRequestMarshaller instance = ListVersionsRequestMarshaller.Instance;
			ListVersionsResponseUnmarshaller instance2 = ListVersionsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListVersionsRequest, ListVersionsResponse> responseObject = new AmazonServiceResult<ListVersionsRequest, ListVersionsResponse>((ListVersionsRequest)req, (ListVersionsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutACLAsync(PutACLRequest request, AmazonServiceCallback<PutACLRequest, PutACLResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutACL is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutACLRequestMarshaller instance = PutACLRequestMarshaller.Instance;
			PutACLResponseUnmarshaller instance2 = PutACLResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutACLRequest, PutACLResponse> responseObject = new AmazonServiceResult<PutACLRequest, PutACLResponse>((PutACLRequest)req, (PutACLResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketAsync(string bucketName, AmazonServiceCallback<PutBucketRequest, PutBucketResponse> callback, AsyncOptions options = null)
		{
			PutBucketRequest putBucketRequest = new PutBucketRequest();
			putBucketRequest.BucketName = bucketName;
			PutBucketAsync(putBucketRequest, callback, options);
		}

		public virtual void PutBucketAsync(PutBucketRequest request, AmazonServiceCallback<PutBucketRequest, PutBucketResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucket is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketRequestMarshaller instance = PutBucketRequestMarshaller.Instance;
			PutBucketResponseUnmarshaller instance2 = PutBucketResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketRequest, PutBucketResponse> responseObject = new AmazonServiceResult<PutBucketRequest, PutBucketResponse>((PutBucketRequest)req, (PutBucketResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketAccelerateConfigurationAsync(PutBucketAccelerateConfigurationRequest request, AmazonServiceCallback<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketAccelerateConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketAccelerateConfigurationRequestMarshaller instance = PutBucketAccelerateConfigurationRequestMarshaller.Instance;
			PutBucketAccelerateConfigurationResponseUnmarshaller instance2 = PutBucketAccelerateConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse> responseObject = new AmazonServiceResult<PutBucketAccelerateConfigurationRequest, PutBucketAccelerateConfigurationResponse>((PutBucketAccelerateConfigurationRequest)req, (PutBucketAccelerateConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketAnalyticsConfigurationAsync(PutBucketAnalyticsConfigurationRequest request, AmazonServiceCallback<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketAnalyticsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketAnalyticsConfigurationRequestMarshaller instance = PutBucketAnalyticsConfigurationRequestMarshaller.Instance;
			PutBucketAnalyticsConfigurationResponseUnmarshaller instance2 = PutBucketAnalyticsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse> responseObject = new AmazonServiceResult<PutBucketAnalyticsConfigurationRequest, PutBucketAnalyticsConfigurationResponse>((PutBucketAnalyticsConfigurationRequest)req, (PutBucketAnalyticsConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketEncryptionAsync(PutBucketEncryptionRequest request, AmazonServiceCallback<PutBucketEncryptionRequest, PutBucketEncryptionResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketEncryption is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketEncryptionRequestMarshaller instance = PutBucketEncryptionRequestMarshaller.Instance;
			PutBucketEncryptionResponseUnmarshaller instance2 = PutBucketEncryptionResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketEncryptionRequest, PutBucketEncryptionResponse> responseObject = new AmazonServiceResult<PutBucketEncryptionRequest, PutBucketEncryptionResponse>((PutBucketEncryptionRequest)req, (PutBucketEncryptionResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketInventoryConfigurationAsync(PutBucketInventoryConfigurationRequest request, AmazonServiceCallback<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketInventoryConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketInventoryConfigurationRequestMarshaller instance = PutBucketInventoryConfigurationRequestMarshaller.Instance;
			PutBucketInventoryConfigurationResponseUnmarshaller instance2 = PutBucketInventoryConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse> responseObject = new AmazonServiceResult<PutBucketInventoryConfigurationRequest, PutBucketInventoryConfigurationResponse>((PutBucketInventoryConfigurationRequest)req, (PutBucketInventoryConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketLoggingAsync(PutBucketLoggingRequest request, AmazonServiceCallback<PutBucketLoggingRequest, PutBucketLoggingResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketLogging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketLoggingRequestMarshaller instance = PutBucketLoggingRequestMarshaller.Instance;
			PutBucketLoggingResponseUnmarshaller instance2 = PutBucketLoggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketLoggingRequest, PutBucketLoggingResponse> responseObject = new AmazonServiceResult<PutBucketLoggingRequest, PutBucketLoggingResponse>((PutBucketLoggingRequest)req, (PutBucketLoggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketMetricsConfigurationAsync(PutBucketMetricsConfigurationRequest request, AmazonServiceCallback<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketMetricsConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketMetricsConfigurationRequestMarshaller instance = PutBucketMetricsConfigurationRequestMarshaller.Instance;
			PutBucketMetricsConfigurationResponseUnmarshaller instance2 = PutBucketMetricsConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse> responseObject = new AmazonServiceResult<PutBucketMetricsConfigurationRequest, PutBucketMetricsConfigurationResponse>((PutBucketMetricsConfigurationRequest)req, (PutBucketMetricsConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketNotificationAsync(PutBucketNotificationRequest request, AmazonServiceCallback<PutBucketNotificationRequest, PutBucketNotificationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketNotification is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketNotificationRequestMarshaller instance = PutBucketNotificationRequestMarshaller.Instance;
			PutBucketNotificationResponseUnmarshaller instance2 = PutBucketNotificationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketNotificationRequest, PutBucketNotificationResponse> responseObject = new AmazonServiceResult<PutBucketNotificationRequest, PutBucketNotificationResponse>((PutBucketNotificationRequest)req, (PutBucketNotificationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketPolicyAsync(string bucketName, string policy, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			PutBucketPolicyRequest putBucketPolicyRequest = new PutBucketPolicyRequest();
			putBucketPolicyRequest.BucketName = bucketName;
			putBucketPolicyRequest.Policy = policy;
			PutBucketPolicyAsync(putBucketPolicyRequest, callback, options);
		}

		public virtual void PutBucketPolicyAsync(string bucketName, string policy, string contentMD5, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			PutBucketPolicyRequest putBucketPolicyRequest = new PutBucketPolicyRequest();
			putBucketPolicyRequest.BucketName = bucketName;
			putBucketPolicyRequest.Policy = policy;
			putBucketPolicyRequest.ContentMD5 = contentMD5;
			PutBucketPolicyAsync(putBucketPolicyRequest, callback, options);
		}

		public virtual void PutBucketPolicyAsync(PutBucketPolicyRequest request, AmazonServiceCallback<PutBucketPolicyRequest, PutBucketPolicyResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketPolicy is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketPolicyRequestMarshaller instance = PutBucketPolicyRequestMarshaller.Instance;
			PutBucketPolicyResponseUnmarshaller instance2 = PutBucketPolicyResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketPolicyRequest, PutBucketPolicyResponse> responseObject = new AmazonServiceResult<PutBucketPolicyRequest, PutBucketPolicyResponse>((PutBucketPolicyRequest)req, (PutBucketPolicyResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketReplicationAsync(PutBucketReplicationRequest request, AmazonServiceCallback<PutBucketReplicationRequest, PutBucketReplicationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketReplication is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketReplicationRequestMarshaller instance = PutBucketReplicationRequestMarshaller.Instance;
			PutBucketReplicationResponseUnmarshaller instance2 = PutBucketReplicationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketReplicationRequest, PutBucketReplicationResponse> responseObject = new AmazonServiceResult<PutBucketReplicationRequest, PutBucketReplicationResponse>((PutBucketReplicationRequest)req, (PutBucketReplicationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketRequestPaymentAsync(string bucketName, RequestPaymentConfiguration requestPaymentConfiguration, AmazonServiceCallback<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			PutBucketRequestPaymentRequest putBucketRequestPaymentRequest = new PutBucketRequestPaymentRequest();
			putBucketRequestPaymentRequest.BucketName = bucketName;
			putBucketRequestPaymentRequest.RequestPaymentConfiguration = requestPaymentConfiguration;
			PutBucketRequestPaymentAsync(putBucketRequestPaymentRequest, callback, options);
		}

		public virtual void PutBucketRequestPaymentAsync(PutBucketRequestPaymentRequest request, AmazonServiceCallback<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketRequestPayment is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketRequestPaymentRequestMarshaller instance = PutBucketRequestPaymentRequestMarshaller.Instance;
			PutBucketRequestPaymentResponseUnmarshaller instance2 = PutBucketRequestPaymentResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse> responseObject = new AmazonServiceResult<PutBucketRequestPaymentRequest, PutBucketRequestPaymentResponse>((PutBucketRequestPaymentRequest)req, (PutBucketRequestPaymentResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketTaggingAsync(string bucketName, List<Tag> tagSet, AmazonServiceCallback<PutBucketTaggingRequest, PutBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			PutBucketTaggingRequest putBucketTaggingRequest = new PutBucketTaggingRequest();
			putBucketTaggingRequest.BucketName = bucketName;
			putBucketTaggingRequest.TagSet = tagSet;
			PutBucketTaggingAsync(putBucketTaggingRequest, callback, options);
		}

		public virtual void PutBucketTaggingAsync(PutBucketTaggingRequest request, AmazonServiceCallback<PutBucketTaggingRequest, PutBucketTaggingResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketTaggingRequestMarshaller instance = PutBucketTaggingRequestMarshaller.Instance;
			PutBucketTaggingResponseUnmarshaller instance2 = PutBucketTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketTaggingRequest, PutBucketTaggingResponse> responseObject = new AmazonServiceResult<PutBucketTaggingRequest, PutBucketTaggingResponse>((PutBucketTaggingRequest)req, (PutBucketTaggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketVersioningAsync(PutBucketVersioningRequest request, AmazonServiceCallback<PutBucketVersioningRequest, PutBucketVersioningResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketVersioning is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketVersioningRequestMarshaller instance = PutBucketVersioningRequestMarshaller.Instance;
			PutBucketVersioningResponseUnmarshaller instance2 = PutBucketVersioningResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketVersioningRequest, PutBucketVersioningResponse> responseObject = new AmazonServiceResult<PutBucketVersioningRequest, PutBucketVersioningResponse>((PutBucketVersioningRequest)req, (PutBucketVersioningResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutBucketWebsiteAsync(string bucketName, WebsiteConfiguration websiteConfiguration, AmazonServiceCallback<PutBucketWebsiteRequest, PutBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			PutBucketWebsiteRequest putBucketWebsiteRequest = new PutBucketWebsiteRequest();
			putBucketWebsiteRequest.BucketName = bucketName;
			putBucketWebsiteRequest.WebsiteConfiguration = websiteConfiguration;
			PutBucketWebsiteAsync(putBucketWebsiteRequest, callback, options);
		}

		public virtual void PutBucketWebsiteAsync(PutBucketWebsiteRequest request, AmazonServiceCallback<PutBucketWebsiteRequest, PutBucketWebsiteResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutBucketWebsite is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutBucketWebsiteRequestMarshaller instance = PutBucketWebsiteRequestMarshaller.Instance;
			PutBucketWebsiteResponseUnmarshaller instance2 = PutBucketWebsiteResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutBucketWebsiteRequest, PutBucketWebsiteResponse> responseObject = new AmazonServiceResult<PutBucketWebsiteRequest, PutBucketWebsiteResponse>((PutBucketWebsiteRequest)req, (PutBucketWebsiteResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutCORSConfigurationAsync(string bucketName, CORSConfiguration configuration, AmazonServiceCallback<PutCORSConfigurationRequest, PutCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			PutCORSConfigurationRequest putCORSConfigurationRequest = new PutCORSConfigurationRequest();
			putCORSConfigurationRequest.BucketName = bucketName;
			putCORSConfigurationRequest.Configuration = configuration;
			PutCORSConfigurationAsync(putCORSConfigurationRequest, callback, options);
		}

		public virtual void PutCORSConfigurationAsync(PutCORSConfigurationRequest request, AmazonServiceCallback<PutCORSConfigurationRequest, PutCORSConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutCORSConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutCORSConfigurationRequestMarshaller instance = PutCORSConfigurationRequestMarshaller.Instance;
			PutCORSConfigurationResponseUnmarshaller instance2 = PutCORSConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutCORSConfigurationRequest, PutCORSConfigurationResponse> responseObject = new AmazonServiceResult<PutCORSConfigurationRequest, PutCORSConfigurationResponse>((PutCORSConfigurationRequest)req, (PutCORSConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutLifecycleConfigurationAsync(string bucketName, LifecycleConfiguration configuration, AmazonServiceCallback<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			PutLifecycleConfigurationRequest putLifecycleConfigurationRequest = new PutLifecycleConfigurationRequest();
			putLifecycleConfigurationRequest.BucketName = bucketName;
			putLifecycleConfigurationRequest.Configuration = configuration;
			PutLifecycleConfigurationAsync(putLifecycleConfigurationRequest, callback, options);
		}

		public virtual void PutLifecycleConfigurationAsync(PutLifecycleConfigurationRequest request, AmazonServiceCallback<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutLifecycleConfiguration is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutLifecycleConfigurationRequestMarshaller instance = PutLifecycleConfigurationRequestMarshaller.Instance;
			PutLifecycleConfigurationResponseUnmarshaller instance2 = PutLifecycleConfigurationResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse> responseObject = new AmazonServiceResult<PutLifecycleConfigurationRequest, PutLifecycleConfigurationResponse>((PutLifecycleConfigurationRequest)req, (PutLifecycleConfigurationResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutObjectAsync(PutObjectRequest request, AmazonServiceCallback<PutObjectRequest, PutObjectResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutObject is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutObjectRequestMarshaller instance = PutObjectRequestMarshaller.Instance;
			PutObjectResponseUnmarshaller instance2 = PutObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutObjectRequest, PutObjectResponse> responseObject = new AmazonServiceResult<PutObjectRequest, PutObjectResponse>((PutObjectRequest)req, (PutObjectResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void PutObjectTaggingAsync(PutObjectTaggingRequest request, AmazonServiceCallback<PutObjectTaggingRequest, PutObjectTaggingResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("PutObjectTagging is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			PutObjectTaggingRequestMarshaller instance = PutObjectTaggingRequestMarshaller.Instance;
			PutObjectTaggingResponseUnmarshaller instance2 = PutObjectTaggingResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<PutObjectTaggingRequest, PutObjectTaggingResponse> responseObject = new AmazonServiceResult<PutObjectTaggingRequest, PutObjectTaggingResponse>((PutObjectTaggingRequest)req, (PutObjectTaggingResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void RestoreObjectAsync(string bucketName, string key, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public virtual void RestoreObjectAsync(string bucketName, string key, int days, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			restoreObjectRequest.Days = days;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public virtual void RestoreObjectAsync(string bucketName, string key, string versionId, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			restoreObjectRequest.VersionId = versionId;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public virtual void RestoreObjectAsync(string bucketName, string key, string versionId, int days, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			RestoreObjectRequest restoreObjectRequest = new RestoreObjectRequest();
			restoreObjectRequest.BucketName = bucketName;
			restoreObjectRequest.Key = key;
			restoreObjectRequest.VersionId = versionId;
			restoreObjectRequest.Days = days;
			RestoreObjectAsync(restoreObjectRequest, callback, options);
		}

		public virtual void RestoreObjectAsync(RestoreObjectRequest request, AmazonServiceCallback<RestoreObjectRequest, RestoreObjectResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			RestoreObjectRequestMarshaller instance = RestoreObjectRequestMarshaller.Instance;
			RestoreObjectResponseUnmarshaller instance2 = RestoreObjectResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<RestoreObjectRequest, RestoreObjectResponse> responseObject = new AmazonServiceResult<RestoreObjectRequest, RestoreObjectResponse>((RestoreObjectRequest)req, (RestoreObjectResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		public virtual void UploadPartAsync(UploadPartRequest request, AmazonServiceCallback<UploadPartRequest, UploadPartResponse> callback, AsyncOptions options = null)
		{
			if (AWSConfigs.HttpClient == AWSConfigs.HttpClientOption.UnityWWW)
			{
				throw new InvalidOperationException("UploadPart is only allowed with AWSConfigs.HttpClientOption.UnityWebRequest API option");
			}
			options = ((options == null) ? new AsyncOptions() : options);
			UploadPartRequestMarshaller instance = UploadPartRequestMarshaller.Instance;
			UploadPartResponseUnmarshaller instance2 = UploadPartResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UploadPartRequest, UploadPartResponse> responseObject = new AmazonServiceResult<UploadPartRequest, UploadPartResponse>((UploadPartRequest)req, (UploadPartResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}
	}
}
