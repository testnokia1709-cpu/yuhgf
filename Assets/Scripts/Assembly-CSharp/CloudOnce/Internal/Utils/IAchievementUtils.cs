using System;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public interface IAchievementUtils
	{
		void Unlock(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

		void Reveal(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

		void Increment(string id, double progress, Action<CloudRequestResult<bool>> onComplete, string internalID = "");

		void ShowOverlay();

		void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback);

		void LoadAchievements(Action<IAchievement[]> callback);
	}
}
