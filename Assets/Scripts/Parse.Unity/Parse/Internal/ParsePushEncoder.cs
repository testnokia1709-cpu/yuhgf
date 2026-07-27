using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal class ParsePushEncoder
	{
		private static readonly ParsePushEncoder instance = new ParsePushEncoder();

		public static ParsePushEncoder Instance
		{
			get
			{
				return instance;
			}
		}

		private ParsePushEncoder()
		{
		}

		public IDictionary<string, object> Encode(IPushState state)
		{
			if (state.Alert == null && state.Data == null)
			{
				throw new InvalidOperationException("A push must have either an Alert or Data");
			}
			if (state.Channels == null && state.Query == null)
			{
				throw new InvalidOperationException("A push must have either Channels or a Query");
			}
			IDictionary<string, object> value = state.Data ?? new Dictionary<string, object> { { "alert", state.Alert } };
			ParseQuery<ParseInstallation> parseQuery = state.Query ?? ParseInstallation.Query;
			if (state.Channels != null)
			{
				parseQuery = parseQuery.WhereContainedIn("channels", state.Channels);
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>
			{
				{ "data", value },
				{
					"where",
					parseQuery.BuildParameters().GetOrDefault("where", new Dictionary<string, object>())
				}
			};
			if (state.Expiration.HasValue)
			{
				dictionary["expiration_time"] = state.Expiration.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
			}
			else if (state.ExpirationInterval.HasValue)
			{
				dictionary["expiration_interval"] = state.ExpirationInterval.Value.TotalSeconds;
			}
			if (state.PushTime.HasValue)
			{
				dictionary["push_time"] = state.PushTime.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
			}
			return dictionary;
		}
	}
}
