using System;
using CloudOnce.Internal.Providers;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class GoogleAchievementUtils : IAchievementUtils
	{
		private const string c_unlockAction = "unlock";

		private const string c_revealAction = "reveal";

		private const string c_incrementAction = "increment";

		public void Unlock(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError("Can't unlock achievement. Supplied ID is null or empty!", onComplete);
				return;
			}
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized)
			{
				string errorMessage = ((!string.IsNullOrEmpty(internalID)) ? string.Format("Can't unlock {0} ({1}). Unlock can only be called after authentication.", internalID, id) : string.Format("Can't unlock {0}. UnlockAchievement can only be called after authentication.", id));
				ReportError(errorMessage, onComplete);
				return;
			}
			Action<bool> callback = delegate(bool response)
			{
				OnReportCompleted(response, onComplete, "unlock", id, internalID);
			};
			PlayGamesPlatform.Instance.ReportProgress(id, 100.0, callback);
		}

		public void Reveal(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError("Can't reveal achievement. Supplied ID is null or empty!", onComplete);
				return;
			}
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized)
			{
				string errorMessage = ((!string.IsNullOrEmpty(internalID)) ? string.Format("Can't reveal {0} ({1}). Reveal can only be called after authentication.", internalID, id) : string.Format("Can't reveal {0}. RevealAchievement can only be called after authentication.", id));
				ReportError(errorMessage, onComplete);
				return;
			}
			Action<bool> callback = delegate(bool response)
			{
				OnReportCompleted(response, onComplete, "reveal", id, internalID);
			};
			PlayGamesPlatform.Instance.ReportProgress(id, 0.0, callback);
		}

		public void Increment(string id, double progress, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError("Can't increment achievement. Supplied ID is null or empty!", onComplete);
				return;
			}
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized)
			{
				string errorMessage = ((!string.IsNullOrEmpty(internalID)) ? string.Format("Can't increment {0} ({1}). Increment can only be called after authentication.", internalID, id) : string.Format("Can't increment {0}. IncrementAchievement can only be called after authentication.", id));
				ReportError(errorMessage, onComplete);
				return;
			}
			Action<bool> callback = delegate(bool response)
			{
				OnReportCompleted(response, onComplete, "increment", id, internalID);
			};
			PlayGamesPlatform.Instance.ReportProgress(id, progress, callback);
		}

		public void ShowOverlay()
		{
			if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized)
			{
				PlayGamesPlatform.Instance.ShowAchievementsUI(OnShowOverlayCompleted);
			}
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized)
			{
				PlayGamesPlatform.Instance.LoadAchievementDescriptions(callback);
			}
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized)
			{
				PlayGamesPlatform.Instance.LoadAchievements(callback);
			}
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

		private static void OnReportCompleted(bool response, Action<CloudRequestResult<bool>> callbackAction, string action, string id, string internalID)
		{
			if (response)
			{
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
				return;
			}
			string errorMessage = ((!string.IsNullOrEmpty(internalID)) ? string.Format("Native API failed to {0} achievement {1} ({2}). Cause unknown.", action, internalID, id) : string.Format("Native API failed to {0} achievement {1}. Cause unknown.", action, id));
			ReportError(errorMessage, callbackAction);
		}
	}
}
