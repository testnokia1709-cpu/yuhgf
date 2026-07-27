using System;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;
using Amazon.Runtime.Internal.Util;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SecurityToken.Model;
using Amazon.SecurityToken.Model.Internal.MarshallTransformations;

namespace Amazon.SecurityToken
{
	public class AmazonSecurityTokenServiceClient : AmazonServiceClient, IAmazonSecurityTokenService, IDisposable, ICoreAmazonSTS, IAmazonService
	{
		IClientConfig IAmazonService.Config
		{
			get
			{
				return base.Config;
			}
		}

		AssumeRoleImmutableCredentials ICoreAmazonSTS.CredentialsFromAssumeRoleAuthentication(string roleArn, string roleSessionName, AssumeRoleAWSCredentialsOptions options)
		{
			try
			{
				AssumeRoleRequest assumeRoleRequest = new AssumeRoleRequest
				{
					RoleArn = roleArn,
					RoleSessionName = roleSessionName
				};
				if (options != null)
				{
					assumeRoleRequest.ExternalId = options.ExternalId;
					assumeRoleRequest.SerialNumber = options.MfaSerialNumber;
					assumeRoleRequest.TokenCode = options.MfaTokenCode;
					assumeRoleRequest.Policy = options.Policy;
					if (options.DurationSeconds.HasValue)
					{
						assumeRoleRequest.DurationSeconds = options.DurationSeconds.Value;
					}
				}
				AssumeRoleResponse assumeRoleResponse = AssumeRole(assumeRoleRequest);
				return new AssumeRoleImmutableCredentials(assumeRoleResponse.Credentials.AccessKeyId, assumeRoleResponse.Credentials.SecretAccessKey, assumeRoleResponse.Credentials.SessionToken, assumeRoleResponse.Credentials.Expiration);
			}
			catch (Exception innerException)
			{
				AmazonClientException ex = new AmazonClientException("Error calling AssumeRole for role " + roleArn, innerException);
				Logger.GetLogger(typeof(AmazonSecurityTokenServiceClient)).Error(ex, ex.Message);
				throw ex;
			}
		}

		public AmazonSecurityTokenServiceClient(AWSCredentials credentials)
			: this(credentials, new AmazonSecurityTokenServiceConfig())
		{
		}

