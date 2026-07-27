using Amazon.Runtime;

namespace Amazon.S3
{
	public sealed class SseKmsEncryptedObjectsStatus : ConstantClass
	{
		public static readonly SseKmsEncryptedObjectsStatus Enabled = new SseKmsEncryptedObjectsStatus("Enabled");

		public static readonly SseKmsEncryptedObjectsStatus Disabled = new SseKmsEncryptedObjectsStatus("Disabled");

		public SseKmsEncryptedObjectsStatus(string value)
			: base(value)
		{
		}

		public static SseKmsEncryptedObjectsStatus FindValue(string value)
		{
			return ConstantClass.FindValue<SseKmsEncryptedObjectsStatus>(value);
		}

		public static implicit operator SseKmsEncryptedObjectsStatus(string value)
		{
			return FindValue(value);
		}
	}
}
