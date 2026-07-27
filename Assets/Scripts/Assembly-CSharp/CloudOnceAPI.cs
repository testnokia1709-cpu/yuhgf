using System;
using System.Collections.Generic;
using CloudOnce;
using CloudOnce.Internal.Utils;
using UnityEngine;
using UnityEngine.Events;

public class CloudOnceAPI : MonoBehaviour
{
	public static CloudOnceAPI Instance;

	public Action<bool> OnCloundSignInChanged;

	public Action OnCloudLoadComplete;

	public int MinimumSecondsBetweenSaves = 120;

	private List<Action> m_deferredActions = new List<Action>();

	private DateTime m_timeLastSaved;

	public string PlayerDisplayName
	{
		get
		{
			return Cloud.PlayerDisplayName;
		}
	}

	public string PlayerID
	{
		get
		{
			return "g_" + Cloud.PlayerID;
		}
	}

	public Texture2D PlayerImage
	{
		get
		{
			return Cloud.PlayerImage;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		Cloud.OnInitializeComplete += CloudOnceInitializeComplete;
		Cloud.OnCloudLoadComplete += Cloud_OnCloudLoadComplete;
		Cloud.OnSignedInChanged += Cloud_OnSignedInChanged;
		Cloud.OnSignInFailed += Cloud_OnSignInFailed;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			Cloud.Initialize();
		}
		else
		{
			Cloud.Initialize(DataStore.Instance.CloudSaveEnabled, DataStore.Instance.CloudAutoSignIn);
		}
	}

	private void Update()
	{
		if (m_deferredActions.Count > 0)
		{
			m_deferredActions[0]();
			m_deferredActions.RemoveAt(0);
		}
	}

	public void CloudOnceInitializeComplete()
	{
		Cloud.OnInitializeComplete -= CloudOnceInitializeComplete;
	}

	private void Cloud_OnCloudLoadComplete(bool success)
	{
		Debug.Log("Cloud load: " + success);
		if (!success)
		{
			return;
		}
		if (!string.IsNullOrEmpty(CloudVariables.LevelsLocked))
		{
			SerializableDictionaryStringBool serializableDictionaryStringBool = JsonUtility.FromJson<SerializableDictionaryStringBool>(CloudVariables.LevelsLocked.FromBase64StringToString());
			foreach (KeyValuePair<string, bool> item in serializableDictionaryStringBool)
			{
				if (!DataStore.Instance.LevelsLocked.ContainsKey(item.Key))
				{
					DataStore.Instance.LevelsLocked.Add(item.Key, item.Value);
				}
				else if (!item.Value)
				{
					DataStore.Instance.LevelsLocked[item.Key] = item.Value;
				}
			}
		}
		Debug.Log("Levels Solved: " + CloudVariables.LevelsSolved);
		if (!string.IsNullOrEmpty(CloudVariables.LevelsSolved))
		{
			Debug.Log("Json: " + CloudVariables.LevelsSolved.FromBase64StringToString());
			SerializableDictionaryStringInt serializableDictionaryStringInt = JsonUtility.FromJson<SerializableDictionaryStringInt>(CloudVariables.LevelsSolved.FromBase64StringToString());
			foreach (KeyValuePair<string, int> item2 in serializableDictionaryStringInt)
			{
				if (!DataStore.Instance.LevelsSolved.ContainsKey(item2.Key))
				{
					DataStore.Instance.LevelsSolved.Add(item2.Key, item2.Value);
				}
				else if (item2.Value > DataStore.Instance.LevelsSolved[item2.Key])
				{
					DataStore.Instance.LevelsSolved[item2.Key] = item2.Value;
				}
			}
		}
		if (!string.IsNullOrEmpty(CloudVariables.FreeItems))
		{
			SerializableDictionaryStringBool serializableDictionaryStringBool2 = JsonUtility.FromJson<SerializableDictionaryStringBool>(CloudVariables.FreeItems.FromBase64StringToString());
			foreach (KeyValuePair<string, bool> item3 in serializableDictionaryStringBool2)
			{
				if (!DataStore.Instance.FreeItems.ContainsKey(item3.Key))
				{
					DataStore.Instance.FreeItems.Add(item3.Key, item3.Value);
				}
				else if (item3.Value)
				{
					DataStore.Instance.FreeItems[item3.Key] = item3.Value;
				}
			}
		}
		if (!string.IsNullOrEmpty(CloudVariables.LevelsMinShapeCount))
		{
			SerializableDictionaryStringInt serializableDictionaryStringInt2 = JsonUtility.FromJson<SerializableDictionaryStringInt>(CloudVariables.LevelsMinShapeCount.FromBase64StringToString());
			foreach (KeyValuePair<string, int> item4 in serializableDictionaryStringInt2)
			{
				if (!DataStore.Instance.LevelsMinShapeCount.ContainsKey(item4.Key))
				{
					DataStore.Instance.LevelsMinShapeCount.Add(item4.Key, item4.Value);
				}
				else if (item4.Value < DataStore.Instance.LevelsMinShapeCount[item4.Key])
				{
					DataStore.Instance.LevelsMinShapeCount[item4.Key] = item4.Value;
				}
			}
		}
		if (!string.IsNullOrEmpty(CloudVariables.LevelsMinTime))
		{
			SerializableDictionaryStringFloat serializableDictionaryStringFloat = JsonUtility.FromJson<SerializableDictionaryStringFloat>(CloudVariables.LevelsMinTime.FromBase64StringToString());
			foreach (KeyValuePair<string, float> item5 in serializableDictionaryStringFloat)
			{
				if (!DataStore.Instance.LevelsMinTime.ContainsKey(item5.Key))
				{
					DataStore.Instance.LevelsMinTime.Add(item5.Key, item5.Value);
				}
				else if (item5.Value < DataStore.Instance.LevelsMinTime[item5.Key])
				{
					DataStore.Instance.LevelsMinTime[item5.Key] = item5.Value;
				}
			}
		}
		if (CloudVariables.ShapeCount > DataStore.Instance.ShapeCount)
		{
			DataStore.Instance.ShapeCount = CloudVariables.ShapeCount;
		}
		if (CloudVariables.CoinCount > DataStore.Instance.CoinCount)
		{
			DataStore.Instance.CoinCount = CloudVariables.CoinCount;
		}
		if (CloudVariables.GemCount > DataStore.Instance.GemCount)
		{
			DataStore.Instance.GemCount = CloudVariables.GemCount;
		}
		if (OnCloudLoadComplete != null)
		{
			OnCloudLoadComplete();
		}
	}

