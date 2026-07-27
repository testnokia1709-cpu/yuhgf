using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class EditorLeaderboardUtils : ILeaderboardUtils
	{
		public void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError(string.Format("Can't submit score to {0} leaderboard. Platform ID is null or empty!", internalID), onComplete);
			}
			else
			{
				ReportSubmitScoreSuccess(score, onComplete, id, internalID);
			}
		}

		public void ShowOverlay(string id = "", string internalID = "")
		{
			Debug.LogWarning("Leaderboards overlay is not supported in the Unity Editor.");
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			Debug.LogWarning("Leaderboards overlay is not supported in the Unity Editor.");
			CloudOnceUtils.SafeInvoke(callback, new IScore[0]);
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}

		private static void ReportSubmitScoreSuccess(long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
		{
			CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
		}
	}
}
