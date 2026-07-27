using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

[Serializable]
[XmlRoot("DataStore")]
public class DataStore
{
	public static DataStore Instance;

	public GameSettings GameSettings;

	public MarketingSettings MarketingSettings;

	public DeviceSettings DeviceSettings;

	public ConfigSettings ConfigSettings;

	public FacebookSettings FacebookSettings;

	public ParseSettings ParseSettings;

	public LastPlayed LastPlayed;

	public int ShapeCount;

	public int GameScorePosted;

	public bool CloudAutoSignIn;

	public bool CloudSaveEnabled;

	public long LastSubmittedLevelTicks;

	public int CoinCount;

	public int GemCount;

	public SerializableDictionaryStringInt LevelsSolved = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringBool LevelsLocked = new SerializableDictionaryStringBool();

	public SerializableDictionaryStringInt LevelImageVersion = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringBool Purchases = new SerializableDictionaryStringBool();

	public SerializableDictionaryStringBool FreeItems = new SerializableDictionaryStringBool();

	public SerializableDictionaryStringInt FriendsScore = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt LevelsMinShapeCount = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringFloat LevelsMinTime = new SerializableDictionaryStringFloat();

	public SerializableDictionaryStringBool AchievementEarned = new SerializableDictionaryStringBool();

	public SerializableDictionaryStringInt LeaderboardScore = new SerializableDictionaryStringInt();

	[SerializeField]
	private List<StringInt> m_levelsSolved;

	[SerializeField]
	private List<StringBool> m_levelsLocked;

	[SerializeField]
	private List<StringInt> m_levelImageVersion;

	[SerializeField]
	private List<StringBool> m_purchases;

	[SerializeField]
	private List<StringBool> m_freeItems;

	[SerializeField]
	private List<StringInt> m_friendsScore;

	[SerializeField]
	private List<StringInt> m_levelsMinShapeCount;

	[SerializeField]
	private List<StringFloat> m_levelsMinTime;

	[SerializeField]
	private List<StringBool> m_achievementEarned;

	[SerializeField]
	private List<StringInt> m_leaderboardScore;

	public DataStore()
	{
		ConfigSettings = new ConfigSettings();
		setDefaultSettings();
	}

	public static void Create()
	{
		if (Instance == null)
		{
			Instance = SaveData.Load();
			if (Instance == null)
			{
				Debug.Log("No save game found, creating new data store");
				Instance = new DataStore();
			}
		}
		Instance.setDeviceSettings();
	}

	public static void Reset()
	{
		if (Instance != null)
		{
			Instance.LevelsSolved = new SerializableDictionaryStringInt();
			Instance.LevelImageVersion = new SerializableDictionaryStringInt();
			Instance.LevelsMinShapeCount = new SerializableDictionaryStringInt();
			Instance.LevelsMinTime = new SerializableDictionaryStringFloat();
			Instance.AchievementEarned = new SerializableDictionaryStringBool();
			Instance.LeaderboardScore = new SerializableDictionaryStringInt();
			AchievementManager.Instance.Clear();
			Instance.LastPlayed.Level = 1;
			Instance.ShapeCount = 0;
			Instance.CoinCount = 0;
			Instance.GemCount = 0;
			LevelManager.Level = 1;
			LevelManager.ThumbnailCache.Clear();
			DataFile.ClearAllImages();
			ParseAPI.Instance.Reset();
			Instance.MarketingSettings.LevelsCompleted = 0;
		}
	}

	public void BeforeSave()
	{
		m_levelsSolved = new List<StringInt>();
		foreach (KeyValuePair<string, int> item in LevelsSolved)
		{
			m_levelsSolved.Add(new StringInt
			{
				Key = item.Key,
				Value = item.Value
			});
		}
		m_levelsLocked = new List<StringBool>();
		foreach (KeyValuePair<string, bool> item2 in LevelsLocked)
		{
			m_levelsLocked.Add(new StringBool
			{
				Key = item2.Key,
				Value = item2.Value
			});
		}
		m_levelImageVersion = new List<StringInt>();
		foreach (KeyValuePair<string, int> item3 in LevelImageVersion)
		{
			m_levelImageVersion.Add(new StringInt
			{
				Key = item3.Key,
				Value = item3.Value
			});
		}
		m_purchases = new List<StringBool>();
		foreach (KeyValuePair<string, bool> purchase in Purchases)
		{
			m_purchases.Add(new StringBool
			{
				Key = purchase.Key,
				Value = purchase.Value
			});
		}
		m_freeItems = new List<StringBool>();
		foreach (KeyValuePair<string, bool> freeItem in FreeItems)
		{
			m_freeItems.Add(new StringBool
			{
				Key = freeItem.Key,
				Value = freeItem.Value
			});
		}
		m_friendsScore = new List<StringInt>();
		foreach (KeyValuePair<string, int> item4 in FriendsScore)
		{
			m_friendsScore.Add(new StringInt
			{
				Key = item4.Key,
				Value = item4.Value
			});
		}
		m_levelsMinShapeCount = new List<StringInt>();
		foreach (KeyValuePair<string, int> item5 in LevelsMinShapeCount)
		{
			m_levelsMinShapeCount.Add(new StringInt
			{
				Key = item5.Key,
				Value = item5.Value
			});
		}
		m_levelsMinTime = new List<StringFloat>();
		foreach (KeyValuePair<string, float> item6 in LevelsMinTime)
		{
			m_levelsMinTime.Add(new StringFloat
			{
				Key = item6.Key,
				Value = item6.Value
			});
		}
		m_achievementEarned = new List<StringBool>();
		foreach (KeyValuePair<string, bool> item7 in AchievementEarned)
		{
			m_achievementEarned.Add(new StringBool
			{
				Key = item7.Key,
				Value = item7.Value
			});
		}
		m_leaderboardScore = new List<StringInt>();
		foreach (KeyValuePair<string, int> item8 in LeaderboardScore)
		{
			m_leaderboardScore.Add(new StringInt
			{
				Key = item8.Key,
				Value = item8.Value
			});
		}
	}

