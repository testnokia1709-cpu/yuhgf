using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParsePushChannelsController : IParsePushChannelsController
	{
		public Task SubscribeAsync(IEnumerable<string> channels, CancellationToken cancellationToken)
		{
			ParseInstallation currentInstallation = ParseInstallation.CurrentInstallation;
			currentInstallation.AddRangeUniqueToList("channels", channels);
			return currentInstallation.SaveAsync(cancellationToken);
		}

		public Task UnsubscribeAsync(IEnumerable<string> channels, CancellationToken cancellationToken)
		{
			ParseInstallation currentInstallation = ParseInstallation.CurrentInstallation;
			currentInstallation.RemoveAllFromList("channels", channels);
			return currentInstallation.SaveAsync(cancellationToken);
		}
	}
}
