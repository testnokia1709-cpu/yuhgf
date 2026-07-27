using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IPlatformHooks
	{
		IDictionary<string, object> ApplicationSettings { get; }

		IHttpClient HttpClient { get; }

		string SDKName { get; }

		string AppName { get; }

		string AppBuildVersion { get; }

		string AppDisplayVersion { get; }

		string AppIdentifier { get; }

		string OSVersion { get; }

		string DeviceType { get; }

		string DeviceTimeZone { get; }

		void Initialize();

		Task ExecuteParseInstallationSaveHookAsync(ParseInstallation installation);
	}
}
