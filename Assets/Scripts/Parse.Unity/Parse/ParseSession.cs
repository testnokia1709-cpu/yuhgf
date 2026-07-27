using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	[ParseClassName("_Session")]
	public class ParseSession : ParseObject
	{
		private static readonly HashSet<string> readOnlyKeys = new HashSet<string> { "sessionToken", "createdWith", "restricted", "user", "expiresAt", "installationId" };

		[ParseFieldName("sessionToken")]
		public string SessionToken
		{
			get
			{
				return GetProperty<string>(null, "SessionToken");
			}
		}

		public static ParseQuery<ParseSession> Query
		{
			get
			{
				return new ParseQuery<ParseSession>();
			}
		}

		internal static IParseSessionController SessionController
		{
			get
			{
				return ParseCorePlugins.Instance.SessionController;
			}
		}

		internal override bool IsKeyMutable(string key)
		{
			return !readOnlyKeys.Contains(key);
		}

		public static Task<ParseSession> GetCurrentSessionAsync()
		{
			return GetCurrentSessionAsync(CancellationToken.None);
		}

		public static Task<ParseSession> GetCurrentSessionAsync(CancellationToken cancellationToken)
		{
			return ParseUser.GetCurrentUserAsync().OnSuccess(delegate(Task<ParseUser> t1)
			{
				ParseUser result = t1.Result;
				if (result == null)
				{
					return Task.FromResult<ParseSession>(null);
				}
				string sessionToken = result.SessionToken;
				return (sessionToken == null) ? Task.FromResult<ParseSession>(null) : SessionController.GetSessionAsync(sessionToken, cancellationToken).OnSuccess((Task<IObjectState> task) => ParseObject.FromState<ParseSession>(task.Result, "_Session"));
			}).Unwrap();
		}

		internal static Task RevokeAsync(string sessionToken, CancellationToken cancellationToken)
		{
			if (sessionToken == null || !IsRevocableSessionToken(sessionToken))
			{
				return Task.FromResult(0);
			}
			return SessionController.RevokeAsync(sessionToken, cancellationToken);
		}

		internal static Task<string> UpgradeToRevocableSessionAsync(string sessionToken, CancellationToken cancellationToken)
		{
			if (sessionToken == null || IsRevocableSessionToken(sessionToken))
			{
				return Task.FromResult(sessionToken);
			}
			return SessionController.UpgradeToRevocableSessionAsync(sessionToken, cancellationToken).OnSuccess((Task<IObjectState> t) => ParseObject.FromState<ParseSession>(t.Result, "_Session").SessionToken);
		}

		internal static bool IsRevocableSessionToken(string sessionToken)
		{
			return sessionToken.Contains("r:");
		}
	}
}
