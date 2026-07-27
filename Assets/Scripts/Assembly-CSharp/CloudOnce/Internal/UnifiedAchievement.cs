using System;
using CloudOnce.Internal.Utils;

namespace CloudOnce.Internal
{
	public class UnifiedAchievement
	{
		private readonly string internalID;

		private bool isAchievementHidden = true;

		private double achievementProgress;

		public string ID { get; private set; }

		public bool IsUnlocked { get; private set; }

		public double Progress
		{
			get
			{
				return achievementProgress;
			}
			private set
			{
				if (!(value < achievementProgress))
				{
					achievementProgress = ((!(value > 100.0)) ? value : 100.0);
				}
			}
		}

		public UnifiedAchievement(string internalID, string platformID)
		{
			this.internalID = internalID;
			ID = platformID;
		}

		public void Unlock(Action<CloudRequestResult<bool>> onComplete = null)
		{
			if (!IsUnlocked)
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					OnUnlockCompleted(response, onComplete);
				};
				CloudOnceUtils.AchievementUtils.Unlock(ID, onComplete2, internalID);
			}
			else
			{
				string errorMessage = string.Format("Can't unlock {0}. Achievement has already been unlocked.", ID);
				ReportError(errorMessage, onComplete);
			}
		}

		public void Reveal(Action<CloudRequestResult<bool>> onComplete = null)
		{
			if (isAchievementHidden)
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					OnRevealCompleted(response, onComplete);
				};
				CloudOnceUtils.AchievementUtils.Reveal(ID, onComplete2, internalID);
			}
			else
			{
				string errorMessage = string.Format("Can't reveal {0}. Achievement has already been revealed.", ID);
				ReportError(errorMessage, onComplete);
			}
		}

		public void Increment(double current, double goal, Action<CloudRequestResult<bool>> onComplete = null)
		{
			Increment(current / goal * 100.0, onComplete);
		}

		public void Increment(double progress, Action<CloudRequestResult<bool>> onComplete = null)
		{
			if (IsUnlocked)
			{
				string errorMessage = string.Format("Can't increment {0} ({1}). Achievement is already unlocked.", internalID, ID);
				ReportError(errorMessage, onComplete);
				return;
			}
			if (progress < 0.0)
			{
				throw new ArgumentException("Value must not be negative!", "progress");
			}
			if (progress.Equals(0.0))
			{
				Reveal(onComplete);
			}
			else if (progress >= 100.0)
			{
				Unlock(onComplete);
			}
			else if (progress <= Progress)
			{
				string errorMessage2 = string.Format("Can't increment {0} ({1}) to {2:F2}%. Achievement is already at {3:F2}%.", internalID, ID, progress, Progress);
				ReportError(errorMessage2, onComplete);
			}
			else
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					OnIncrementCompleted(response, progress, onComplete);
				};
				CloudOnceUtils.AchievementUtils.Increment(ID, progress, onComplete2, internalID);
			}
		}

		public void UpdateData(bool isUnlocked, double progress, bool isHidden)
		{
			if (IsUnlocked && !isUnlocked)
			{
				Action<CloudRequestResult<bool>> onComplete = delegate(CloudRequestResult<bool> response)
				{
					OnUnlockCompleted(response, null);
				};
				CloudOnceUtils.AchievementUtils.Unlock(ID, onComplete, internalID);
				return;
			}
			if (Progress > progress)
			{
				Action<CloudRequestResult<bool>> onComplete2 = delegate(CloudRequestResult<bool> response)
				{
					OnIncrementCompleted(response, progress, null);
				};
				CloudOnceUtils.AchievementUtils.Increment(ID, progress, onComplete2, internalID);
				return;
			}
			IsUnlocked = isUnlocked;
			Progress = progress;
			isAchievementHidden = isHidden;
			if (!IsUnlocked && Progress.Equals(100.0))
			{
				Action<CloudRequestResult<bool>> onComplete3 = delegate(CloudRequestResult<bool> response)
				{
					OnUnlockCompleted(response, null);
				};
				CloudOnceUtils.AchievementUtils.Unlock(ID, onComplete3, internalID);
			}
		}

		private static void ReportError(string errorMessage, Action<CloudRequestResult<bool>> callbackAction)
		{
			CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, errorMessage));
		}

		private void OnUnlockCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
		{
			if (response.Result)
			{
				IsUnlocked = true;
				isAchievementHidden = false;
				Progress = 100.0;
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, response.Error));
			}
		}

		private void OnRevealCompleted(CloudRequestResult<bool> response, Action<CloudRequestResult<bool>> callbackAction)
		{
			if (response.Result)
			{
				isAchievementHidden = false;
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, response.Error));
			}
		}

		private void OnIncrementCompleted(CloudRequestResult<bool> response, double progress, Action<CloudRequestResult<bool>> callbackAction)
		{
			if (response.Result)
			{
				Progress = progress;
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(true));
			}
			else
			{
				CloudOnceUtils.SafeInvoke(callbackAction, new CloudRequestResult<bool>(false, response.Error));
			}
		}
	}
}
