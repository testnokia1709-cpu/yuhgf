using Amazon.Runtime;
using Amazon.Util.Internal;

namespace Amazon.SecurityToken
{
	public class AmazonSecurityTokenServiceConfig : ClientConfig
	{
		private static readonly string UserAgentString = InternalSDKUtils.BuildUserAgentString("3.3.4.1");

		private string _userAgent = UserAgentString;

		public override string RegionEndpointServiceName
		{
			get
			{
				return "sts";
			}
		}

		public override string ServiceVersion
		{
			get
			{
				return "2011-06-15";
			}
		}

		public override string UserAgent
		{
			get
			{
				return _userAgent;
			}
		}

		public AmazonSecurityTokenServiceConfig()
		{
			base.AuthenticationServiceName = "sts";
			RegionEndpoint regionEndpoint = FallbackRegionFactory.GetRegionEndpoint(false);
			base.RegionEndpoint = regionEndpoint ?? RegionEndpoint.USEast1;
		}
	}
}
