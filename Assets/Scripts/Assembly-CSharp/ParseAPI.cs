using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Parse;
using UnityEngine;

public class ParseAPI : MonoBehaviour
{
	public static ParseAPI Instance;

	[NonSerialized]
	public List<CommunityLevel> Levels;

	[NonSerialized]
	public LevelRequestFilter CurrentRequestFilter = LevelRequestFilter.Featured;

	private static float s_updateInterval = 180f;

	private static float s_updateTime;

	private static string s_datafilename = "parsecache.json";

	private static string s_historyfilename = "parsehistory.json";

	private static int s_requestLimit = 100;

	private List<Action> m_deferredActions;

	private Dictionary<string, string> m_levelStatObjectIds;

	private ParseCache m_cache = new ParseCache();

	private ParseLocalCache m_localCache = new ParseLocalCache();

	private ParseHistory m_history = new ParseHistory();

	private List<CommunityLevel> m_levelsNew;

	private List<CommunityLevel> m_levelsTop;

	private List<CommunityLevel> m_levelsTopAllTime;

	private List<CommunityLevel> m_levelsUser;

	private List<CommunityLevel> m_levelsFeatured;

	private List<CommunityLevel> m_levelsUserSpecific;

	private List<CommunityLevel> m_levelsEasy;

	private List<CommunityLevel> m_levelsMedium;

	private List<CommunityLevel> m_levelsHard;

