using System;
using System.Collections.Generic;
using System.Globalization;
using Amazon.Runtime.Internal;

namespace Amazon.Runtime.SharedInterfaces.Internal
{
	public class CoreAmazonKMS : ICoreAmazonKMS, IDisposable
	{
		private object wrappedClientLock = new object();

		private ICoreAmazonKMS wrappedClient;

		private AmazonServiceClient existingClient;

		private string feature;

		private bool disposed;

		public CoreAmazonKMS(AmazonServiceClient existingClient, string feature)
		{
			this.existingClient = existingClient;
			this.feature = feature;
		}

		public byte[] Decrypt(byte[] ciphertextBlob, Dictionary<string, string> encryptionContext)
		{
			EnsureWrappedClientIsInstantiated();
			return wrappedClient.Decrypt(ciphertextBlob, encryptionContext);
		}

		public GenerateDataKeyResult GenerateDataKey(string keyID, Dictionary<string, string> encryptionContext, string keySpec)
		{
			EnsureWrappedClientIsInstantiated();
			return wrappedClient.GenerateDataKey(keyID, encryptionContext, keySpec);
		}

		private void EnsureWrappedClientIsInstantiated()
		{
			if (wrappedClient != null)
			{
				return;
			}
			lock (wrappedClientLock)
			{
				if (wrappedClient == null)
				{
					wrappedClient = CreateFromExistingClient(existingClient, feature);
				}
			}
		}

		private static ICoreAmazonKMS CreateFromExistingClient(AmazonServiceClient existingClient, string feature)
		{
			ICoreAmazonKMS coreAmazonKMS = null;
			try
			{
				return ServiceClientHelpers.CreateServiceFromAssembly<ICoreAmazonKMS>("AWSSDK.KeyManagementService", "Amazon.KeyManagementService.AmazonKeyManagementServiceClient", existingClient);
			}
			catch (Exception innerException)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Error instantiating {0} from assembly {1}.  The assembly and class must be available at runtime in order to use {2}.", "Amazon.KeyManagementService.AmazonKeyManagementServiceClient", "AWSSDK.KeyManagementService", feature), innerException);
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposed || !disposing)
			{
				return;
			}
			lock (wrappedClientLock)
			{
				if (wrappedClient != null)
				{
					wrappedClient.Dispose();
					wrappedClient = null;
				}
			}
			disposed = true;
		}
	}
}
