using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class ParseCloudCodeController : IParseCloudCodeController
	{
		private readonly IParseCommandRunner commandRunner;

		internal ParseCloudCodeController(IParseCommandRunner commandRunner)
		{
			this.commandRunner = commandRunner;
		}

		public Task<T> CallFunctionAsync<T>(string name, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand(string.Format("functions/{0}", Uri.EscapeUriString(name)), "POST", sessionToken, null, NoObjectsEncoder.Instance.Encode(parameters) as IDictionary<string, object>);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess(delegate(Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t)
			{
				IDictionary<string, object> dictionary = ParseDecoder.Instance.Decode(t.Result.Item2) as IDictionary<string, object>;
				return (!dictionary.ContainsKey("result")) ? default(T) : ((T)Conversion.ConvertTo<T>(dictionary["result"]));
			});
		}
	}
}
