using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IParsePushController
	{
		Task SendPushNotificationAsync(IPushState state, string sessionToken, CancellationToken cancellationToken);
	}
}
