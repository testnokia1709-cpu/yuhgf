using System;
using Amazon.Runtime;
using Amazon.SecurityToken.Model;

namespace Amazon.SecurityToken
{
	[Obsolete("This class has been replaced by Amazon.Runtime.AssumeRoleAWSCredentials and Amazon.Runtime.StoredProfileFederatedCredentials, and will be removed in a future version.", false)]
	public class STSAssumeRoleAWSCredentials : RefreshingAWSCredentials, IDisposable
	{
		private AmazonSecurityTokenServiceClient _stsClient;

		private AssumeRoleRequest _assumeRequest;

		private AssumeRoleWithSAMLRequest _assumeSamlRequest;

		private bool _isDisposed;

		private static TimeSpan _defaultPreemptExpiryTime = TimeSpan.FromMinutes(5.0);

		public STSAssumeRoleAWSCredentials(IAmazonSecurityTokenService sts, AssumeRoleRequest assumeRoleRequest)
		{
			if (sts == null)
			{
				throw new ArgumentNullException("sts");
			}
			if (assumeRoleRequest == null)
			{
				throw new ArgumentNullException("assumeRoleRequest");
			}
			_stsClient = (AmazonSecurityTokenServiceClient)sts;
			_assumeRequest = assumeRoleRequest;
			base.PreemptExpiryTime = _defaultPreemptExpiryTime;
		}

		public STSAssumeRoleAWSCredentials(AssumeRoleWithSAMLRequest assumeRoleWithSamlRequest)
		{
			if (assumeRoleWithSamlRequest == null)
			{
				throw new ArgumentNullException("assumeRoleWithSamlRequest");
			}
			_stsClient = new AmazonSecurityTokenServiceClient(new AnonymousAWSCredentials());
			_assumeSamlRequest = assumeRoleWithSamlRequest;
			base.PreemptExpiryTime = _defaultPreemptExpiryTime;
		}

		protected override CredentialsRefreshState GenerateNewCredentials()
		{
			Credentials serviceCredentials = GetServiceCredentials();
			return new CredentialsRefreshState
			{
				Expiration = serviceCredentials.Expiration,
				Credentials = serviceCredentials.GetCredentials()
			};
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing && _stsClient != null)
				{
					_stsClient.Dispose();
					_stsClient = null;
				}
				_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private Credentials GetServiceCredentials()
		{
			if (_assumeRequest != null)
			{
				return _stsClient.AssumeRole(_assumeRequest).Credentials;
			}
			return _stsClient.AssumeRoleWithSAML(_assumeSamlRequest).Credentials;
		}
	}
}
