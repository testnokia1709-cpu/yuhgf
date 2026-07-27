using System;
using Amazon.Runtime;
using Amazon.Util.Internal;

namespace Amazon.S3
{
	public class AmazonS3Config : ClientConfig
	{
		private const string _accelerateEndpoint = "s3-accelerate.amazonaws.com";

		private const string _accelerateDualstackEndpoint = "s3-accelerate.dualstack.amazonaws.com";

		private bool forcePathStyle;

		private bool useAccelerateEndpoint;

		private static readonly string UserAgentString = InternalSDKUtils.BuildUserAgentString("3.3.17.3");

		private string _userAgent = UserAgentString;

		public bool ForcePathStyle
		{
			get
			{
				return forcePathStyle;
			}
			set
			{
				forcePathStyle = value;
			}
		}

		public bool UseAccelerateEndpoint
		{
			get
			{
				return useAccelerateEndpoint;
			}
			set
			{
				useAccelerateEndpoint = value;
			}
		}

		internal string AccelerateEndpoint
		{
			get
			{
				if (!base.UseDualstackEndpoint)
				{
					return "s3-accelerate.amazonaws.com";
				}
				return "s3-accelerate.dualstack.amazonaws.com";
			}
		}

		public override string RegionEndpointServiceName
		{
			get
			{
				return "s3";
			}
		}

		public override string ServiceVersion
		{
			get
			{
				return "2006-03-01";
			}
		}

		public override string UserAgent
		{
			get
			{
				return _userAgent;
			}
		}

		public override void Validate()
		{
			base.Validate();
			if (ForcePathStyle && UseAccelerateEndpoint)
			{
				throw new AmazonClientException("S3 accelerate is not compatible with Path style requests. Disable Path style requests using AmazonS3Config.ForcePathStyle property to use S3 accelerate.");
			}
			if (!string.IsNullOrEmpty(base.ServiceURL) && (base.ServiceURL.IndexOf("s3-accelerate.amazonaws.com", StringComparison.OrdinalIgnoreCase) >= 0 || base.ServiceURL.IndexOf("s3-accelerate.dualstack.amazonaws.com", StringComparison.OrdinalIgnoreCase) >= 0))
			{
				if (base.RegionEndpoint == null && string.IsNullOrEmpty(base.AuthenticationRegion))
				{
					throw new AmazonClientException("Specify a region using AmazonS3Config.RegionEndpoint or AmazonS3Config.AuthenticationRegion to use S3 accelerate.");
				}
				if (base.RegionEndpoint == null && !string.IsNullOrEmpty(base.AuthenticationRegion))
				{
					base.RegionEndpoint = RegionEndpoint.GetBySystemName(base.AuthenticationRegion);
				}
				UseAccelerateEndpoint = true;
			}
		}

		protected override void Initialize()
		{
			base.AllowAutoRedirect = false;
		}

		public AmazonS3Config()
		{
			base.AuthenticationServiceName = "s3";
		}
	}
}
