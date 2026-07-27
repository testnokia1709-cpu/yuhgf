using System;
using System.Collections.Generic;

namespace Amazon.Runtime.SharedInterfaces
{
	public interface ICoreAmazonKMS : IDisposable
	{
		GenerateDataKeyResult GenerateDataKey(string keyID, Dictionary<string, string> encryptionContext, string keySpec);

		byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext);
	}
}
