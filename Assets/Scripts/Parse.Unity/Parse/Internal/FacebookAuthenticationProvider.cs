using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Parse.Internal
{
	internal class FacebookAuthenticationProvider : IParseAuthenticationProvider
	{
		internal static readonly Uri LoginDialogUrl = new Uri("https://www.facebook.com/dialog/oauth", UriKind.Absolute);

		private static readonly Uri TokenExtensionUrl = new Uri("https://graph.facebook.com/oauth/access_token", UriKind.Absolute);

		internal static readonly Uri ResponseUrl = new Uri("https://www.facebook.com/connect/login_success.html", UriKind.Absolute);

		private static readonly Uri MeUrl = new Uri("https://graph.facebook.com/me", UriKind.Absolute);

		private TaskCompletionSource<IDictionary<string, object>> pendingTask;

		private CancellationToken pendingCancellationToken;

		internal Uri LoginDialogUrlOverride { get; set; }

		internal Uri ResponseUrlOverride { get; set; }

		public IEnumerable<string> Permissions { get; set; }

		public string AppId { get; set; }

		public string AccessToken { get; set; }

		public string AuthType
		{
			get
			{
				return "facebook";
			}
		}

		public event Action<Uri> Navigate;

		private bool TryParseOAuthCallbackUrl(Uri uri, out IDictionary<string, string> result)
		{
			if (!uri.AbsoluteUri.StartsWith((ResponseUrlOverride ?? ResponseUrl).AbsoluteUri) || uri.Fragment == null)
			{
				result = null;
				return false;
			}
			string text = (string.IsNullOrEmpty(uri.Fragment) ? uri.Query : uri.Fragment);
			result = ParseClient.DecodeQueryString(text.Substring(1));
			return true;
		}

		public IDictionary<string, object> GetAuthData(string facebookId, string accessToken, DateTime expiration)
		{
			return new Dictionary<string, object>
			{
				{ "id", facebookId },
				{ "access_token", accessToken },
				{
					"expiration_date",
					expiration.ToString(ParseClient.DateFormatStrings.First())
				}
			};
		}

		public bool HandleNavigation(Uri uri)
		{
			IDictionary<string, string> result;
			if (TryParseOAuthCallbackUrl(uri, out result))
			{
				((Action)delegate
				{
					try
					{
						if (result.ContainsKey("error"))
						{
							pendingTask.TrySetException(new ParseException(ParseException.ErrorCode.OtherCause, string.Format("{0}: {1}", result["error_description"], result["error"])));
						}
						else
						{
							Dictionary<string, object> dictionary = new Dictionary<string, object>();
							dictionary["access_token"] = result["access_token"];
							dictionary["fields"] = "id";
							HttpRequest httpRequest = new HttpRequest
							{
								Uri = new Uri(MeUrl, "?" + ParseClient.BuildQueryString(dictionary)),
								Method = "GET"
							};
							ParseClient.PlatformHooks.HttpClient.ExecuteAsync(httpRequest, null, null, CancellationToken.None).OnSuccess(delegate(Task<Tuple<HttpStatusCode, string>> t)
							{
								IDictionary<string, object> dictionary2 = ParseClient.DeserializeJsonString(t.Result.Item2);
								pendingTask.TrySetResult(GetAuthData(dictionary2["id"] as string, result["access_token"], DateTime.Now + TimeSpan.FromSeconds(int.Parse(result["expires_in"]))));
							}).ContinueWith(delegate(Task t)
							{
								if (t.IsFaulted)
								{
									pendingTask.TrySetException(t.Exception);
								}
							});
						}
					}
					catch (Exception exception)
					{
						pendingTask.TrySetException(exception);
					}
				})();
				return true;
			}
			return false;
		}

		public Task<IDictionary<string, object>> AuthenticateAsync(CancellationToken cancellationToken)
		{
			if (AppId == null)
			{
				throw new InvalidOperationException("You must initialize ParseFacebookUtils before attempting a Facebook login.");
			}
			if (pendingTask != null)
			{
				pendingTask.TrySetCanceled();
			}
			TaskCompletionSource<IDictionary<string, object>> tcs = new TaskCompletionSource<IDictionary<string, object>>();
			pendingCancellationToken = cancellationToken;
			pendingTask = tcs;
			cancellationToken.Register(delegate
			{
				tcs.TrySetCanceled();
			});
			Action<Uri> action = this.Navigate;
			if (action != null)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"redirect_uri",
						(ResponseUrlOverride ?? ResponseUrl).AbsoluteUri
					},
					{ "response_type", "token" },
					{ "display", "popup" },
					{ "client_id", AppId }
				};
				if (Permissions != null)
				{
					dictionary["scope"] = string.Join(",", Permissions.ToArray());
				}
				action(new Uri(LoginDialogUrlOverride ?? LoginDialogUrl, "?" + ParseClient.BuildQueryString(dictionary)));
			}
			return tcs.Task;
		}

		public void Deauthenticate()
		{
			AccessToken = null;
		}

		public bool RestoreAuthentication(IDictionary<string, object> authData)
		{
			if (authData == null)
			{
				Deauthenticate();
			}
			else
			{
				AccessToken = authData["access_token"] as string;
			}
			return true;
		}
	}
}
