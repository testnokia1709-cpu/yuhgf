using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class ServerSideEncryptionConfiguration
	{
		private List<ServerSideEncryptionRule> serverSideEncryptionRules = new List<ServerSideEncryptionRule>();

		public List<ServerSideEncryptionRule> ServerSideEncryptionRules
		{
			get
			{
				return serverSideEncryptionRules;
			}
			set
			{
				serverSideEncryptionRules = value;
			}
		}

		internal bool IsSetServerSideEncryptionRules()
		{
			if (serverSideEncryptionRules != null)
			{
				return serverSideEncryptionRules.Count > 0;
			}
			return false;
		}
	}
}