		public AmazonSecurityTokenServiceClient(AWSCredentials credentials, RegionEndpoint region)
			: this(credentials, new AmazonSecurityTokenServiceConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonSecurityTokenServiceClient(AWSCredentials credentials, AmazonSecurityTokenServiceConfig clientConfig)
			: base(credentials, clientConfig)
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonSecurityTokenServiceConfig())
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonSecurityTokenServiceConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, AmazonSecurityTokenServiceConfig clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, clientConfig)
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonSecurityTokenServiceConfig())
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonSecurityTokenServiceConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonSecurityTokenServiceClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonSecurityTokenServiceConfig clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, clientConfig)
		{
		}

		protected override AbstractAWSSigner CreateSigner()
		{
			return new AWS4Signer();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
		}

		internal virtual AssumeRoleResponse AssumeRole(AssumeRoleRequest request)
		{
			AssumeRoleRequestMarshaller instance = AssumeRoleRequestMarshaller.Instance;
			AssumeRoleResponseUnmarshaller instance2 = AssumeRoleResponseUnmarshaller.Instance;
			return Invoke<AssumeRoleRequest, AssumeRoleResponse>(request, instance, instance2);
		}

		public virtual void AssumeRoleAsync(AssumeRoleRequest request, AmazonServiceCallback<AssumeRoleRequest, AssumeRoleResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			AssumeRoleRequestMarshaller instance = AssumeRoleRequestMarshaller.Instance;
			AssumeRoleResponseUnmarshaller instance2 = AssumeRoleResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AssumeRoleRequest, AssumeRoleResponse> responseObject = new AmazonServiceResult<AssumeRoleRequest, AssumeRoleResponse>((AssumeRoleRequest)req, (AssumeRoleResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual AssumeRoleWithSAMLResponse AssumeRoleWithSAML(AssumeRoleWithSAMLRequest request)
		{
			AssumeRoleWithSAMLRequestMarshaller instance = AssumeRoleWithSAMLRequestMarshaller.Instance;
			AssumeRoleWithSAMLResponseUnmarshaller instance2 = AssumeRoleWithSAMLResponseUnmarshaller.Instance;
			return Invoke<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse>(request, instance, instance2);
		}

		public virtual void AssumeRoleWithSAMLAsync(AssumeRoleWithSAMLRequest request, AmazonServiceCallback<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			AssumeRoleWithSAMLRequestMarshaller instance = AssumeRoleWithSAMLRequestMarshaller.Instance;
			AssumeRoleWithSAMLResponseUnmarshaller instance2 = AssumeRoleWithSAMLResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse> responseObject = new AmazonServiceResult<AssumeRoleWithSAMLRequest, AssumeRoleWithSAMLResponse>((AssumeRoleWithSAMLRequest)req, (AssumeRoleWithSAMLResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual AssumeRoleWithWebIdentityResponse AssumeRoleWithWebIdentity(AssumeRoleWithWebIdentityRequest request)
		{
			AssumeRoleWithWebIdentityRequestMarshaller instance = AssumeRoleWithWebIdentityRequestMarshaller.Instance;
			AssumeRoleWithWebIdentityResponseUnmarshaller instance2 = AssumeRoleWithWebIdentityResponseUnmarshaller.Instance;
			return Invoke<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse>(request, instance, instance2);
		}

		public virtual void AssumeRoleWithWebIdentityAsync(AssumeRoleWithWebIdentityRequest request, AmazonServiceCallback<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			AssumeRoleWithWebIdentityRequestMarshaller instance = AssumeRoleWithWebIdentityRequestMarshaller.Instance;
			AssumeRoleWithWebIdentityResponseUnmarshaller instance2 = AssumeRoleWithWebIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse> responseObject = new AmazonServiceResult<AssumeRoleWithWebIdentityRequest, AssumeRoleWithWebIdentityResponse>((AssumeRoleWithWebIdentityRequest)req, (AssumeRoleWithWebIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual DecodeAuthorizationMessageResponse DecodeAuthorizationMessage(DecodeAuthorizationMessageRequest request)
		{
			DecodeAuthorizationMessageRequestMarshaller instance = DecodeAuthorizationMessageRequestMarshaller.Instance;
			DecodeAuthorizationMessageResponseUnmarshaller instance2 = DecodeAuthorizationMessageResponseUnmarshaller.Instance;
			return Invoke<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse>(request, instance, instance2);
		}

		public virtual void DecodeAuthorizationMessageAsync(DecodeAuthorizationMessageRequest request, AmazonServiceCallback<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DecodeAuthorizationMessageRequestMarshaller instance = DecodeAuthorizationMessageRequestMarshaller.Instance;
			DecodeAuthorizationMessageResponseUnmarshaller instance2 = DecodeAuthorizationMessageResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse> responseObject = new AmazonServiceResult<DecodeAuthorizationMessageRequest, DecodeAuthorizationMessageResponse>((DecodeAuthorizationMessageRequest)req, (DecodeAuthorizationMessageResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetCallerIdentityResponse GetCallerIdentity(GetCallerIdentityRequest request)
		{
			GetCallerIdentityRequestMarshaller instance = GetCallerIdentityRequestMarshaller.Instance;
			GetCallerIdentityResponseUnmarshaller instance2 = GetCallerIdentityResponseUnmarshaller.Instance;
			return Invoke<GetCallerIdentityRequest, GetCallerIdentityResponse>(request, instance, instance2);
		}

		public virtual void GetCallerIdentityAsync(GetCallerIdentityRequest request, AmazonServiceCallback<GetCallerIdentityRequest, GetCallerIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetCallerIdentityRequestMarshaller instance = GetCallerIdentityRequestMarshaller.Instance;
			GetCallerIdentityResponseUnmarshaller instance2 = GetCallerIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetCallerIdentityRequest, GetCallerIdentityResponse> responseObject = new AmazonServiceResult<GetCallerIdentityRequest, GetCallerIdentityResponse>((GetCallerIdentityRequest)req, (GetCallerIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetFederationTokenResponse GetFederationToken(GetFederationTokenRequest request)
		{
			GetFederationTokenRequestMarshaller instance = GetFederationTokenRequestMarshaller.Instance;
			GetFederationTokenResponseUnmarshaller instance2 = GetFederationTokenResponseUnmarshaller.Instance;
			return Invoke<GetFederationTokenRequest, GetFederationTokenResponse>(request, instance, instance2);
		}

		public virtual void GetFederationTokenAsync(GetFederationTokenRequest request, AmazonServiceCallback<GetFederationTokenRequest, GetFederationTokenResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetFederationTokenRequestMarshaller instance = GetFederationTokenRequestMarshaller.Instance;
			GetFederationTokenResponseUnmarshaller instance2 = GetFederationTokenResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetFederationTokenRequest, GetFederationTokenResponse> responseObject = new AmazonServiceResult<GetFederationTokenRequest, GetFederationTokenResponse>((GetFederationTokenRequest)req, (GetFederationTokenResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetSessionTokenResponse GetSessionToken()
		{
			return GetSessionToken(new GetSessionTokenRequest());
		}

		internal virtual GetSessionTokenResponse GetSessionToken(GetSessionTokenRequest request)
		{
			GetSessionTokenRequestMarshaller instance = GetSessionTokenRequestMarshaller.Instance;
			GetSessionTokenResponseUnmarshaller instance2 = GetSessionTokenResponseUnmarshaller.Instance;
			return Invoke<GetSessionTokenRequest, GetSessionTokenResponse>(request, instance, instance2);
		}

		public virtual void GetSessionTokenAsync(AmazonServiceCallback<GetSessionTokenRequest, GetSessionTokenResponse> callback, AsyncOptions options = null)
		{
			GetSessionTokenAsync(new GetSessionTokenRequest(), callback, options);
		}

		public virtual void GetSessionTokenAsync(GetSessionTokenRequest request, AmazonServiceCallback<GetSessionTokenRequest, GetSessionTokenResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetSessionTokenRequestMarshaller instance = GetSessionTokenRequestMarshaller.Instance;
			GetSessionTokenResponseUnmarshaller instance2 = GetSessionTokenResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetSessionTokenRequest, GetSessionTokenResponse> responseObject = new AmazonServiceResult<GetSessionTokenRequest, GetSessionTokenResponse>((GetSessionTokenRequest)req, (GetSessionTokenResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}
	}
}
