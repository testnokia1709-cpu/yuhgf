using System;
using System.Globalization;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using Amazon.Runtime.SharedInterfaces;

namespace Amazon.Runtime
{
	public class AssumeRoleAWSCredentials : RefreshingAWSCredentials
	{
		private RegionEndpoint DefaultSTSClientRegion = RegionEndpoint.USEast1;

		public AWSCredentials SourceCredentials { get; private set; }

		public string RoleArn { get; private set; }

		public string RoleSessionName { get; private set; }

		public AssumeRoleAWSCredentialsOptions Options { get; private set; }

		public AssumeRoleAWSCredentials(AWSCredentials sourceCredentials, string roleArn, string roleSessionName)
			: this(sourceCredentials, roleArn, roleSessionName, new AssumeRoleAWSCredentialsOptions())
		{
		}

		public AssumeRoleAWSCredentials(AWSCredentials sourceCredentials, string roleArn, string roleSessionName, AssumeRoleAWSCredentialsOptions options)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}
			SourceCredentials = sourceCredentials;
			RoleArn = roleArn;
			RoleSessionName = roleSessionName;
			Options = options;
		}

		protected override CredentialsRefreshState GenerateNewCredentials()
		{
			string aWSRegion = AWSConfigs.AWSRegion;
			RegionEndpoint regionEndpoint = (string.IsNullOrEmpty(aWSRegion) ? DefaultSTSClientRegion : RegionEndpoint.GetBySystemName(aWSRegion));
			ICoreAmazonSTS coreAmazonSTS = null;
			try
			{
				ClientConfig clientConfig = ServiceClientHelpers.CreateServiceConfig("AWSSDK.SecurityToken", "Amazon.SecurityToken.AmazonSecurityTokenServiceConfig");
				clientConfig.RegionEndpoint = regionEndpoint;
				if (Options != null && Options.ProxySettings != null)
				{
					clientConfig.SetWebProxy(Options.ProxySettings);
				}
				coreAmazonSTS = ServiceClientHelpers.CreateServiceFromAssembly<ICoreAmazonSTS>("AWSSDK.SecurityToken", "Amazon.SecurityToken.AmazonSecurityTokenServiceClient", SourceCredentials, clientConfig);
			}
			catch (Exception innerException)
			{
				InvalidOperationException ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Assembly {0} could not be found or loaded. This assembly must be available at runtime to use Amazon.Runtime.AssumeRoleAWSCredentials.", "AWSSDK.SecurityToken"), innerException);
				Logger.GetLogger(typeof(AssumeRoleAWSCredentials)).Error(ex, ex.Message);
				throw ex;
			}
			AssumeRoleImmutableCredentials assumeRoleImmutableCredentials = coreAmazonSTS.CredentialsFromAssumeRoleAuthentication(RoleArn, RoleSessionName, Options);
			return new CredentialsRefreshState(assumeRoleImmutableCredentials, assumeRoleImmutableCredentials.Expiration);
		}
	}
}
