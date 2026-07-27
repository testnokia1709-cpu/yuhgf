namespace Amazon.S3.Model
{
	public class EncryptionConfiguration
	{
		private string replicaKmsKeyID;

		public string ReplicaKmsKeyID
		{
			get
			{
				return replicaKmsKeyID;
			}
			set
			{
				replicaKmsKeyID = value;
			}
		}

		internal bool isSetReplicaKmsKeyID()
		{
			return replicaKmsKeyID != null;
		}
	}
}
