namespace Amazon.S3.Model
{
	public class ReplicationDestination
	{
		private string bucketArn;

		private S3StorageClass storageClass;

		private EncryptionConfiguration encryptionConfiguration;

		private AccessControlTranslation accessControlTranslation;

		private string accountId;

		public string BucketArn
		{
			get
			{
				return bucketArn;
			}
			set
			{
				bucketArn = value;
			}
		}

		public S3StorageClass StorageClass
		{
			get
			{
				return storageClass;
			}
			set
			{
				storageClass = value;
			}
		}

		public string AccountId
		{
			get
			{
				return accountId;
			}
			set
			{
				accountId = value;
			}
		}

		public AccessControlTranslation AccessControlTranslation
		{
			get
			{
				return accessControlTranslation;
			}
			set
			{
				accessControlTranslation = value;
			}
		}

		public EncryptionConfiguration EncryptionConfiguration
		{
			get
			{
				return encryptionConfiguration;
			}
			set
			{
				encryptionConfiguration = value;
			}
		}

		internal bool IsSetBucketArn()
		{
			return !string.IsNullOrEmpty(bucketArn);
		}

		internal bool IsSetStorageClass()
		{
			return storageClass != null;
		}

		public bool IsSetAccountId()
		{
			return !string.IsNullOrEmpty(accountId);
		}

		public bool IsSetAccessControlTranslation()
		{
			return accessControlTranslation != null;
		}

		public bool IsSetEncryptionConfiguration()
		{
			return encryptionConfiguration != null;
		}
	}
}
