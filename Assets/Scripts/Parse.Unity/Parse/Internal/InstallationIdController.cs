using System;

namespace Parse.Internal
{
	internal class InstallationIdController : IInstallationIdController
	{
		private readonly object mutex = new object();

		private Guid? installationId;

		public void Set(Guid? installationId)
		{
			lock (mutex)
			{
				if (!installationId.HasValue)
				{
					ParseClient.PlatformHooks.ApplicationSettings.Remove("InstallationId");
				}
				else
				{
					ParseClient.PlatformHooks.ApplicationSettings["InstallationId"] = installationId.ToString();
				}
				this.installationId = installationId;
			}
		}

		public Guid? Get()
		{
			lock (mutex)
			{
				if (installationId.HasValue)
				{
					return installationId;
				}
				object value;
				ParseClient.PlatformHooks.ApplicationSettings.TryGetValue("InstallationId", out value);
				try
				{
					installationId = new Guid((string)value);
				}
				catch (Exception)
				{
					Guid value2 = Guid.NewGuid();
					Set(value2);
				}
				return installationId;
			}
		}

		public void Clear()
		{
			Set(null);
		}
	}
}
