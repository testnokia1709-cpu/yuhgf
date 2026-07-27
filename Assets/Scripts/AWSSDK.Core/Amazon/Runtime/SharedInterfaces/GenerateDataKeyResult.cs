namespace Amazon.Runtime.SharedInterfaces
{
	public class GenerateDataKeyResult
	{
		public byte[] KeyPlaintext { get; set; }

		public byte[] KeyCiphertext { get; set; }
	}
}
