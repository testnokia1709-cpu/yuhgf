using Amazon.Runtime;
using Amazon.Util.Internal;

namespace Amazon.CognitoIdentity
{
	public class AmazonCognitoIdentityConfig : ClientConfig
	{
		private static readonly string UserAgentString = InternalSDKUtils.BuildUserAgentString("3.3.2.20");

		private string _userAgent = UserAgentString;

		public override string RegionEndpointServiceName
		{
			get
			{
				return "cognito-identity";
			}
		}

		public override string ServiceVersion
		{
			get
			{
				return "2014-06-30";
			}
		}

		public override string UserAgent
		{
			get
			{
				return _userAgent;
			}
		}

		public AmazonCognitoIdentityConfig()
		{
			base.AuthenticationServiceName = "cognito-identity";
		}
	}
}
