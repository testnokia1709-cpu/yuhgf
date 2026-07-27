using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Amazon.Runtime
{
	public static class FallbackCredentialsFactory
	{
		public delegate AWSCredentials CredentialsGenerator();

		private static AWSCredentials cachedCredentials;

		public static List<CredentialsGenerator> CredentialsGenerators { get; set; }

		static FallbackCredentialsFactory()
		{
			Reset();
		}

		public static void Reset()
		{
			cachedCredentials = null;
			CredentialsGenerators = new List<CredentialsGenerator>();
		}

		public static AWSCredentials GetCredentials()
		{
			return GetCredentials(false);
		}

		public static AWSCredentials GetCredentials(bool fallbackToAnonymous)
		{
			if (cachedCredentials != null)
			{
				return cachedCredentials;
			}
			List<Exception> list = new List<Exception>();
			foreach (CredentialsGenerator credentialsGenerator in CredentialsGenerators)
			{
				try
				{
					cachedCredentials = credentialsGenerator();
				}
				catch (Exception item)
				{
					cachedCredentials = null;
					list.Add(item);
				}
				if (cachedCredentials != null)
				{
					break;
				}
			}
			if (cachedCredentials == null)
			{
				if (fallbackToAnonymous)
				{
					return new AnonymousAWSCredentials();
				}
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					stringWriter.WriteLine("Unable to find credentials");
					stringWriter.WriteLine();
					for (int i = 0; i < list.Count; i++)
					{
						Exception ex = list[i];
						stringWriter.WriteLine("Exception {0} of {1}:", i + 1, list.Count);
						stringWriter.WriteLine(ex.ToString());
						stringWriter.WriteLine();
					}
					throw new AmazonServiceException(stringWriter.ToString());
				}
			}
			return cachedCredentials;
		}
	}
}
