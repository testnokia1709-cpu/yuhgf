using System;
using System.Collections.Generic;
using System.Linq;
using CloudOnce;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
	public static AchievementManager Instance;

	public static Dictionary<string, Achievement> s_achievementList = new Dictionary<string, Achievement>
	{
		{
			CloudIDs.AchievementIDs.Solve40levelswith3stars,
			new Achievement
			{
				Target = 40,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasThreeStarCount(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_THREESTARS,
				RawAchievement = Achievements.Solve40levelswith3stars
			}
		},
		{
			CloudIDs.AchievementIDs.Solve80levelswith3stars,
			new Achievement
			{
				Target = 80,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasThreeStarCount(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_THREESTARS,
				RawAchievement = Achievements.Solve80levelswith3stars
			}
		},
		{
			CloudIDs.AchievementIDs.Solve120levelswith3stars,
			new Achievement
			{
				Target = 120,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasThreeStarCount(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_THREESTARS,
				RawAchievement = Achievements.Solve120levelswith3stars
			}
		},
		{
			CloudIDs.AchievementIDs.Solve160levelswith3stars,
			new Achievement
			{
				Target = 160,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasThreeStarCount(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_THREESTARS,
				RawAchievement = Achievements.Solve160levelswith3stars
			}
		},
		{
			CloudIDs.AchievementIDs.Solve200levelswith3stars,
			new Achievement
			{
				Target = 200,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasThreeStarCount(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_THREESTARS,
				RawAchievement = Achievements.Solve200levelswith3stars
			}
		},
		{
			CloudIDs.AchievementIDs.Solve5levelsbelowtheshapegoal,
			new Achievement
			{
				Target = 5,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowShapeGoal(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
				RawAchievement = Achievements.Solve5levelsbelowtheshapegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Solve10levelsbelowtheshapegoal,
			new Achievement
			{
				Target = 10,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowShapeGoal(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
				RawAchievement = Achievements.Solve10levelsbelowtheshapegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Solve15levelsbelowtheshapegoal,
			new Achievement
			{
				Target = 15,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowShapeGoal(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
				RawAchievement = Achievements.Solve15levelsbelowtheshapegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Solve20levelsbelowtheshapegoal,
			new Achievement
			{
				Target = 20,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowShapeGoal(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
				RawAchievement = Achievements.Solve20levelsbelowtheshapegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Solve25levelsbelowtheshapegoal,
			new Achievement
			{
				Target = 25,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowShapeGoal(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
				RawAchievement = Achievements.Solve25levelsbelowtheshapegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Solve30levelsbelowtheshapegoal,
			new Achievement
			{
				Target = 30,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowShapeGoal(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
				RawAchievement = Achievements.Solve30levelsbelowtheshapegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Draw5000shapes,
			new Achievement
			{
				Target = 5000,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasEnoughShapes(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_DRAWSHAPES,
				RawAchievement = Achievements.Draw5000shapes
			}
		},
		{
			CloudIDs.AchievementIDs.Draw10000shapes,
			new Achievement
			{
				Target = 10000,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasEnoughShapes(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_DRAWSHAPES,
				RawAchievement = Achievements.Draw10000shapes
			}
		},
		{
			CloudIDs.AchievementIDs.Draw25000shapes,
			new Achievement
			{
				Target = 25000,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasEnoughShapes(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_DRAWSHAPES,
				RawAchievement = Achievements.Draw25000shapes
			}
		},
		{
			CloudIDs.AchievementIDs.Solve60levelswithonly1shape,
			new Achievement
			{
				Target = 60,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasOneShapeCount(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_ONESHAPE,
				RawAchievement = Achievements.Solve60levelswithonly1shape
			}
		},
		{
			CloudIDs.AchievementIDs.Solve3levelsinlessthan1second,
			new Achievement
			{
				Target = 3,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasLessThanOneSecond(target),
				LocalizedDescription = StringId.S_ACHIEVEMENT_ONESECOND,
				RawAchievement = Achievements.Solve3levelsinlessthan1second
			}
		},
		{
			CloudIDs.AchievementIDs.Solve40levels5secondsbelowthetimegoal,
			new Achievement
			{
				Target = 40,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowTimeGoal(target, 5f),
				LocalizedDescription = StringId.S_ACHIEVEMENT_FIVEBELOWTIME,
				RawAchievement = Achievements.Solve40levels5secondsbelowthetimegoal
			}
		},
		{
			CloudIDs.AchievementIDs.Solve10levels10secondsbelowthetimegoal,
			new Achievement
			{
				Target = 10,
				Earned = false,
				Progress = 0f,
				Calculate = (int target) => hasBelowTimeGoal(target, 10f),
				LocalizedDescription = StringId.S_ACHIEVEMENT_TENBELOWTIME,
				RawAchievement = Achievements.Solve10levels10secondsbelowthetimegoal
			}
		}
	};

	public Text CountText;

	public Text DescriptionText;

	public Text TextTotalTime;

	public Text TextTotalShape;

	public Color AchievementActiveColor;

	public Color AchievementInactiveColor;

	public AchievementButton SelectedButton;

	public List<AchievementButton> ButtonList;

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		DescriptionText.text = string.Empty;
		SelectedButton = null;
	}

	public void Clear()
	{
		foreach (KeyValuePair<string, Achievement> s_achievement in s_achievementList)
		{
			s_achievement.Value.Progress = 0f;
		}
	}

	private void Update()
	{
	}

	public void AchievementButtonDown(AchievementButton button)
	{
		if (SelectedButton != null)
		{
			SelectedButton.Border.gameObject.SetActive(false);
			SelectedButton = null;
		}
		if (ButtonList.Contains(button))
		{
			int num = ButtonList.IndexOf(button);
			if (num < s_achievementList.Count)
			{
				Achievement achievement = s_achievementList.Values.ElementAt(num);
				int num2 = Mathf.FloorToInt(achievement.Progress * 100f);
				DescriptionText.text = string.Format(TextLibrary.Get(achievement.LocalizedDescription), achievement.Target) + string.Format(" ({0}%)", num2);
				SelectedButton = button;
				SelectedButton.Border.gameObject.SetActive(true);
			}
		}
	}

	public void UpdateControls()
	{
		bool flag = LevelManager.Instance.GetTotalLevelsSolved(1, 200) == 200;
		TimeSpan timeSpan = ((!flag) ? default(TimeSpan) : LevelManager.Instance.GetTotalTime(1, 200));
		int num = (flag ? LevelManager.Instance.GetTotalShapes(1, 200) : 0);
		string arg = string.Format("{0:mm:ss}", timeSpan);
		TextTotalTime.text = string.Format(TextLibrary.Get(StringId.S_TOTALTIME), arg);
		TextTotalShape.text = string.Format(TextLibrary.Get(StringId.S_TOTALSHAPE), num);
		TextTotalTime.color = ((!flag) ? AchievementInactiveColor : AchievementActiveColor);
		TextTotalShape.color = ((!flag) ? AchievementInactiveColor : AchievementActiveColor);
		foreach (AchievementButton button in ButtonList)
		{
			button.Border.gameObject.SetActive(false);
		}
		SelectedButton = null;
		DescriptionText.text = string.Empty;
	}

	public int CalculateAchievements(bool incremental = false)
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < s_achievementList.Count; i++)
		{
			Achievement achievement = s_achievementList.Values.ElementAt(i);
			string text = s_achievementList.Keys.ElementAt(i);
			achievement.Progress = (float)achievement.RawAchievement.Progress / 100f;
			float num2 = achievement.Calculate(achievement.Target);
			if (num2 > achievement.Progress)
			{
				achievement.Progress = num2;
				CloudOnceAPI.Instance.AchievementIncrement(text, achievement.Progress * 100f);
			}
			if (achievement.Progress >= 1f)
			{
				num++;
				achievement.Earned = true;
				if (incremental && !string.IsNullOrEmpty(text) && !DataStore.Instance.AchievementEarned.ContainsKey(text))
				{
					DataStore.Instance.AchievementEarned.Add(text, true);
					flag = true;
					CloudOnceAPI.Instance.AchievementUnlock(text);
					break;
				}
			}
			else
			{
				achievement.Earned = false;
			}
			if (!incremental)
			{
				ButtonList[i].SetEarned(achievement.Earned);
				ButtonList[i].SetProgress(achievement.Progress);
			}
		}
		if (!incremental)
		{
			CountText.text = num.ToString();
		}
		if (flag | calculateLeaderboard())
		{
			DataStore.Save();
		}
		return num;
	}

	private bool calculateLeaderboard()
	{
		if (!CloudOnceAPI.Instance.IsSignedIn())
		{
			return false;
		}
		bool result = false;
		bool flag = LevelManager.Instance.GetTotalLevelsSolved(1, 200) == 200;
		TimeSpan totalTime = LevelManager.Instance.GetTotalTime(1, 200);
		int totalShapes = LevelManager.Instance.GetTotalShapes(1, 200);
		if (flag)
		{
			int num = (int)totalTime.TotalSeconds * 1000;
			string fastestTime = CloudIDs.LeaderboardIDs.FastestTime;
			CloudOnceAPI.Instance.LeaderboardSet(fastestTime, num);
			if (!DataStore.Instance.LeaderboardScore.ContainsKey(fastestTime))
			{
				DataStore.Instance.LeaderboardScore.Add(fastestTime, num);
				result = true;
			}
			else
			{
				int num2 = DataStore.Instance.LeaderboardScore[fastestTime];
				if (num < num2)
				{
					DataStore.Instance.LeaderboardScore[fastestTime] = num;
					result = true;
				}
			}
			num = totalShapes;
			fastestTime = CloudIDs.LeaderboardIDs.FewestShapes;
			CloudOnceAPI.Instance.LeaderboardSet(fastestTime, num);
			if (!DataStore.Instance.LeaderboardScore.ContainsKey(fastestTime))
			{
				DataStore.Instance.LeaderboardScore.Add(fastestTime, num);
				result = true;
			}
			else
			{
				int num3 = DataStore.Instance.LeaderboardScore[fastestTime];
				if (num < num3)
				{
					DataStore.Instance.LeaderboardScore[fastestTime] = num;
					result = true;
				}
			}
		}
		return result;
	}

	private static float hasThreeStarCount(int target)
	{
		float num = 0f;
		int threeStarCount = LevelManager.Instance.GetThreeStarCount();
		if (threeStarCount >= target)
		{
			return 1f;
		}
		return (float)threeStarCount / (float)target;
	}

	private static float hasBelowShapeGoal(int target)
	{
		float num = 0f;
		int belowShapeGoalCount = LevelManager.Instance.GetBelowShapeGoalCount();
		if (belowShapeGoalCount >= target)
		{
			return 1f;
		}
		return (float)belowShapeGoalCount / (float)target;
	}

	private static float hasEnoughShapes(int target)
	{
		float num = 0f;
		int shapeCount = DataStore.Instance.ShapeCount;
		if (shapeCount >= target)
		{
			return 1f;
		}
		return (float)shapeCount / (float)target;
	}

	private static float hasOneShapeCount(int target)
	{
		float num = 0f;
		int oneShapeCount = LevelManager.Instance.GetOneShapeCount();
		if (oneShapeCount >= target)
		{
			return 1f;
		}
		return (float)oneShapeCount / (float)target;
	}

	private static float hasLessThanOneSecond(int target)
	{
		float num = 0f;
		int lessThanTime = LevelManager.Instance.GetLessThanTime(1f);
		if (lessThanTime >= target)
		{
			return 1f;
		}
		return (float)lessThanTime / (float)target;
	}

	private static float hasBelowTimeGoal(int target, float timeDelta)
	{
		float num = 0f;
		int belowTimeGoalByAmount = LevelManager.Instance.GetBelowTimeGoalByAmount(timeDelta);
		if (belowTimeGoalByAmount >= target)
		{
			return 1f;
		}
		return (float)belowTimeGoalByAmount / (float)target;
	}
}
