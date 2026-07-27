using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal interface IParseCurrentUserController : IParseObjectCurrentController<ParseUser>
	{
		Task<string> GetCurrentSessionTokenAsync(CancellationToken cancellationToken);

		Task LogOutAsync(CancellationToken cancellationToken);
	}
}
