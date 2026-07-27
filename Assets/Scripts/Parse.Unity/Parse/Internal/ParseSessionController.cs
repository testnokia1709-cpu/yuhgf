using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseSessionController : IParseSessionController
	{
		private readonly IParseCommandRunner commandRunner;

		internal ParseSessionController(IParseCommandRunner commandRunner)
		{
			this.commandRunner = commandRunner;
		}

		public Task<IObjectState> GetSessionAsync(string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("sessions/me", "GET", sessionToken, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance));
		}

		public Task RevokeAsync(string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("logout", "POST", sessionToken, null, new Dictionary<string, object>());
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken);
		}

		public Task<IObjectState> UpgradeToRevocableSessionAsync(string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("upgradeToRevocableSession", "POST", sessionToken, null, new Dictionary<string, object>());
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance));
		}
	}
}
