using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class ParseCommandRunner : IParseCommandRunner
	{
		private readonly IHttpClient httpClient;

		public ParseCommandRunner(IHttpClient httpClient)
		{
			this.httpClient = httpClient;
		}

		public Task<Tuple<HttpStatusCode, IDictionary<string, object>>> RunCommandAsync(ParseCommand command, IProgress<ParseUploadProgressEventArgs> uploadProgress = null, IProgress<ParseDownloadProgressEventArgs> downloadProgress = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return httpClient.ExecuteAsync(command, uploadProgress, downloadProgress, cancellationToken).OnSuccess(delegate(Task<Tuple<HttpStatusCode, string>> t)
			{
				cancellationToken.ThrowIfCancellationRequested();
				Tuple<HttpStatusCode, string> result = t.Result;
				string item = result.Item2;
				int item2 = (int)result.Item1;
				if (item2 >= 500)
				{
					throw new ParseException(ParseException.ErrorCode.InternalServerError, result.Item2);
				}
				if (item != null)
				{
					IDictionary<string, object> dictionary = null;
					try
					{
						if (item.StartsWith("["))
						{
							object value = Json.Parse(item);
							dictionary = new Dictionary<string, object> { { "results", value } };
						}
						else
						{
							dictionary = Json.Parse(item) as IDictionary<string, object>;
						}
					}
					catch (Exception cause)
					{
						throw new ParseException(ParseException.ErrorCode.OtherCause, "Invalid response from server (" + result.Item2 + "):", cause);
					}
					if (item2 < 200 || item2 > 299)
					{
						int code = (int)(dictionary.ContainsKey("code") ? ((long)dictionary["code"]) : (-1));
						string message = (dictionary.ContainsKey("error") ? (dictionary["error"] as string) : item);
						throw new ParseException((ParseException.ErrorCode)code, message);
					}
					return new Tuple<HttpStatusCode, IDictionary<string, object>>(result.Item1, dictionary);
				}
				return new Tuple<HttpStatusCode, IDictionary<string, object>>(result.Item1, null);
			});
		}
	}
}
