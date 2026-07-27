using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParsePushController : IParsePushController
	{
		public Task SendPushNotificationAsync(IPushState state, string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("push", "POST", sessionToken, null, ParsePushEncoder.Instance.Encode(state));
			return ParseClient.ParseCommandRunner.RunCommandAsync(command, null, null, cancellationToken);
		}
	}
}
