namespace Amazon.S3.Model
{
	public class ServerSideEncryptionRule
	{
		private ServerSideEncryptionByDefault serverSideEncryptionByDefault;

		public ServerSideEncryptionByDefault ServerSideEncryptionByDefault
		{
			get
			{
				return serverSideEncryptionByDefault;
			}
			set
			{
				serverSideEncryptionByDefault = value;
			}
		}

		internal bool IsSetServerSideEncryptionByDefault()
		{
			return serverSideEncryptionByDefault != null;
		}
	}
}
