namespace Amazon.S3.Model
{
	public class SseKmsEncryptedObjects
	{
		private SseKmsEncryptedObjectsStatus sseKmsEncryptedObjectsStatus;

		public SseKmsEncryptedObjectsStatus SseKmsEncryptedObjectsStatus
		{
			get
			{
				return sseKmsEncryptedObjectsStatus;
			}
			set
			{
				sseKmsEncryptedObjectsStatus = value;
			}
		}

		internal bool IsSetSseKmsEncryptedObjectsStatus()
		{
			return sseKmsEncryptedObjectsStatus != null;
		}
	}
}
