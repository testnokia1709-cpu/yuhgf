using UnityEngine;

public class GoogleAnalyticsManager : MonoBehaviour
{
	public static GoogleAnalyticsManager Instance;

	private GoogleAnalyticsV4 m_googleAnalytics;

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

	private void Start()
	{
		m_googleAnalytics = GoogleAnalyticsV4.instance;
		if (m_googleAnalytics != null)
		{
			TextAsset textAsset = Resources.Load("bundleversion") as TextAsset;
			m_googleAnalytics.bundleVersion = textAsset.text;
			Debug.Log("Bundle version: " + textAsset.text);
		}
	}

	public static void LogScreen(ScreenName screen)
	{
		if (Instance != null && Instance.m_googleAnalytics != null)
		{
			Instance.m_googleAnalytics.LogScreen(screen.ToString());
		}
	}

	public static void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
	{
		if (Instance != null && Instance.m_googleAnalytics != null)
		{
			Instance.m_googleAnalytics.LogEvent(eventCategory, eventAction, eventLabel, value);
		}
	}

	public static void LogAverageGameplayFPS(long value, string label)
	{
		if (Instance != null && Instance.m_googleAnalytics != null)
		{
			Instance.m_googleAnalytics.LogTiming("GameplayFPS", value, "Average", label);
		}
	}

	public static void LogVote(string levelName, bool thumbUp)
	{
		if (Instance != null && Instance.m_googleAnalytics != null)
		{
			Instance.m_googleAnalytics.LogEvent("Opinion", "LevelVote", levelName, thumbUp ? 1 : (-1));
		}
	}
}
