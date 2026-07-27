using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseConfigController : IParseConfigController
	{
		public IParseCurrentConfigController CurrentConfigController { get; internal set; }

		public ParseConfigController()
		{
			CurrentConfigController = new ParseCurrentConfigController();
		}

		public Task<ParseConfig> FetchConfigAsync(string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("config", "GET", sessionToken, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return ParseClient.ParseCommandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess(delegate(Task<Tuple<HttpStatusCode, IDictionary<string, object>>> task)
			{
				cancellationToken.ThrowIfCancellationRequested();
				return new ParseConfig(task.Result.Item2);
			}).OnSuccess(delegate(Task<ParseConfig> task)
			{
				cancellationToken.ThrowIfCancellationRequested();
				CurrentConfigController.SetCurrentConfigAsync(task.Result);
				return task;
			})
				.Unwrap();
		}
	}
}
