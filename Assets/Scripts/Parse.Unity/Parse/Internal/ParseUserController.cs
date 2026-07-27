using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseUserController : IParseUserController
	{
		private readonly IParseCommandRunner commandRunner;

		internal ParseUserController(IParseCommandRunner commandRunner)
		{
			this.commandRunner = commandRunner;
		}

		public Task<IObjectState> SignUpAsync(IObjectState state, IDictionary<string, IParseFieldOperation> operations, CancellationToken cancellationToken)
		{
			IDictionary<string, object> data = ParseObject.ToJSONObjectForSaving(operations);
			ParseCommand command = new ParseCommand("classes/_User", "POST", null, null, data);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance).MutatedClone(delegate(MutableObjectState mutableClone)
			{
				mutableClone.IsNew = true;
			}));
		}

		public Task<IObjectState> LogInAsync(string username, string password, CancellationToken cancellationToken)
		{
			Dictionary<string, object> parameters = new Dictionary<string, object>
			{
				{ "username", username },
				{ "password", password }
			};
			ParseCommand command = new ParseCommand(string.Format("login?{0}", ParseClient.BuildQueryString(parameters)), "GET", (string)null, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance).MutatedClone(delegate(MutableObjectState mutableClone)
			{
				mutableClone.IsNew = t.Result.Item1 == HttpStatusCode.Created;
			}));
		}

		public Task<IObjectState> LogInAsync(string authType, IDictionary<string, object> data, CancellationToken cancellationToken)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary[authType] = data;
			ParseCommand command = new ParseCommand("users", "POST", null, null, new Dictionary<string, object> { { "authData", dictionary } });
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance).MutatedClone(delegate(MutableObjectState mutableClone)
			{
				mutableClone.IsNew = t.Result.Item1 == HttpStatusCode.Created;
			}));
		}

		public Task<IObjectState> GetUserAsync(string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("users/me", "GET", sessionToken, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => ParseObjectCoder.Instance.Decode(t.Result.Item2, ParseDecoder.Instance));
		}

		public Task RequestPasswordResetAsync(string email, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand("requestPasswordReset", "POST", null, null, new Dictionary<string, object> { { "email", email } });
			return commandRunner.RunCommandAsync(command, null, null, cancellationToken);
		}
	}
}
