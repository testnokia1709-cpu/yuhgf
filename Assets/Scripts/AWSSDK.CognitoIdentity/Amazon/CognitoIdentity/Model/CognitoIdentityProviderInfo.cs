namespace Amazon.CognitoIdentity.Model
{
	public class CognitoIdentityProviderInfo
	{
		private string _clientId;

		private string _providerName;

		private bool? _serverSideTokenCheck;

		public string ClientId
		{
			get
			{
				return _clientId;
			}
			set
			{
				_clientId = value;
			}
		}

		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
			}
		}

		public bool ServerSideTokenCheck
		{
			get
			{
				return _serverSideTokenCheck == true;
			}
			set
			{
				_serverSideTokenCheck = value;
			}
		}

		internal bool IsSetClientId()
		{
			return _clientId != null;
		}

		internal bool IsSetProviderName()
		{
			return _providerName != null;
		}

		internal bool IsSetServerSideTokenCheck()
		{
			return _serverSideTokenCheck.HasValue;
		}
	}
}
