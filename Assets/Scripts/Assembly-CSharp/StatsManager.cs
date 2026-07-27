using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance;

	private Dictionary<string, ScoreStat> m_stats;

	private Dictionary<string, CommunityStat> m_communityStats;

	private List<Action> m_deferredActions;

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
	}

	private void Start()
	{
		m_stats = new Dictionary<string, ScoreStat>();
	}

	private void Update()
	{
		if (m_deferredActions.Count > 0)
		{
			m_deferredActions[0]();
			m_deferredActions.RemoveAt(0);
		}
	}

	public void PostLevelSolve(string levelKey, int solveCount, int attemptCount)
	{
		ParseAPI.Instance.PostLevelStats(levelKey, solveCount, attemptCount);
		if (m_stats.ContainsKey(levelKey))
		{
			ScoreStat scoreStat = m_stats[levelKey];
			scoreStat.SolveCount++;
			m_stats[levelKey] = scoreStat;
		}
	}

	public void PostCommunitySolve(int index, LevelCompletion completion)
	{
		ParseAPI.Instance.PostCommunityStats(index, completion);
	}

	public void GetLevelSolveCount(string levelKey, Action<int> action)
	{
		int num = 0;
		float num2 = 0f;
		if (m_stats.ContainsKey(levelKey))
		{
			num = m_stats[levelKey].SolveCount;
			num2 = m_stats[levelKey].Difficulty;
		}
		action(num);
		Debug.Log("Retrieving cached score: " + num + " diff = " + num2);
	}

	public void SetLevelStats(Dictionary<string, ScoreStat> stats)
	{
		m_stats = stats;
		Debug.Log("Received scores from Parse. " + stats.Count + " entries.");
	}

	public void SetCommunityStats(Dictionary<string, CommunityStat> stats)
	{
		m_communityStats = stats;
		Debug.Log("Received community stats from Parse. " + stats.Count + " entries.");
	}

	public CommunityStat GetCommunityStats(string levelKey)
	{
		if (m_communityStats.ContainsKey(levelKey))
		{
			return m_communityStats[levelKey];
		}
		return new CommunityStat();
	}
}
