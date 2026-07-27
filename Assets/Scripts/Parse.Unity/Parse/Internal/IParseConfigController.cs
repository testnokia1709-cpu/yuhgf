using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IParseConfigController
	{
		IParseCurrentConfigController CurrentConfigController { get; }

		Task<ParseConfig> FetchConfigAsync(string sessionToken, CancellationToken cancellationToken);
	}
}
