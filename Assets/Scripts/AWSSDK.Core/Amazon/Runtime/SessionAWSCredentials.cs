using System;
using Amazon.Runtime.Internal.Util;
using Amazon.Util;

namespace Amazon.Runtime
{
	public class SessionAWSCredentials : AWSCredentials
	{
		private ImmutableCredentials _lastCredentials;

		public SessionAWSCredentials(string awsAccessKeyId, string awsSecretAccessKey, string token)
		{
			if (string.IsNullOrEmpty(awsAccessKeyId))
			{
				throw new ArgumentNullException("awsAccessKeyId");
			}
			if (string.IsNullOrEmpty(awsSecretAccessKey))
			{
				throw new ArgumentNullException("awsSecretAccessKey");
			}
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentNullException("token");
			}
			_lastCredentials = new ImmutableCredentials(awsAccessKeyId, awsSecretAccessKey, token);
		}

		public override ImmutableCredentials GetCredentials()
		{
			return _lastCredentials.Copy();
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			SessionAWSCredentials sessionAWSCredentials = obj as SessionAWSCredentials;
			if (sessionAWSCredentials == null)
			{
				return false;
			}
			return AWSSDKUtils.AreEqual(new object[1] { _lastCredentials }, new object[1] { sessionAWSCredentials._lastCredentials });
		}

		public override int GetHashCode()
		{
			return Hashing.Hash(_lastCredentials);
		}
	}
}