	private Dictionary<string, int> m_difficultyCache;

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		m_deferredActions = new List<Action>();
		m_levelStatObjectIds = new Dictionary<string, string>();
		m_difficultyCache = new Dictionary<string, int>();
	}

	private void Start()
	{
		if (DataFile.LoadFromFile(s_datafilename, ref m_cache))
		{
			m_cache.AfterLoad();
		}
		else
		{
			m_cache = new ParseCache();
		}
		if (DataFile.LoadFromFile(s_historyfilename, ref m_history))
		{
			m_history.AfterLoad();
		}
		else
		{
			m_history = new ParseHistory();
		}
		m_deferredActions.Add(delegate
		{
			Debug.Log("Requesting scores from Parse...");
			GetAllLevelStats(delegate(Dictionary<string, ScoreStat> stats)
			{
				StatsManager.Instance.SetLevelStats(stats);
			});
		});
		s_updateTime = Time.unscaledTime;
	}

	private void Update()
	{
		if (m_deferredActions.Count > 0)
		{
			m_deferredActions[0]();
			m_deferredActions.RemoveAt(0);
		}
		if (Time.unscaledTime - s_updateTime > s_updateInterval)
		{
			uploadStats(m_cache);
			s_updateTime = Time.unscaledTime;
		}
	}

	public void Reset()
	{
		m_history.CommunityLevelCompletion = new SerializableDictionaryStringInt();
		m_history.CommunityThreeStars = new SerializableDictionaryStringBool();
		m_history.BeforeSave();
		DataFile.SaveToFile(s_historyfilename, m_history);
	}

	public bool GetCachedLevels()
	{
		if (CurrentRequestFilter == LevelRequestFilter.Newest && m_levelsNew != null)
		{
			Levels = m_levelsNew;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.TopRated && m_levelsTop != null)
		{
			Levels = m_levelsTop;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.TopAllTime && m_levelsTopAllTime != null)
		{
			Levels = m_levelsTopAllTime;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.User && m_levelsUser != null)
		{
			Levels = m_levelsUser;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.UserSpecific && m_levelsUserSpecific != null)
		{
			Levels = m_levelsUserSpecific;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.Easy && m_levelsEasy != null)
		{
			Levels = m_levelsEasy;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.Medium && m_levelsMedium != null)
		{
			Levels = m_levelsMedium;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.Hard && m_levelsHard != null)
		{
			Levels = m_levelsHard;
			return true;
		}
		if (CurrentRequestFilter == LevelRequestFilter.Featured && m_levelsFeatured != null)
		{
			Debug.Log("Using cached featured levels.");
			Levels = m_levelsFeatured;
			return true;
		}
		return false;
	}

	public void ClearCachedLevels(LevelRequestFilter filterType)
	{
		switch (filterType)
		{
		case LevelRequestFilter.Newest:
			m_levelsNew = null;
			break;
		case LevelRequestFilter.TopRated:
			m_levelsTop = null;
			break;
		case LevelRequestFilter.TopAllTime:
			m_levelsTopAllTime = null;
			break;
		case LevelRequestFilter.User:
			m_levelsUser = null;
			break;
		case LevelRequestFilter.UserSpecific:
			m_levelsUserSpecific = null;
			break;
		case LevelRequestFilter.Easy:
			m_levelsEasy = null;
			break;
		case LevelRequestFilter.Medium:
			m_levelsMedium = null;
			break;
		case LevelRequestFilter.Hard:
			m_levelsHard = null;
			break;
		case LevelRequestFilter.Featured:
			m_levelsFeatured = null;
			break;
		}
	}

	public void RequestCommunityData(string filter, Action<bool> onComplete)
	{
		if (GetCachedLevels())
		{
			onComplete(true);
			return;
		}
		Debug.Log("Requesting levels from the server...");
		GetCommunityLevels(CurrentRequestFilter, filter, delegate(bool success, List<CommunityLevel> levels)
		{
			if (success)
			{
				if (CurrentRequestFilter == LevelRequestFilter.Newest)
				{
					m_levelsNew = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.TopRated)
				{
					m_levelsTop = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.TopAllTime)
				{
					m_levelsTopAllTime = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.User)
				{
					m_levelsUser = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.UserSpecific)
				{
					m_levelsUserSpecific = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.Featured)
				{
					m_levelsFeatured = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.Easy)
				{
					m_levelsEasy = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.Medium)
				{
					m_levelsMedium = levels;
				}
				else if (CurrentRequestFilter == LevelRequestFilter.Hard)
				{
					m_levelsHard = levels;
				}
				Levels = levels;
				foreach (CommunityLevel level in levels)
				{
					if (m_difficultyCache.ContainsKey(level.ObjectId))
					{
						m_difficultyCache[level.ObjectId] = level.Difficulty;
					}
					else
					{
						m_difficultyCache.Add(level.ObjectId, level.Difficulty);
					}
				}
			}
			onComplete(success);
		});
	}

	public void GetUserScores(IEnumerable<string> userIds, Action<bool> onComplete)
	{
		try
		{
			ParseObject.GetQuery("GameScore").WhereContainedIn("userId", userIds).FindAsync()
				.ContinueWith(delegate(Task<IEnumerable<ParseObject>> t)
				{
					if (!t.IsFaulted)
					{
						IEnumerable<ParseObject> result = t.Result;
						foreach (ParseObject item in result)
						{
							string userId = item.Get<string>("userId");
							int score = item.Get<int>("score");
							DataStore.Instance.SetFriendsScore(userId, score);
						}
						onComplete(true);
					}
					else
					{
						onComplete(false);
					}
				});
		}
		catch (Exception exception)
		{
			onComplete(false);
			Debug.LogException(exception);
		}
	}

	public void PostLevelStats(string levelKey, int solveCount, int attemptCount)
	{
		if (!Application.isEditor)
		{
			if (attemptCount > 0)
			{
				if (m_cache.LevelAttempts.ContainsKey(levelKey))
				{
					m_cache.LevelAttempts[levelKey] += attemptCount;
				}
				else
				{
					m_cache.LevelAttempts.Add(levelKey, attemptCount);
				}
			}
			if (solveCount > 0)
			{
				if (m_cache.LevelSolves.ContainsKey(levelKey))
				{
					m_cache.LevelSolves[levelKey] += solveCount;
				}
				else
				{
					m_cache.LevelSolves.Add(levelKey, solveCount);
				}
			}
			Debug.Log("Caching scores for " + levelKey + " attempts: " + attemptCount + " solves: " + solveCount);
			m_cache.BeforeSave();
			DataFile.SaveToFile(s_datafilename, m_cache);
			return;
		}
		try
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("LevelSolveCount");
			query.WhereEqualTo("key", levelKey).FirstOrDefaultAsync().ContinueWith(delegate(Task<ParseObject> t)
			{
				if (!t.IsFaulted)
				{
					ParseObject result = t.Result;
					if (result == null)
					{
						result = ParseObject.Create("LevelSolveCount");
						result["key"] = levelKey;
						result.Increment("count", solveCount);
						result.Increment("attemptCount", attemptCount);
						result.SaveAsync();
					}
				}
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void PostCommunityStats(int index, LevelCompletion completion)
	{
		bool flag = false;
		string objectId = CommunityManager.Levels[index].ObjectId;
		bool flag2 = m_history.CommunityLevelCompletion.ContainsKey(objectId);
		LevelCompletion levelCompletion = LevelCompletion.Unsolved;
		if (flag2)
		{
			levelCompletion = (LevelCompletion)m_history.CommunityLevelCompletion[objectId];
		}
		bool flag3 = flag2 && (levelCompletion & LevelCompletion.Solved) == LevelCompletion.Solved;
		bool flag4 = flag2 && (levelCompletion & LevelCompletion.Complete) == LevelCompletion.Complete;
		if ((completion & LevelCompletion.Solved) == LevelCompletion.Solved && !flag3)
		{
			if (m_cache.CommunitySolves.ContainsKey(objectId))
			{
				m_cache.CommunitySolves[objectId] = 1;
			}
			else
			{
				m_cache.CommunitySolves.Add(objectId, 1);
			}
			if (!m_history.CommunityLevelCompletion.ContainsKey(objectId))
			{
				m_history.CommunityLevelCompletion.Add(objectId, (int)completion);
			}
			else
			{
				m_history.CommunityLevelCompletion[objectId] = (int)completion;
			}
			flag = true;
		}
		if ((completion & LevelCompletion.Complete) == LevelCompletion.Complete && !flag4)
		{
			if (m_cache.CommunityThreeStars.ContainsKey(objectId))
			{
				m_cache.CommunityThreeStars[objectId] = 1;
			}
			else
			{
				m_cache.CommunityThreeStars.Add(objectId, 1);
			}
			if (!m_history.CommunityLevelCompletion.ContainsKey(objectId))
			{
				m_history.CommunityLevelCompletion.Add(objectId, (int)completion);
			}
			else
			{
				m_history.CommunityLevelCompletion[objectId] = (int)completion;
			}
			flag = true;
		}
		if (!flag2)
		{
			if (m_cache.CommunityAttempts.ContainsKey(objectId))
			{
				m_cache.CommunityAttempts[objectId] = 1;
			}
			else
			{
				m_cache.CommunityAttempts.Add(objectId, 1);
			}
			if (!m_history.CommunityLevelCompletion.ContainsKey(objectId))
			{
				m_history.CommunityLevelCompletion.Add(objectId, (int)completion);
			}
			else
			{
				m_history.CommunityLevelCompletion[objectId] = (int)completion;
			}
		}
		flag = true;
		if (!m_history.CommunityLevelCompletion.ContainsKey(objectId))
		{
			m_history.CommunityLevelCompletion.Add(objectId, (int)completion);
			flag = true;
		}
		else
		{
			LevelCompletion levelCompletion2 = completion | levelCompletion;
			if ((int)levelCompletion2 > m_history.CommunityLevelCompletion[objectId])
			{
				m_history.CommunityLevelCompletion[objectId] = (int)levelCompletion2;
				flag = true;
			}
		}
		if (completion == LevelCompletion.Complete && !m_history.CommunityThreeStars.ContainsKey(objectId))
		{
			flag = true;
			m_history.CommunityThreeStars.Add(objectId, true);
		}
		if (flag)
		{
			Debug.Log("Caching scores for " + objectId);
			m_cache.BeforeSave();
			DataFile.SaveToFile(s_datafilename, m_cache);
			m_history.BeforeSave();
			DataFile.SaveToFile(s_historyfilename, m_history);
		}
	}

	public void PostCommunityLevel(string playerName, string userId, string levelTitle, string levelData, Action<bool> onComplete)
	{
		try
		{
			ParseObject parseObject = ParseObject.Create("CommunityLevel");
			parseObject["key"] = string.Empty;
			parseObject["attempt"] = 0;
			parseObject["solve"] = 0;
			parseObject["threestar"] = 0;
			parseObject["like"] = 0;
			parseObject["data"] = levelData;
			parseObject["author"] = playerName;
			parseObject["userId"] = userId;
			parseObject["title"] = levelTitle;
			parseObject.SaveAsync().ContinueWith(delegate(Task t)
			{
				if (!t.IsFaulted)
				{
					m_levelsNew = null;
				}
				onComplete(!t.IsFaulted);
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			onComplete(false);
		}
	}

	public void DeleteCommunityLevel(string objectId, Action<bool> onComplete)
	{
		try
		{
			ParseObject parseObject = ParseObject.CreateWithoutData("CommunityLevel", objectId);
			parseObject.DeleteAsync().ContinueWith(delegate(Task t)
			{
				if (!t.IsFaulted)
				{
					if (m_levelsNew != null)
					{
						m_levelsNew.RemoveAll((CommunityLevel l) => l.ObjectId == objectId);
					}
					if (m_levelsTop != null)
					{
						m_levelsTop.RemoveAll((CommunityLevel l) => l.ObjectId == objectId);
					}
					if (m_levelsUser != null)
					{
						m_levelsUser.RemoveAll((CommunityLevel l) => l.ObjectId == objectId);
					}
				}
				onComplete(!t.IsFaulted);
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			onComplete(false);
		}
	}

	public void FeatureCommunityLevel(string objectId, int value, Action<bool> onComplete)
	{
		try
		{
			ParseObject parseObject = ParseObject.CreateWithoutData("CommunityLevel", objectId);
			parseObject["feature"] = value;
			parseObject.SaveAsync().ContinueWith(delegate(Task t)
			{
				if (!t.IsFaulted)
				{
					m_levelsFeatured = null;
				}
				onComplete(!t.IsFaulted);
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			onComplete(false);
		}
	}

	public LevelCompletion GetCommunityLevelCompletion(string objectId)
	{
		if (m_history.CommunityLevelCompletion.ContainsKey(objectId))
		{
			return (LevelCompletion)m_history.CommunityLevelCompletion[objectId];
		}
		return LevelCompletion.Unsolved;
	}

	public bool GetCommunityThreeStar(string objectId)
	{
		if (m_history.CommunityThreeStars.ContainsKey(objectId))
		{
			return m_history.CommunityThreeStars[objectId];
		}
		return false;
	}

	public bool IsCommunityLevelAttempted(string objectId)
	{
		if (m_history != null)
		{
			return m_history.CommunityLevelCompletion.ContainsKey(objectId) ? true : false;
		}
		return false;
	}

	public bool IsCommunityLevelLiked(string objectId)
	{
		if (m_history != null)
		{
			return m_history.CommunityLikes.ContainsKey(objectId) ? true : false;
		}
		return false;
	}

	public bool IsCommunityLevelLikedThisSession(string objectId)
	{
		if (m_history != null)
		{
			return m_localCache.CommunityLikes.ContainsKey(objectId) ? true : false;
		}
		return false;
	}

	public void PostCommunityLike(string objectId, bool liked)
	{
		if (m_history.CommunityLikes.ContainsKey(objectId))
		{
			return;
		}
		m_localCache.CommunityLikes.Add(objectId, 1);
		m_history.CommunityLikes.Add(objectId, 1);
		m_history.BeforeSave();
		DataFile.SaveToFile(s_historyfilename, m_history);
		if (liked)
		{
			if (m_cache.CommunityLikes.ContainsKey(objectId))
			{
				m_cache.CommunityLikes[objectId] = 1;
			}
			else
			{
				m_cache.CommunityLikes.Add(objectId, 1);
			}
		}
		Debug.Log("Caching like for " + objectId);
		m_cache.BeforeSave();
		DataFile.SaveToFile(s_datafilename, m_cache);
	}

	public void GetAllLevelStats(Action<Dictionary<string, ScoreStat>> action)
	{
		Dictionary<string, ScoreStat> results = new Dictionary<string, ScoreStat>();
		try
		{
			ParseQuery<ParseObject> query = ParseObject.GetQuery("LevelSolveCount");
			query.WhereStartsWith("key", "A_").OrderBy("key").Limit(200)
				.FindAsync()
				.ContinueWith(delegate(Task<IEnumerable<ParseObject>> t)
				{
					if (!t.IsFaulted)
					{
						IEnumerable<ParseObject> result = t.Result;
						foreach (ParseObject item in result)
						{
							string key = item.Get<string>("key");
							m_levelStatObjectIds.Add(key, item.ObjectId);
							int num = item.Get<int>("count");
							int num2 = item.Get<int>("attemptCount");
							ScoreStat value = new ScoreStat
							{
								SolveCount = num,
								Difficulty = (float)num2 / (float)num
							};
							if (!results.ContainsKey(key))
							{
								results.Add(key, value);
							}
							else
							{
								Debug.LogError("Duplicate key found on the server. Check the data!");
							}
						}
						action(results);
					}
					else
					{
						Debug.LogError("GetAllLevelStats() error: " + t.Exception);
					}
				});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			action(results);
		}
	}

	public void GetCommunityLevels(LevelRequestFilter filterType, string filter, Action<bool, List<CommunityLevel>> action)
	{
		List<CommunityLevel> results = new List<CommunityLevel>();
		try
		{
			ParseQuery<ParseObject> parseQuery = ParseObject.GetQuery("CommunityLevel");
			switch (filterType)
			{
			case LevelRequestFilter.Newest:
				parseQuery = parseQuery.OrderByDescending("createdAt").Limit(s_requestLimit);
				break;
			case LevelRequestFilter.TopRated:
			{
				DateTime dateTime = DateTime.Now - TimeSpan.FromDays(30.0);
				parseQuery = parseQuery.WhereGreaterThan("createdAt", dateTime).OrderByDescending("like").Limit(s_requestLimit);
				break;
			}
			case LevelRequestFilter.TopAllTime:
			{
				DateTime dateTime2 = DateTime.Now - TimeSpan.FromDays(90.0);
				parseQuery = parseQuery.WhereGreaterThan("createdAt", dateTime2).OrderByDescending("like").Limit(s_requestLimit);
				break;
			}
			case LevelRequestFilter.UserSpecific:
				parseQuery = parseQuery.WhereEqualTo("author", filter).OrderByDescending("createdAt").Limit(s_requestLimit);
				break;
			case LevelRequestFilter.Easy:
				parseQuery = parseQuery.WhereGreaterThanOrEqualTo("difficulty", 75).WhereLessThan("difficulty", 95).OrderByDescending("like")
					.Limit(s_requestLimit);
				break;
			case LevelRequestFilter.Medium:
				parseQuery = parseQuery.WhereGreaterThanOrEqualTo("difficulty", 35).WhereLessThan("difficulty", 75).OrderByDescending("like")
					.Limit(s_requestLimit);
				break;
			case LevelRequestFilter.Hard:
				parseQuery = parseQuery.WhereGreaterThanOrEqualTo("difficulty", 5).WhereLessThan("difficulty", 35).OrderByDescending("like")
					.Limit(s_requestLimit);
				break;
			case LevelRequestFilter.User:
				if (CloudOnceAPI.Instance.IsSignedIn())
				{
					string playerDisplayName = CloudOnceAPI.Instance.PlayerDisplayName;
					parseQuery = parseQuery.WhereEqualTo("author", playerDisplayName).OrderByDescending("createdAt").Limit(s_requestLimit);
					break;
				}
				action(true, results);
				return;
			case LevelRequestFilter.Featured:
				parseQuery = parseQuery.WhereEqualTo("feature", 1).OrderByDescending("createdAt").Limit(s_requestLimit);
				break;
			}
			parseQuery.FindAsync().ContinueWith(delegate(Task<IEnumerable<ParseObject>> t)
			{
				if (!t.IsFaulted)
				{
					IEnumerable<ParseObject> result = t.Result;
					foreach (ParseObject item in result)
					{
						CommunityLevel communityLevel = new CommunityLevel
						{
							ObjectId = item.ObjectId,
							UserId = ((!item.ContainsKey("userId")) ? string.Empty : item.Get<string>("userId")),
							Author = ((!item.ContainsKey("author")) ? string.Empty : item.Get<string>("author")),
							Data = ((!item.ContainsKey("data")) ? string.Empty : item.Get<string>("data")),
							Title = ((!item.ContainsKey("title")) ? string.Empty : item.Get<string>("title")),
							AttemptCount = (item.ContainsKey("attempt") ? item.Get<int>("attempt") : 0),
							SolveCount = (item.ContainsKey("solve") ? item.Get<int>("solve") : 0),
							ThreeStarCount = (item.ContainsKey("threestar") ? item.Get<int>("threestar") : 0),
							LikeCount = (item.ContainsKey("like") ? item.Get<int>("like") : 0),
							Featured = (item.ContainsKey("feature") ? item.Get<int>("feature") : 0)
						};
						if (communityLevel.AttemptCount > 0)
						{
							communityLevel.Difficulty = (communityLevel.SolveCount * 100 / communityLevel.AttemptCount + communityLevel.ThreeStarCount * 100 / communityLevel.AttemptCount) / 2;
						}
						else
						{
							communityLevel.Difficulty = 0;
						}
						results.Add(communityLevel);
					}
					action(true, results);
				}
				else
				{
					Debug.LogError("GetCommunityLevels() error: " + t.Exception);
					action(false, results);
				}
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			action(false, results);
		}
	}

	private void uploadStats(ParseCache cache)
	{
		if (cache == null || Instance == null)
		{
			return;
		}
		Dictionary<string, ParseObject> dictionary = new Dictionary<string, ParseObject>();
		if (cache.LevelAttempts.Count > 0)
		{
			foreach (KeyValuePair<string, int> levelAttempt in cache.LevelAttempts)
			{
				string key = levelAttempt.Key;
				string text = null;
				if (m_levelStatObjectIds.ContainsKey(key))
				{
					text = m_levelStatObjectIds[key];
				}
				if (text != null)
				{
					ParseObject parseObject = ParseObject.CreateWithoutData("LevelSolveCount", text);
					parseObject.Increment("attemptCount", levelAttempt.Value);
					if (cache.LevelSolves.ContainsKey(key))
					{
						int num = cache.LevelSolves[key];
						parseObject.Increment("count", num);
					}
					dictionary.Add(key, parseObject);
				}
			}
		}
		if (cache.CommunitySolves.Count > 0)
		{
			foreach (KeyValuePair<string, int> communitySolf in cache.CommunitySolves)
			{
				string key2 = communitySolf.Key;
				if (key2 != null)
				{
					ParseObject parseObject2 = null;
					parseObject2 = ((!dictionary.ContainsKey(key2)) ? ParseObject.CreateWithoutData("CommunityLevel", key2) : dictionary[key2]);
					parseObject2.Increment("solve", communitySolf.Value);
					if (!dictionary.ContainsKey(key2))
					{
						dictionary.Add(key2, parseObject2);
					}
				}
			}
		}
		if (cache.CommunityThreeStars.Count > 0)
		{
			foreach (KeyValuePair<string, int> communityThreeStar in cache.CommunityThreeStars)
			{
				string key3 = communityThreeStar.Key;
				if (key3 != null)
				{
					ParseObject parseObject3 = null;
					parseObject3 = ((!dictionary.ContainsKey(key3)) ? ParseObject.CreateWithoutData("CommunityLevel", key3) : dictionary[key3]);
					parseObject3.Increment("threestar", communityThreeStar.Value);
					if (!dictionary.ContainsKey(key3))
					{
						dictionary.Add(key3, parseObject3);
					}
				}
			}
		}
		if (cache.CommunityAttempts.Count > 0)
		{
			foreach (KeyValuePair<string, int> communityAttempt in cache.CommunityAttempts)
			{
				string key4 = communityAttempt.Key;
				if (key4 != null)
				{
					ParseObject parseObject4 = null;
					parseObject4 = ((!dictionary.ContainsKey(key4)) ? ParseObject.CreateWithoutData("CommunityLevel", key4) : dictionary[key4]);
					parseObject4.Increment("attempt", communityAttempt.Value);
					if (m_difficultyCache.ContainsKey(key4))
					{
						parseObject4["difficulty"] = m_difficultyCache[key4];
					}
					if (!dictionary.ContainsKey(key4))
					{
						dictionary.Add(key4, parseObject4);
					}
				}
			}
		}
		if (cache.CommunityLikes.Count > 0)
		{
			foreach (KeyValuePair<string, int> communityLike in cache.CommunityLikes)
			{
				string key5 = communityLike.Key;
				if (key5 != null)
				{
					ParseObject parseObject5 = null;
					parseObject5 = ((!dictionary.ContainsKey(key5)) ? ParseObject.CreateWithoutData("CommunityLevel", key5) : dictionary[key5]);
					parseObject5.Increment("like", communityLike.Value);
					if (!dictionary.ContainsKey(key5))
					{
						dictionary.Add(key5, parseObject5);
					}
				}
			}
		}
		if (dictionary.Count > 0)
		{
			try
			{
				Debug.Log("Uploading " + dictionary.Count + " rows of stats to the server...");
				ParseObject.SaveAllAsync(dictionary.Values);
				m_cache.LevelAttempts.Clear();
				m_cache.LevelSolves.Clear();
				m_cache.CommunityAttempts.Clear();
				m_cache.CommunitySolves.Clear();
				m_cache.CommunityThreeStars.Clear();
				m_cache.CommunityLikes.Clear();
				DataFile.Delete(s_datafilename);
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return;
			}
		}
		Debug.Log("No stats to upload to the server.");
	}
}
