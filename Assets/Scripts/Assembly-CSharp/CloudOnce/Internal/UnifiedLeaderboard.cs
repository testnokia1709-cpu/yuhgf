using System;
using CloudOnce.Internal.Utils;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal
{
	public class UnifiedLeaderboard
	{
		private readonly string internalID;

		public string ID { get; private set; }

		public UnifiedLeaderboard(string internalID, string platformID)
		{
			this.internalID = internalID;
			ID = platformID;
		}

		public void SubmitScore(long score, Action<CloudRequestResult<bool>> onComplete = null)
		{
			CloudOnceUtils.LeaderboardUtils.SubmitScore(ID, score, onComplete, internalID);
		}

		public void ShowOverlay()
		{
			CloudOnceUtils.LeaderboardUtils.ShowOverlay(ID, internalID);
		}

		public void LoadScores(Action<IScore[]> callback)
		{
			CloudOnceUtils.LeaderboardUtils.LoadScores(ID, callback);
		}
	}
}
