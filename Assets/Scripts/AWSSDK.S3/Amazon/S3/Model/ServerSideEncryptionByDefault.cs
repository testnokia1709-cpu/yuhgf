namespace Amazon.S3.Model
{
	public class ServerSideEncryptionByDefault
	{
		private ServerSideEncryptionMethod serverSideEncryptionAlgorithm;

		private string serverSideEncryptionKeyManagementServiceKeyId;

		public ServerSideEncryptionMethod ServerSideEncryptionAlgorithm
		{
			get
			{
				return serverSideEncryptionAlgorithm;
			}
			set
			{
				serverSideEncryptionAlgorithm = value;
			}
		}

		public string ServerSideEncryptionKeyManagementServiceKeyId
		{
			get
			{
				return serverSideEncryptionKeyManagementServiceKeyId;
			}
			set
			{
				serverSideEncryptionKeyManagementServiceKeyId = value;
			}
		}

		internal bool IsSetServerSideEncryptionAlgorithm()
		{
			return serverSideEncryptionAlgorithm != null;
		}

		internal bool IsSetServerSideEncryptionKeyManagementServiceKeyId()
		{
			return serverSideEncryptionKeyManagementServiceKeyId != null;
		}
	}
}
