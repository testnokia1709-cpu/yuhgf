using System;

namespace Parse.Internal
{
	internal interface IInstallationIdController
	{
		void Set(Guid? installationId);

		Guid? Get();

		void Clear();
	}
}
