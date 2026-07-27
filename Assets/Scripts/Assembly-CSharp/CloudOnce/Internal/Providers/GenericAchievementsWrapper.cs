using System;
using CloudOnce.Internal.Utils;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Providers
{
	public class GenericAchievementsWrapper
	{
		public void UnlockAchievement(string achievementId, Action<CloudRequestResult<bool>> onComplete = null)
		{
			Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
			{
				OnUpdateAchievementCompleted(response, onComplete);
			};
			CloudOnceUtils.AchievementUtils.Unlock(achievementId, onComplete2, string.Empty);
		}

		public void RevealAchievement(string achievementId, Action<CloudRequestResult<bool>> onComplete = null)
		{
			Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
			{
				OnUpdateAchievementCompleted(response, onComplete);
			};
			CloudOnceUtils.AchievementUtils.Reveal(achievementId, onComplete2, string.Empty);
		}

		public void IncrementAchievement(string achievementId, double current, double goal, Action<CloudRequestResult<bool>> onComplete = null)
		{
			IncrementAchievement(achievementId, current / goal * 100.0, onComplete);
		}

		public void IncrementAchievement(string achievementId, double progress, Action<CloudRequestResult<bool>> onComplete)
		{
			if (progress < 0.0)
			{
				throw new ArgumentException("Value must not be negative!", "progress");
			}
			if (progress.Equals(0.0))
			{
				RevealAchievement(achievementId, onComplete);
				return;
			}
			if (progress >= 100.0)
			{
				UnlockAchievement(achievementId, onComplete);
				return;
			}
			Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
			{
				OnUpdateAchievementCompleted(response, onComplete);
			};
			CloudOnceUtils.AchievementUtils.Increment(achievementId, progress, onComplete2, string.Empty);
		}

		public void ShowOverlay()
		{
			CloudOnceUtils.AchievementUtils.ShowOverlay();
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			CloudOnceUtils.AchievementUtils.LoadAchievementDescriptions(callback);
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			CloudOnceUtils.AchievementUtils.LoadAchievements(callback);
		}

		private void OnUpdateAchievementCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudRequestResult<bool> param = ((!response.Result) ? new CloudRequestResult<bool>(false, response.Error) : new CloudRequestResult<bool>(true));
			CloudOnceUtils.SafeInvoke(callbackAction, param);
		}
	}
}
