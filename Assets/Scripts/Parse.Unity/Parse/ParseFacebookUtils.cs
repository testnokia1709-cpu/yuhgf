using System;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public static class ParseFacebookUtils
	{
		private static readonly FacebookAuthenticationProvider authProvider = new FacebookAuthenticationProvider();

		public static string AccessToken
		{
			get
			{
				return authProvider.AccessToken;
			}
		}

		internal static void Initialize()
		{
			ParseUser.RegisterProvider(authProvider);
		}

		public static Task<ParseUser> LogInAsync(string facebookId, string accessToken, DateTime expiration, CancellationToken cancellationToken)
		{
			return ParseUser.LogInWithAsync("facebook", authProvider.GetAuthData(facebookId, accessToken, expiration), cancellationToken);
		}

		public static Task<ParseUser> LogInAsync(string facebookId, string accessToken, DateTime expiration)
		{
			return LogInAsync(facebookId, accessToken, expiration, CancellationToken.None);
		}

		public static Task LinkAsync(ParseUser user, string facebookId, string accessToken, DateTime expiration, CancellationToken cancellationToken)
		{
			return user.LinkWithAsync("facebook", authProvider.GetAuthData(facebookId, accessToken, expiration), cancellationToken);
		}

		public static Task LinkAsync(ParseUser user, string facebookId, string accessToken, DateTime expiration)
		{
			return LinkAsync(user, facebookId, accessToken, expiration, CancellationToken.None);
		}

		public static bool IsLinked(ParseUser user)
		{
			return user.IsLinked("facebook");
		}

		public static Task UnlinkAsync(ParseUser user, CancellationToken cancellationToken)
		{
			return user.UnlinkFromAsync("facebook", cancellationToken);
		}

		public static Task UnlinkAsync(ParseUser user)
		{
			return UnlinkAsync(user, CancellationToken.None);
		}
	}
}
