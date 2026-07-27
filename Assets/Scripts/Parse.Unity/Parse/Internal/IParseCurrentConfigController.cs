using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IParseCurrentConfigController
	{
		Task<ParseConfig> GetCurrentConfigAsync();

		Task SetCurrentConfigAsync(ParseConfig config);

		Task ClearCurrentConfigAsync();

		Task ClearCurrentConfigInMemoryAsync();
	}
}
