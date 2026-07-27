using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseAnalyticsController : IParseAnalyticsController
	{
		private readonly IParseCommandRunner commandRunner;

		internal ParseAnalyticsController(IParseCommandRunner commandRunner)
		{
			this.commandRunner = commandRunner;
		}

		public Task TrackEventAsync(string name, IDictionary<string, string> dimensions, string sessionToken, CancellationToken cancellationToken)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{
					"at",
					DateTime.Now
				},
				{ "name", name }
			};
			if (dimensions != null)
			{
				dictionary["dimensions"] = dimensions;
			}
			ParseCommand command = new ParseCommand("events/" + name, "POST", sessionToken, null, PointerOrLocalIdEncoder.Instance.Encode(dictionary) as IDictionary<string, object>);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken);
		}

		public Task TrackAppOpenedAsync(string pushHash, string sessionToken, CancellationToken cancellationToken)
		{
			IDictionary<string, object> dictionary = new Dictionary<string, object> { 
			{
				"at",
				DateTime.Now
			} };
			if (pushHash != null)
			{
				dictionary["push_hash"] = pushHash;
			}
			ParseCommand command = new ParseCommand("events/AppOpened", "POST", sessionToken, null, PointerOrLocalIdEncoder.Instance.Encode(dictionary) as IDictionary<string, object>);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken);
		}
	}
}