	public void AfterLoad()
	{
		foreach (StringInt item in m_levelsSolved)
		{
			if (!LevelsSolved.ContainsKey(item.Key))
			{
				LevelsSolved.Add(item.Key, item.Value);
			}
		}
		foreach (StringBool item2 in m_levelsLocked)
		{
			if (!LevelsLocked.ContainsKey(item2.Key))
			{
				LevelsLocked.Add(item2.Key, item2.Value);
			}
		}
		foreach (StringInt item3 in m_levelImageVersion)
		{
			if (!LevelImageVersion.ContainsKey(item3.Key))
			{
				LevelImageVersion.Add(item3.Key, item3.Value);
			}
		}
		foreach (StringBool purchase in m_purchases)
		{
			if (!Purchases.ContainsKey(purchase.Key))
			{
				Purchases.Add(purchase.Key, purchase.Value);
			}
		}
		foreach (StringBool freeItem in m_freeItems)
		{
			if (!FreeItems.ContainsKey(freeItem.Key))
			{
				FreeItems.Add(freeItem.Key, freeItem.Value);
			}
		}
		foreach (StringInt item4 in m_friendsScore)
		{
			if (!FriendsScore.ContainsKey(item4.Key))
			{
				FriendsScore.Add(item4.Key, item4.Value);
			}
		}
		foreach (StringInt item5 in m_levelsMinShapeCount)
		{
			if (!LevelsMinShapeCount.ContainsKey(item5.Key))
			{
				LevelsMinShapeCount.Add(item5.Key, item5.Value);
			}
		}
		foreach (StringFloat item6 in m_levelsMinTime)
		{
			if (!LevelsMinTime.ContainsKey(item6.Key))
			{
				LevelsMinTime.Add(item6.Key, item6.Value);
			}
		}
		foreach (StringBool item7 in m_achievementEarned)
		{
			if (!AchievementEarned.ContainsKey(item7.Key))
			{
				AchievementEarned.Add(item7.Key, item7.Value);
			}
		}
		foreach (StringInt item8 in m_leaderboardScore)
		{
			if (!LeaderboardScore.ContainsKey(item8.Key))
			{
				LeaderboardScore.Add(item8.Key, item8.Value);
			}
		}
	}

	public static bool Purge()
	{
		bool flag = false;
		HashSet<string> allLevelKeysAsSet = LevelManager.GetAllLevelKeysAsSet();
		flag |= purgeKeys(Instance.LevelImageVersion, allLevelKeysAsSet);
		flag |= purgeKeys(Instance.LevelsSolved, allLevelKeysAsSet);
		flag |= purgeKeys(Instance.LevelsLocked, allLevelKeysAsSet);
		flag |= purgeKeys(Instance.LevelsMinShapeCount, allLevelKeysAsSet);
		flag |= purgeKeys(Instance.LevelsMinTime, allLevelKeysAsSet);
		Debug.Log("Purged = " + flag);
		return flag;
	}

	private static bool purgeKeys<T, X>(Dictionary<T, X> dictionary, HashSet<T> keySet)
	{
		bool result = false;
		List<T> list = new List<T>();
		foreach (T key in dictionary.Keys)
		{
			if (!keySet.Contains(key))
			{
				list.Add(key);
			}
		}
		if (list.Count > 0)
		{
			result = true;
			foreach (T item in list)
			{
				dictionary.Remove(item);
			}
			list.Clear();
		}
		return result;
	}

	public static void Save()
	{
		if (Instance != null && !SaveData.Save(Instance))
		{
			DialogManager.ShowDialog("Failed to save game data. Please contact support@brainitongame.com for help.", "Ok");
		}
	}

	private void setDefaultSettings()
	{
		GameSettings.MusicOn = true;
		GameSettings.SoundOn = true;
		GameSettings.Language = string.Empty;
		FacebookSettings.FirstName = string.Empty;
		FacebookSettings.UserId = string.Empty;
		CloudAutoSignIn = true;
		CloudSaveEnabled = true;
		LastSubmittedLevelTicks = (NTPTime.GetTime() - TimeSpan.FromDays(1.0)).Ticks;
	}

	private void setDeviceSettings()
	{
		DeviceSettings.VideoSupported = true;
	}

	public void SetFriendsProfileImage(string userId, Texture2D texture)
	{
		DataFile.SaveImage(FacebookSettings.s_imagePrefix + userId, texture);
	}

	public Texture2D GetFriendsProfileImage(string userId)
	{
		Texture2D texture2D = null;
		if (DataFile.ExistsImage(FacebookSettings.s_imagePrefix + userId))
		{
			texture2D = DataFile.LoadImage(FacebookSettings.s_imagePrefix + userId);
			if (texture2D == null)
			{
				texture2D = new Texture2D(128, 128);
			}
		}
		return texture2D;
	}

	public bool ExistsFriendsProfileImage(string userId)
	{
		return DataFile.ExistsImage(FacebookSettings.s_imagePrefix + userId);
	}

	public void SetFriendsScore(string userId, int score)
	{
		if (!FriendsScore.ContainsKey(userId))
		{
			FriendsScore.Add(userId, score);
		}
		else
		{
			FriendsScore[userId] = score;
		}
	}

	public int GetFriendsScore(string userId)
	{
		int result = 0;
		if (FriendsScore.ContainsKey(userId))
		{
			result = FriendsScore[userId];
		}
		return result;
	}
}
