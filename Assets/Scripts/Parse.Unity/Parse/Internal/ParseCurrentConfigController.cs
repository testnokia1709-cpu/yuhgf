using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseCurrentConfigController : IParseCurrentConfigController
	{
		private const string CurrentConfigKey = "CurrentConfig";

		private readonly TaskQueue taskQueue;

		private ParseConfig currentConfig;

		public ParseCurrentConfigController()
		{
			taskQueue = new TaskQueue();
		}

		public Task<ParseConfig> GetCurrentConfigAsync()
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				if (currentConfig == null)
				{
					object value;
					ParseClient.ApplicationSettings.TryGetValue("CurrentConfig", out value);
					string text = value as string;
					if (text != null)
					{
						IDictionary<string, object> fetchedConfig = ParseClient.DeserializeJsonString(text);
						currentConfig = new ParseConfig(fetchedConfig);
					}
					else
					{
						currentConfig = new ParseConfig();
					}
				}
				return currentConfig;
			}), CancellationToken.None);
		}

		public Task SetCurrentConfigAsync(ParseConfig config)
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				currentConfig = config;
				string value = ParseClient.SerializeJsonString(((IJsonConvertible)config).ToJSON());
				ParseClient.ApplicationSettings["CurrentConfig"] = value;
			}), CancellationToken.None);
		}

		public Task ClearCurrentConfigAsync()
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				currentConfig = null;
				ParseClient.ApplicationSettings.Remove("CurrentConfig");
			}), CancellationToken.None);
		}

		public Task ClearCurrentConfigInMemoryAsync()
		{
			return taskQueue.Enqueue((Task toAwait) => toAwait.ContinueWith(delegate
			{
				currentConfig = null;
			}), CancellationToken.None);
		}
	}
}
