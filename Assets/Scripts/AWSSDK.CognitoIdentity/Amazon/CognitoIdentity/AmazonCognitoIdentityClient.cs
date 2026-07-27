using System;
using System.Collections.Generic;
using Amazon.CognitoIdentity.Model;
using Amazon.CognitoIdentity.Model.Internal.MarshallTransformations;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Auth;

namespace Amazon.CognitoIdentity
{
	public class AmazonCognitoIdentityClient : AmazonServiceClient, IAmazonCognitoIdentity, IAmazonService, IDisposable
	{
		IClientConfig IAmazonService.Config
		{
			get
			{
				return base.Config;
			}
		}

		public AmazonCognitoIdentityClient(AWSCredentials credentials)
			: this(credentials, new AmazonCognitoIdentityConfig())
		{
		}

		public AmazonCognitoIdentityClient(AWSCredentials credentials, RegionEndpoint region)
			: this(credentials, new AmazonCognitoIdentityConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonCognitoIdentityClient(AWSCredentials credentials, AmazonCognitoIdentityConfig clientConfig)
			: base(credentials, clientConfig)
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonCognitoIdentityConfig())
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, new AmazonCognitoIdentityConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, AmazonCognitoIdentityConfig clientConfig)
			: base(awsAccessKeyId, awsSecretAccessKey, clientConfig)
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonCognitoIdentityConfig())
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, RegionEndpoint region)
			: this(awsAccessKeyId, awsSecretAccessKey, awsSessionToken, new AmazonCognitoIdentityConfig
			{
				RegionEndpoint = region
			})
		{
		}

		public AmazonCognitoIdentityClient(string awsAccessKeyId, string awsSecretAccessKey, string awsSessionToken, AmazonCognitoIdentityConfig clientConfig)
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

		internal virtual CreateIdentityPoolResponse CreateIdentityPool(CreateIdentityPoolRequest request)
		{
			CreateIdentityPoolRequestMarshaller instance = CreateIdentityPoolRequestMarshaller.Instance;
			CreateIdentityPoolResponseUnmarshaller instance2 = CreateIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<CreateIdentityPoolRequest, CreateIdentityPoolResponse>(request, instance, instance2);
		}

		public virtual void CreateIdentityPoolAsync(CreateIdentityPoolRequest request, AmazonServiceCallback<CreateIdentityPoolRequest, CreateIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			CreateIdentityPoolRequestMarshaller instance = CreateIdentityPoolRequestMarshaller.Instance;
			CreateIdentityPoolResponseUnmarshaller instance2 = CreateIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<CreateIdentityPoolRequest, CreateIdentityPoolResponse> responseObject = new AmazonServiceResult<CreateIdentityPoolRequest, CreateIdentityPoolResponse>((CreateIdentityPoolRequest)req, (CreateIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual DeleteIdentitiesResponse DeleteIdentities(DeleteIdentitiesRequest request)
		{
			DeleteIdentitiesRequestMarshaller instance = DeleteIdentitiesRequestMarshaller.Instance;
			DeleteIdentitiesResponseUnmarshaller instance2 = DeleteIdentitiesResponseUnmarshaller.Instance;
			return Invoke<DeleteIdentitiesRequest, DeleteIdentitiesResponse>(request, instance, instance2);
		}

		public virtual void DeleteIdentitiesAsync(DeleteIdentitiesRequest request, AmazonServiceCallback<DeleteIdentitiesRequest, DeleteIdentitiesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteIdentitiesRequestMarshaller instance = DeleteIdentitiesRequestMarshaller.Instance;
			DeleteIdentitiesResponseUnmarshaller instance2 = DeleteIdentitiesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteIdentitiesRequest, DeleteIdentitiesResponse> responseObject = new AmazonServiceResult<DeleteIdentitiesRequest, DeleteIdentitiesResponse>((DeleteIdentitiesRequest)req, (DeleteIdentitiesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual DeleteIdentityPoolResponse DeleteIdentityPool(DeleteIdentityPoolRequest request)
		{
			DeleteIdentityPoolRequestMarshaller instance = DeleteIdentityPoolRequestMarshaller.Instance;
			DeleteIdentityPoolResponseUnmarshaller instance2 = DeleteIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse>(request, instance, instance2);
		}

		public virtual void DeleteIdentityPoolAsync(string identityPoolId, AmazonServiceCallback<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			DeleteIdentityPoolRequest deleteIdentityPoolRequest = new DeleteIdentityPoolRequest();
			deleteIdentityPoolRequest.IdentityPoolId = identityPoolId;
			DeleteIdentityPoolAsync(deleteIdentityPoolRequest, callback, options);
		}

		public virtual void DeleteIdentityPoolAsync(DeleteIdentityPoolRequest request, AmazonServiceCallback<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DeleteIdentityPoolRequestMarshaller instance = DeleteIdentityPoolRequestMarshaller.Instance;
			DeleteIdentityPoolResponseUnmarshaller instance2 = DeleteIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse> responseObject = new AmazonServiceResult<DeleteIdentityPoolRequest, DeleteIdentityPoolResponse>((DeleteIdentityPoolRequest)req, (DeleteIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual DescribeIdentityResponse DescribeIdentity(DescribeIdentityRequest request)
		{
			DescribeIdentityRequestMarshaller instance = DescribeIdentityRequestMarshaller.Instance;
			DescribeIdentityResponseUnmarshaller instance2 = DescribeIdentityResponseUnmarshaller.Instance;
			return Invoke<DescribeIdentityRequest, DescribeIdentityResponse>(request, instance, instance2);
		}

		public virtual void DescribeIdentityAsync(string identityId, AmazonServiceCallback<DescribeIdentityRequest, DescribeIdentityResponse> callback, AsyncOptions options = null)
		{
			DescribeIdentityRequest describeIdentityRequest = new DescribeIdentityRequest();
			describeIdentityRequest.IdentityId = identityId;
			DescribeIdentityAsync(describeIdentityRequest, callback, options);
		}

		public virtual void DescribeIdentityAsync(DescribeIdentityRequest request, AmazonServiceCallback<DescribeIdentityRequest, DescribeIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DescribeIdentityRequestMarshaller instance = DescribeIdentityRequestMarshaller.Instance;
			DescribeIdentityResponseUnmarshaller instance2 = DescribeIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DescribeIdentityRequest, DescribeIdentityResponse> responseObject = new AmazonServiceResult<DescribeIdentityRequest, DescribeIdentityResponse>((DescribeIdentityRequest)req, (DescribeIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual DescribeIdentityPoolResponse DescribeIdentityPool(DescribeIdentityPoolRequest request)
		{
			DescribeIdentityPoolRequestMarshaller instance = DescribeIdentityPoolRequestMarshaller.Instance;
			DescribeIdentityPoolResponseUnmarshaller instance2 = DescribeIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse>(request, instance, instance2);
		}

		public virtual void DescribeIdentityPoolAsync(string identityPoolId, AmazonServiceCallback<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			DescribeIdentityPoolRequest describeIdentityPoolRequest = new DescribeIdentityPoolRequest();
			describeIdentityPoolRequest.IdentityPoolId = identityPoolId;
			DescribeIdentityPoolAsync(describeIdentityPoolRequest, callback, options);
		}

		public virtual void DescribeIdentityPoolAsync(DescribeIdentityPoolRequest request, AmazonServiceCallback<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			DescribeIdentityPoolRequestMarshaller instance = DescribeIdentityPoolRequestMarshaller.Instance;
			DescribeIdentityPoolResponseUnmarshaller instance2 = DescribeIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse> responseObject = new AmazonServiceResult<DescribeIdentityPoolRequest, DescribeIdentityPoolResponse>((DescribeIdentityPoolRequest)req, (DescribeIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetCredentialsForIdentityResponse GetCredentialsForIdentity(GetCredentialsForIdentityRequest request)
		{
			GetCredentialsForIdentityRequestMarshaller instance = GetCredentialsForIdentityRequestMarshaller.Instance;
			GetCredentialsForIdentityResponseUnmarshaller instance2 = GetCredentialsForIdentityResponseUnmarshaller.Instance;
			return Invoke<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse>(request, instance, instance2);
		}

		public virtual void GetCredentialsForIdentityAsync(string identityId, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null)
		{
			GetCredentialsForIdentityRequest getCredentialsForIdentityRequest = new GetCredentialsForIdentityRequest();
			getCredentialsForIdentityRequest.IdentityId = identityId;
			GetCredentialsForIdentityAsync(getCredentialsForIdentityRequest, callback, options);
		}

		public virtual void GetCredentialsForIdentityAsync(string identityId, Dictionary<string, string> logins, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null)
		{
			GetCredentialsForIdentityRequest getCredentialsForIdentityRequest = new GetCredentialsForIdentityRequest();
			getCredentialsForIdentityRequest.IdentityId = identityId;
			getCredentialsForIdentityRequest.Logins = logins;
			GetCredentialsForIdentityAsync(getCredentialsForIdentityRequest, callback, options);
		}

		public virtual void GetCredentialsForIdentityAsync(GetCredentialsForIdentityRequest request, AmazonServiceCallback<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetCredentialsForIdentityRequestMarshaller instance = GetCredentialsForIdentityRequestMarshaller.Instance;
			GetCredentialsForIdentityResponseUnmarshaller instance2 = GetCredentialsForIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse> responseObject = new AmazonServiceResult<GetCredentialsForIdentityRequest, GetCredentialsForIdentityResponse>((GetCredentialsForIdentityRequest)req, (GetCredentialsForIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetIdResponse GetId(GetIdRequest request)
		{
			GetIdRequestMarshaller instance = GetIdRequestMarshaller.Instance;
			GetIdResponseUnmarshaller instance2 = GetIdResponseUnmarshaller.Instance;
			return Invoke<GetIdRequest, GetIdResponse>(request, instance, instance2);
		}

		public virtual void GetIdAsync(GetIdRequest request, AmazonServiceCallback<GetIdRequest, GetIdResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetIdRequestMarshaller instance = GetIdRequestMarshaller.Instance;
			GetIdResponseUnmarshaller instance2 = GetIdResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetIdRequest, GetIdResponse> responseObject = new AmazonServiceResult<GetIdRequest, GetIdResponse>((GetIdRequest)req, (GetIdResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetIdentityPoolRolesResponse GetIdentityPoolRoles(GetIdentityPoolRolesRequest request)
		{
			GetIdentityPoolRolesRequestMarshaller instance = GetIdentityPoolRolesRequestMarshaller.Instance;
			GetIdentityPoolRolesResponseUnmarshaller instance2 = GetIdentityPoolRolesResponseUnmarshaller.Instance;
			return Invoke<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse>(request, instance, instance2);
		}

		public virtual void GetIdentityPoolRolesAsync(string identityPoolId, AmazonServiceCallback<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			GetIdentityPoolRolesRequest getIdentityPoolRolesRequest = new GetIdentityPoolRolesRequest();
			getIdentityPoolRolesRequest.IdentityPoolId = identityPoolId;
			GetIdentityPoolRolesAsync(getIdentityPoolRolesRequest, callback, options);
		}

		public virtual void GetIdentityPoolRolesAsync(GetIdentityPoolRolesRequest request, AmazonServiceCallback<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetIdentityPoolRolesRequestMarshaller instance = GetIdentityPoolRolesRequestMarshaller.Instance;
			GetIdentityPoolRolesResponseUnmarshaller instance2 = GetIdentityPoolRolesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse> responseObject = new AmazonServiceResult<GetIdentityPoolRolesRequest, GetIdentityPoolRolesResponse>((GetIdentityPoolRolesRequest)req, (GetIdentityPoolRolesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetOpenIdTokenResponse GetOpenIdToken(GetOpenIdTokenRequest request)
		{
			GetOpenIdTokenRequestMarshaller instance = GetOpenIdTokenRequestMarshaller.Instance;
			GetOpenIdTokenResponseUnmarshaller instance2 = GetOpenIdTokenResponseUnmarshaller.Instance;
			return Invoke<GetOpenIdTokenRequest, GetOpenIdTokenResponse>(request, instance, instance2);
		}

		public virtual void GetOpenIdTokenAsync(string identityId, AmazonServiceCallback<GetOpenIdTokenRequest, GetOpenIdTokenResponse> callback, AsyncOptions options = null)
		{
			GetOpenIdTokenRequest getOpenIdTokenRequest = new GetOpenIdTokenRequest();
			getOpenIdTokenRequest.IdentityId = identityId;
			GetOpenIdTokenAsync(getOpenIdTokenRequest, callback, options);
		}

		public virtual void GetOpenIdTokenAsync(GetOpenIdTokenRequest request, AmazonServiceCallback<GetOpenIdTokenRequest, GetOpenIdTokenResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetOpenIdTokenRequestMarshaller instance = GetOpenIdTokenRequestMarshaller.Instance;
			GetOpenIdTokenResponseUnmarshaller instance2 = GetOpenIdTokenResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetOpenIdTokenRequest, GetOpenIdTokenResponse> responseObject = new AmazonServiceResult<GetOpenIdTokenRequest, GetOpenIdTokenResponse>((GetOpenIdTokenRequest)req, (GetOpenIdTokenResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual GetOpenIdTokenForDeveloperIdentityResponse GetOpenIdTokenForDeveloperIdentity(GetOpenIdTokenForDeveloperIdentityRequest request)
		{
			GetOpenIdTokenForDeveloperIdentityRequestMarshaller instance = GetOpenIdTokenForDeveloperIdentityRequestMarshaller.Instance;
			GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller instance2 = GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller.Instance;
			return Invoke<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse>(request, instance, instance2);
		}

		public virtual void GetOpenIdTokenForDeveloperIdentityAsync(GetOpenIdTokenForDeveloperIdentityRequest request, AmazonServiceCallback<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			GetOpenIdTokenForDeveloperIdentityRequestMarshaller instance = GetOpenIdTokenForDeveloperIdentityRequestMarshaller.Instance;
			GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller instance2 = GetOpenIdTokenForDeveloperIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse> responseObject = new AmazonServiceResult<GetOpenIdTokenForDeveloperIdentityRequest, GetOpenIdTokenForDeveloperIdentityResponse>((GetOpenIdTokenForDeveloperIdentityRequest)req, (GetOpenIdTokenForDeveloperIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual ListIdentitiesResponse ListIdentities(ListIdentitiesRequest request)
		{
			ListIdentitiesRequestMarshaller instance = ListIdentitiesRequestMarshaller.Instance;
			ListIdentitiesResponseUnmarshaller instance2 = ListIdentitiesResponseUnmarshaller.Instance;
			return Invoke<ListIdentitiesRequest, ListIdentitiesResponse>(request, instance, instance2);
		}

		public virtual void ListIdentitiesAsync(ListIdentitiesRequest request, AmazonServiceCallback<ListIdentitiesRequest, ListIdentitiesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListIdentitiesRequestMarshaller instance = ListIdentitiesRequestMarshaller.Instance;
			ListIdentitiesResponseUnmarshaller instance2 = ListIdentitiesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListIdentitiesRequest, ListIdentitiesResponse> responseObject = new AmazonServiceResult<ListIdentitiesRequest, ListIdentitiesResponse>((ListIdentitiesRequest)req, (ListIdentitiesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual ListIdentityPoolsResponse ListIdentityPools(ListIdentityPoolsRequest request)
		{
			ListIdentityPoolsRequestMarshaller instance = ListIdentityPoolsRequestMarshaller.Instance;
			ListIdentityPoolsResponseUnmarshaller instance2 = ListIdentityPoolsResponseUnmarshaller.Instance;
			return Invoke<ListIdentityPoolsRequest, ListIdentityPoolsResponse>(request, instance, instance2);
		}

		public virtual void ListIdentityPoolsAsync(ListIdentityPoolsRequest request, AmazonServiceCallback<ListIdentityPoolsRequest, ListIdentityPoolsResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			ListIdentityPoolsRequestMarshaller instance = ListIdentityPoolsRequestMarshaller.Instance;
			ListIdentityPoolsResponseUnmarshaller instance2 = ListIdentityPoolsResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<ListIdentityPoolsRequest, ListIdentityPoolsResponse> responseObject = new AmazonServiceResult<ListIdentityPoolsRequest, ListIdentityPoolsResponse>((ListIdentityPoolsRequest)req, (ListIdentityPoolsResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual LookupDeveloperIdentityResponse LookupDeveloperIdentity(LookupDeveloperIdentityRequest request)
		{
			LookupDeveloperIdentityRequestMarshaller instance = LookupDeveloperIdentityRequestMarshaller.Instance;
			LookupDeveloperIdentityResponseUnmarshaller instance2 = LookupDeveloperIdentityResponseUnmarshaller.Instance;
			return Invoke<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse>(request, instance, instance2);
		}

		public virtual void LookupDeveloperIdentityAsync(LookupDeveloperIdentityRequest request, AmazonServiceCallback<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			LookupDeveloperIdentityRequestMarshaller instance = LookupDeveloperIdentityRequestMarshaller.Instance;
			LookupDeveloperIdentityResponseUnmarshaller instance2 = LookupDeveloperIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse> responseObject = new AmazonServiceResult<LookupDeveloperIdentityRequest, LookupDeveloperIdentityResponse>((LookupDeveloperIdentityRequest)req, (LookupDeveloperIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual MergeDeveloperIdentitiesResponse MergeDeveloperIdentities(MergeDeveloperIdentitiesRequest request)
		{
			MergeDeveloperIdentitiesRequestMarshaller instance = MergeDeveloperIdentitiesRequestMarshaller.Instance;
			MergeDeveloperIdentitiesResponseUnmarshaller instance2 = MergeDeveloperIdentitiesResponseUnmarshaller.Instance;
			return Invoke<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse>(request, instance, instance2);
		}

		public virtual void MergeDeveloperIdentitiesAsync(MergeDeveloperIdentitiesRequest request, AmazonServiceCallback<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			MergeDeveloperIdentitiesRequestMarshaller instance = MergeDeveloperIdentitiesRequestMarshaller.Instance;
			MergeDeveloperIdentitiesResponseUnmarshaller instance2 = MergeDeveloperIdentitiesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse> responseObject = new AmazonServiceResult<MergeDeveloperIdentitiesRequest, MergeDeveloperIdentitiesResponse>((MergeDeveloperIdentitiesRequest)req, (MergeDeveloperIdentitiesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual SetIdentityPoolRolesResponse SetIdentityPoolRoles(SetIdentityPoolRolesRequest request)
		{
			SetIdentityPoolRolesRequestMarshaller instance = SetIdentityPoolRolesRequestMarshaller.Instance;
			SetIdentityPoolRolesResponseUnmarshaller instance2 = SetIdentityPoolRolesResponseUnmarshaller.Instance;
			return Invoke<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse>(request, instance, instance2);
		}

		public virtual void SetIdentityPoolRolesAsync(string identityPoolId, Dictionary<string, string> roles, AmazonServiceCallback<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			SetIdentityPoolRolesRequest setIdentityPoolRolesRequest = new SetIdentityPoolRolesRequest();
			setIdentityPoolRolesRequest.IdentityPoolId = identityPoolId;
			setIdentityPoolRolesRequest.Roles = roles;
			SetIdentityPoolRolesAsync(setIdentityPoolRolesRequest, callback, options);
		}

		public virtual void SetIdentityPoolRolesAsync(SetIdentityPoolRolesRequest request, AmazonServiceCallback<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			SetIdentityPoolRolesRequestMarshaller instance = SetIdentityPoolRolesRequestMarshaller.Instance;
			SetIdentityPoolRolesResponseUnmarshaller instance2 = SetIdentityPoolRolesResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse> responseObject = new AmazonServiceResult<SetIdentityPoolRolesRequest, SetIdentityPoolRolesResponse>((SetIdentityPoolRolesRequest)req, (SetIdentityPoolRolesResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual UnlinkDeveloperIdentityResponse UnlinkDeveloperIdentity(UnlinkDeveloperIdentityRequest request)
		{
			UnlinkDeveloperIdentityRequestMarshaller instance = UnlinkDeveloperIdentityRequestMarshaller.Instance;
			UnlinkDeveloperIdentityResponseUnmarshaller instance2 = UnlinkDeveloperIdentityResponseUnmarshaller.Instance;
			return Invoke<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse>(request, instance, instance2);
		}

		public virtual void UnlinkDeveloperIdentityAsync(UnlinkDeveloperIdentityRequest request, AmazonServiceCallback<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			UnlinkDeveloperIdentityRequestMarshaller instance = UnlinkDeveloperIdentityRequestMarshaller.Instance;
			UnlinkDeveloperIdentityResponseUnmarshaller instance2 = UnlinkDeveloperIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse> responseObject = new AmazonServiceResult<UnlinkDeveloperIdentityRequest, UnlinkDeveloperIdentityResponse>((UnlinkDeveloperIdentityRequest)req, (UnlinkDeveloperIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual UnlinkIdentityResponse UnlinkIdentity(UnlinkIdentityRequest request)
		{
			UnlinkIdentityRequestMarshaller instance = UnlinkIdentityRequestMarshaller.Instance;
			UnlinkIdentityResponseUnmarshaller instance2 = UnlinkIdentityResponseUnmarshaller.Instance;
			return Invoke<UnlinkIdentityRequest, UnlinkIdentityResponse>(request, instance, instance2);
		}

		public virtual void UnlinkIdentityAsync(UnlinkIdentityRequest request, AmazonServiceCallback<UnlinkIdentityRequest, UnlinkIdentityResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			UnlinkIdentityRequestMarshaller instance = UnlinkIdentityRequestMarshaller.Instance;
			UnlinkIdentityResponseUnmarshaller instance2 = UnlinkIdentityResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UnlinkIdentityRequest, UnlinkIdentityResponse> responseObject = new AmazonServiceResult<UnlinkIdentityRequest, UnlinkIdentityResponse>((UnlinkIdentityRequest)req, (UnlinkIdentityResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}

		internal virtual UpdateIdentityPoolResponse UpdateIdentityPool(UpdateIdentityPoolRequest request)
		{
			UpdateIdentityPoolRequestMarshaller instance = UpdateIdentityPoolRequestMarshaller.Instance;
			UpdateIdentityPoolResponseUnmarshaller instance2 = UpdateIdentityPoolResponseUnmarshaller.Instance;
			return Invoke<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse>(request, instance, instance2);
		}

		public virtual void UpdateIdentityPoolAsync(UpdateIdentityPoolRequest request, AmazonServiceCallback<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse> callback, AsyncOptions options = null)
		{
			options = ((options == null) ? new AsyncOptions() : options);
			UpdateIdentityPoolRequestMarshaller instance = UpdateIdentityPoolRequestMarshaller.Instance;
			UpdateIdentityPoolResponseUnmarshaller instance2 = UpdateIdentityPoolResponseUnmarshaller.Instance;
			Action<AmazonWebServiceRequest, AmazonWebServiceResponse, Exception, AsyncOptions> callbackHelper = null;
			if (callback != null)
			{
				callbackHelper = delegate(AmazonWebServiceRequest req, AmazonWebServiceResponse res, Exception ex, AsyncOptions ao)
				{
					AmazonServiceResult<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse> responseObject = new AmazonServiceResult<UpdateIdentityPoolRequest, UpdateIdentityPoolResponse>((UpdateIdentityPoolRequest)req, (UpdateIdentityPoolResponse)res, ex, ao.State);
					callback(responseObject);
				};
			}
			BeginInvoke(request, instance, instance2, options, callbackHelper);
		}
	}
}
