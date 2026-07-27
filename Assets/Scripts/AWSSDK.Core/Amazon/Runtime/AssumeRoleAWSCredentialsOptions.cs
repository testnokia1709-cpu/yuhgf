using System;
using System.Net;

namespace Amazon.Runtime
{
	public class AssumeRoleAWSCredentialsOptions
	{
		public string ExternalId { get; set; }

		public string Policy { get; set; }

		public int? DurationSeconds { get; set; }

		public WebProxy ProxySettings { get; set; }

		public string MfaSerialNumber { get; set; }

		public string MfaTokenCode
		{
			get
			{
				if (string.IsNullOrEmpty(MfaSerialNumber))
				{
					return null;
				}
				if (MfaTokenCodeCallback == null)
				{
					throw new InvalidOperationException("The MfaSerialNumber has been set but the MfaTokenCodeCallback hasn't.  MfaTokenCodeCallback is required in order to determine the MfaTokenCode when MfaSerialNumber is set.");
				}
				return MfaTokenCodeCallback();
			}
		}

		public Func<string> MfaTokenCodeCallback { get; set; }
	}
}
