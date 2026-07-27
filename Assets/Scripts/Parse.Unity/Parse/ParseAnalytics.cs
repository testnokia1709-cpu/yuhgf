using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;

namespace Parse
{
	public class ParseAnalytics
	{
		internal static IParseAnalyticsController AnalyticsController
		{
			get
			{
				return ParseCorePlugins.Instance.AnalyticsController;
			}
		}

		public static Task TrackAppOpenedAsync()
		{
			return TrackAppOpenedWithPushHashAsync();
		}

		public static Task TrackEventAsync(string name)
		{
			return TrackEventAsync(name, null);
		}

		public static Task TrackEventAsync(string name, IDictionary<string, string> dimensions)
		{
			if (name == null || name.Trim().Length == 0)
			{
				throw new ArgumentException("A name for the custom event must be provided.");
			}
			return AnalyticsController.TrackEventAsync(name, dimensions, ParseUser.CurrentSessionToken, CancellationToken.None);
		}

		private static Task TrackAppOpenedWithPushHashAsync(string pushHash = null)
		{
			return AnalyticsController.TrackAppOpenedAsync(pushHash, ParseUser.CurrentSessionToken, CancellationToken.None);
		}
	}
}
