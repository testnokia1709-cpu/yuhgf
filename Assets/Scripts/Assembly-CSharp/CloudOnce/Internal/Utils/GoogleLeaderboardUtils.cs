using System;
using CloudOnce.Internal.Providers;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class GoogleLeaderboardUtils : ILeaderboardUtils
	{
		public void SubmitScore(string id, long score, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError(string.Format("Can't submit score to {0} leaderboard. Platform ID is null or empty!", internalID), onComplete);
				return;
			}
			if (!PlayGamesPlatform.Instance.IsAuthenticated())
			{
				ReportError(string.Format("Can't submit score to leaderboard {0} ({1}). SubmitScore can only be called after authentication.", internalID, id), onComplete);
				return;
			}
			Action<bool> callback = delegate(bool response)
			{
				OnSubmitScoreCompleted(response, score, onComplete, id, internalID);
			};
			PlayGamesPlatform.Instance.ReportScore(score, id, callback);
		}

		public void ShowOverlay(string id = "", string internalID = "")
		{
			if (PlayGamesPlatform.Instance.IsAuthenticated())
			{
				if (string.IsNullOrEmpty(id))
				{
					PlayGamesPlatform.Instance.ShowLeaderboardUI(null, OnShowOverlayCompleted);
				}
				else
				{
					PlayGamesPlatform.Instance.ShowLeaderboardUI(id, OnShowOverlayCompleted);
				}
			}
		}

		public void LoadScores(string leaderboardID, Action<IScore[]> callback)
		{
			PlayGamesPlatform.Instance.LoadScores(leaderboardID, callback);
		}

		private static void OnShowOverlayCompleted(UIStatus callback)
		{
			if (callback == UIStatus.NotAuthorized)
			{
				CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.ActivateGuestUserMode();
			}
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}

		private static void ReportSubmitScoreSuccess(long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
		{
			CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
		}

		private static void OnSubmitScoreCompleted(bool response, long score, Action<CloudRequestResult<bool>> callbackAction, string id, string internalID)
		{
			if (response)
			{
				ReportSubmitScoreSuccess(score, callbackAction, id, internalID);
				return;
			}
			string errorMessage = string.Format("Native API failed to submit a score of {0} to {1} ({2}) leaderboard. Cause unknown.", score, internalID, id);
			ReportError(errorMessage, callbackAction);
		}
	}
}