	public void SignOut()
	{
		Cloud.SignOut();
	}

	public void SignIn(UnityAction<bool> signedIn)
	{
		Cloud.SignIn(true, signedIn);
	}

	public bool IsSignedIn()
	{
		return Cloud.IsSignedIn;
	}

	private void Cloud_OnSignInFailed()
	{
		Debug.Log("Cloud_OnSignInFailed()");
		Debug.Log("Social.localUser.authenticated: " + Social.localUser.authenticated);
	}

	private void Cloud_OnSignedInChanged(bool signedin)
	{
		Debug.Log("Cloud_OnSignedInChanged: " + signedin);
		m_deferredActions.Add(delegate
		{
			if (DataStore.Instance.CloudAutoSignIn != signedin)
			{
				Debug.Log("Updating auto sign in: " + signedin);
				DataStore.Instance.CloudAutoSignIn = signedin;
				DataStore.Save();
			}
			if (OnCloundSignInChanged != null)
			{
				OnCloundSignInChanged(signedin);
			}
		});
	}

	public void CloudSave(bool force = false)
	{
		if (DateTime.Now - m_timeLastSaved > TimeSpan.FromSeconds(MinimumSecondsBetweenSaves) || force)
		{
			Debug.Log("CloudSave(): LevelsSolved: " + JsonUtility.ToJson(DataStore.Instance.LevelsSolved));
			CloudVariables.LevelsLocked = JsonUtility.ToJson(DataStore.Instance.LevelsLocked).ToBase64String();
			CloudVariables.LevelsSolved = JsonUtility.ToJson(DataStore.Instance.LevelsSolved).ToBase64String();
			CloudVariables.FreeItems = JsonUtility.ToJson(DataStore.Instance.FreeItems).ToBase64String();
			CloudVariables.LevelsMinShapeCount = JsonUtility.ToJson(DataStore.Instance.LevelsMinShapeCount).ToBase64String();
			CloudVariables.LevelsMinTime = JsonUtility.ToJson(DataStore.Instance.LevelsMinTime).ToBase64String();
			CloudVariables.ShapeCount = DataStore.Instance.ShapeCount;
			CloudVariables.CoinCount = DataStore.Instance.CoinCount;
			CloudVariables.GemCount = DataStore.Instance.GemCount;
			Cloud.Storage.Save();
			m_timeLastSaved = DateTime.Now;
		}
		else
		{
			Debug.Log("CloudSave(): Ignoring request, MinimumSecondsBetweenSaves threshold not reached yet.");
		}
	}

	public void AchievementUnlock(string achievementId)
	{
		Cloud.Achievements.UnlockAchievement(achievementId, delegate(CloudRequestResult<bool> result)
		{
			Debug.Log("Achievement.Unlock result = " + result.Result);
			if (!result.Result)
			{
				DataStore.Instance.AchievementEarned.Remove(achievementId);
				DataStore.Save();
			}
		});
	}

	public void AchievementIncrement(string achievementId, double progress)
	{
		Cloud.Achievements.IncrementAchievement(achievementId, progress, delegate(CloudRequestResult<bool> result)
		{
			Debug.Log("Achievement.Increment result = " + result.Result);
			if (result.Result)
			{
			}
		});
	}

	public void AchievementOverlay()
	{
		Cloud.Achievements.ShowOverlay();
	}

	public void LeaderboardSet(string leaderboardId, long score, Action<string, bool> OnComplete = null)
	{
		Cloud.Leaderboards.SubmitScore(leaderboardId, score, delegate(CloudRequestResult<bool> result)
		{
			Debug.Log("Leaderboard result = " + result.Result);
			if (OnComplete != null)
			{
				OnComplete(leaderboardId, result.Result);
			}
		});
	}

	public void LeaderboardOverlay()
	{
		Cloud.Leaderboards.ShowOverlay(string.Empty);
	}
}
