using System.Collections.Generic;
using UnityEngine;

public class CommunityManager : MonoBehaviour
{
	public static CommunityManager Instance;

	public static TouchDrawLevel CurrentLevel;

	public static int CurrentLevelIndex;

	public static string CurrentTitle;

	public static Vector2 ThumbnailSize;

	public static List<CommunityLevel> Levels
	{
		get
		{
			return ParseAPI.Instance.Levels;
		}
	}

	public static string CurrentLevelId
	{
		get
		{
			return Levels[CurrentLevelIndex].ObjectId;
		}
	}

	public static CommunityLevel CurrentLevelStats
	{
		get
		{
			return Levels[CurrentLevelIndex];
		}
	}

	public static void LoadCurrentLevel(int index)
	{
		CurrentLevel = LoadLevel(index);
		if (CurrentLevel != null)
		{
			CurrentTitle = Levels[index].Title;
			CurrentLevelIndex = index;
		}
	}

	public static bool LoadNextLevel()
	{
		int num = CurrentLevelIndex + 1;
		bool flag = num > -1 && num < Levels.Count;
		if (flag)
		{
			TouchDrawLevel.ClearObjects(CurrentLevel);
			CurrentLevel = LoadLevel(num);
			if (CurrentLevel != null)
			{
				CurrentTitle = Levels[num].Title;
				CurrentLevelIndex = num;
			}
		}
		return flag;
	}

	public static LevelCompletion GetLevelCompletion(string levelKey)
	{
		LevelCompletion levelCompletion = LevelCompletion.Unsolved;
		if (ParseAPI.Instance.IsCommunityLevelAttempted(levelKey))
		{
			levelCompletion |= ParseAPI.Instance.GetCommunityLevelCompletion(levelKey);
		}
		return levelCompletion;
	}

	public static TouchDrawLevel LoadLevel(int index)
	{
		return TouchDrawLevel.DecodeLevel(Levels[index].Data);
	}

	public static TouchDrawLevel LoadLevel(string levelData)
	{
		return TouchDrawLevel.DecodeLevel(levelData);
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(this);
	}
}
