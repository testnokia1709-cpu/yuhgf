namespace Amazon.S3.Model
{
	public class SSEKMS
	{
		private string keyId;

		public string KeyId
		{
			get
			{
				return keyId;
			}
			set
			{
				keyId = value;
			}
		}

		internal bool IsSetKeyId()
		{
			return keyId != null;
		}
	}
}
