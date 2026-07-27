using System;
using CloudOnce.Internal.Utils;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Providers
{
	public class GenericLeaderboardsWrapper
	{
		public void SubmitScore(string leaderboardId, long score, Action<CloudRequestResult<bool>> onComplete = null)
		{
			CloudOnceUtils.LeaderboardUtils.SubmitScore(leaderboardId, score, onComplete, string.Empty);
		}

		public void ShowOverlay(string leaderboardID = "")
		{
			CloudOnceUtils.LeaderboardUtils.ShowOverlay(leaderboardID, string.Empty);
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			CloudOnceUtils.LeaderboardUtils.LoadScores(leaderboardID, callback);
		}
	}
}
