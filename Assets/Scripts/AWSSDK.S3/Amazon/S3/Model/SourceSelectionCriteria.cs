namespace Amazon.S3.Model
{
	public class SourceSelectionCriteria
	{
		private SseKmsEncryptedObjects sseKmsEncryptedObjects;

		public SseKmsEncryptedObjects SseKmsEncryptedObjects
		{
			get
			{
				return sseKmsEncryptedObjects;
			}
			set
			{
				sseKmsEncryptedObjects = value;
			}
		}

		internal bool IsSetSseKmsEncryptedObjects()
		{
			return sseKmsEncryptedObjects != null;
		}
	}
}
