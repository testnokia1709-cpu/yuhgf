using System;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public interface ILeaderboardUtils
	{
		void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

		void ShowOverlay(string id = "", string internalID = "");

		void LoadScores(string leaderboardID, Action<IScore[]> callback);
	}
}
