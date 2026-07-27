using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseQueryController : IParseQueryController
	{
		public Task<IEnumerable<IObjectState>> FindAsync<T>(ParseQuery<T> query, ParseUser user, CancellationToken cancellationToken) where T : ParseObject
		{
			string sessionToken = ((user != null) ? user.SessionToken : null);
			return FindAsync(query.ClassName, query.BuildParameters(), sessionToken, cancellationToken).OnSuccess((Task<IDictionary<string, object>> t) => (t.Result["results"] as IList<object>).Select((object item) => ParseObjectCoder.Instance.Decode(item as IDictionary<string, object>, ParseDecoder.Instance)));
		}

		public Task<int> CountAsync<T>(ParseQuery<T> query, ParseUser user, CancellationToken cancellationToken) where T : ParseObject
		{
			string sessionToken = ((user != null) ? user.SessionToken : null);
			IDictionary<string, object> dictionary = query.BuildParameters();
			dictionary["limit"] = 0;
			dictionary["count"] = 1;
			return FindAsync(query.ClassName, dictionary, sessionToken, cancellationToken).OnSuccess((Task<IDictionary<string, object>> t) => Convert.ToInt32(t.Result["count"]));
		}

		public Task<IObjectState> FirstAsync<T>(ParseQuery<T> query, ParseUser user, CancellationToken cancellationToken) where T : ParseObject
		{
			string sessionToken = ((user != null) ? user.SessionToken : null);
			IDictionary<string, object> dictionary = query.BuildParameters();
			dictionary["limit"] = 1;
			return FindAsync(query.ClassName, dictionary, sessionToken, cancellationToken).OnSuccess(delegate(Task<IDictionary<string, object>> t)
			{
				IDictionary<string, object> dictionary2 = (t.Result["results"] as IList<object>).FirstOrDefault() as IDictionary<string, object>;
				return (dictionary2 == null) ? null : ParseObjectCoder.Instance.Decode(dictionary2, ParseDecoder.Instance);
			});
		}

		private Task<IDictionary<string, object>> FindAsync(string className, IDictionary<string, object> parameters, string sessionToken, CancellationToken cancellationToken)
		{
			ParseCommand command = new ParseCommand(string.Format("classes/{0}?{1}", Uri.EscapeDataString(className), ParseClient.BuildQueryString(parameters)), "GET", sessionToken, (IList<KeyValuePair<string, string>>)null, (IDictionary<string, object>)null);
			return ParseClient.ParseCommandRunner.RunCommandAsync(command, null, null, cancellationToken).OnSuccess((Task<Tuple<HttpStatusCode, IDictionary<string, object>>> t) => t.Result.Item2);
		}
	}
}
