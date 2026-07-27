using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Utils
{
	public class EditorAchievementUtils : IAchievementUtils
	{
		private class TestAchievement : IAchievement
		{
			public string id { get; set; }

			public double percentCompleted { get; set; }

			public bool completed { get; private set; }

			public bool hidden { get; private set; }

			public DateTime lastReportedDate { get; private set; }

			public TestAchievement(PropertyInfo property)
			{
				UnifiedAchievement unifiedAchievement = (UnifiedAchievement)property.GetValue(null, null);
				id = unifiedAchievement.ID;
				percentCompleted = unifiedAchievement.Progress;
				completed = unifiedAchievement.IsUnlocked;
				hidden = false;
				lastReportedDate = DateTime.Now;
			}

			public void ReportProgress(Action<bool> callback)
			{
				CloudOnceUtils.SafeInvoke(callback, true);
			}
		}

		private class TestAchievementDescription : IAchievementDescription
		{
			public string id { get; set; }

			public string title { get; private set; }

			public Texture2D image { get; private set; }

			public string achievedDescription { get; private set; }

			public string unachievedDescription { get; private set; }

			public bool hidden { get; private set; }

			public int points { get; private set; }

			public TestAchievementDescription(PropertyInfo property)
			{
				UnifiedAchievement unifiedAchievement = (UnifiedAchievement)property.GetValue(null, null);
				id = unifiedAchievement.ID;
				title = property.Name;
				image = Texture2D.whiteTexture;
				achievedDescription = "Test description for " + property.Name + ".";
				unachievedDescription = achievedDescription;
				hidden = false;
				points = 0;
			}
		}

		private const string c_unlockAction = "unlock";

		private const string c_revealAction = "reveal";

		private const string c_incrementAction = "increment";

		public void Unlock(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError("Can't unlock achievement. Supplied ID is null or empty!", onComplete);
			}
			else
			{
				OnReportCompleted(true, onComplete, "unlock", id, internalID);
			}
		}

		public void Reveal(string id, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError("Can't reveal achievement. Supplied ID is null or empty!", onComplete);
			}
			else
			{
				OnReportCompleted(true, onComplete, "reveal", id, internalID);
			}
		}

		public void Increment(string id, double progress, Action<CloudRequestResult<bool>> onComplete, string internalID = "")
		{
			if (string.IsNullOrEmpty(id))
			{
				ReportError("Can't increment achievement. Supplied ID is null or empty!", onComplete);
			}
			else
			{
				OnReportCompleted(true, onComplete, "increment", id, internalID);
			}
		}

		public void ShowOverlay()
		{
			Debug.LogWarning("Achievements overlay is not supported in the Unity Editor.");
		}

		public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback)
		{
			CloudOnceUtils.SafeInvoke(callback, GetTestAchievementDescriptions());
		}

		public void LoadAchievements(Action<IAchievement[]> callback)
		{
			CloudOnceUtils.SafeInvoke(callback, GetTestAchievements());
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

		private static IAchievementDescription[] GetTestAchievementDescriptions()
		{
			return (from property in typeof(Achievements).GetProperties()
				where property.PropertyType == typeof(UnifiedAchievement)
				select property).Select((Func<PropertyInfo, IAchievementDescription>)((PropertyInfo property) => new TestAchievementDescription(property))).ToArray();
		}

		private static IAchievement[] GetTestAchievements()
		{
			return (from property in typeof(Achievements).GetProperties()
				where property.PropertyType == typeof(UnifiedAchievement)
				select property).Select((Func<PropertyInfo, IAchievement>)((PropertyInfo property) => new TestAchievement(property))).ToArray();
		}
	}
}
