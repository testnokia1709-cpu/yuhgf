using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager
{
	public static void LogGameEvent(string eventAction, string eventLabel)
	{
		GoogleAnalyticsManager.LogEvent("GameEvent", eventAction, eventLabel, 1L);
	}

	public static void LogGameplayEvent(string eventAction)
	{
		GoogleAnalyticsManager.LogEvent("Gameplay", eventAction, LevelManager.Instance.GetLevelName(), 1L);
	}

	public static void LogGameplayValueEvent(string eventAction, long value)
	{
		GoogleAnalyticsManager.LogEvent("Gameplay", eventAction, LevelManager.Instance.GetLevelName(), value);
	}

	public static void LogChoiceEvent(string eventAction, string eventLabel)
	{
		GoogleAnalyticsManager.LogEvent("Decision", eventAction, eventLabel, 1L);
	}

	public static void LogEvent(string eventCategory, string eventAction, string eventLabel, long value)
	{
		GoogleAnalyticsManager.LogEvent(eventCategory, eventAction, eventLabel, value);
	}

	public static void LogScreen(ScreenName screen)
	{
		GoogleAnalyticsManager.LogScreen(screen);
	}

	public static void LogAverageGameplayFPS(long value, string label)
	{
		GoogleAnalyticsManager.LogAverageGameplayFPS(value, label);
	}

	public static void LogVote(string levelName, bool thumbUp)
	{
		GoogleAnalyticsManager.LogVote(levelName, thumbUp);
	}

	public static void LogDebugEvent(string eventName)
	{
		LogDebugEvent(eventName, new Dictionary<string, object> { 
		{
			"Device",
			SystemInfo.deviceName + "-" + SystemInfo.deviceModel
		} });
	}

	public static void LogDebugEvent(string eventName, Dictionary<string, object> parameters)
	{
		Analytics.CustomEvent(eventName, parameters);
	}

	public static void LogPurchase(string productName, double price, string currency)
	{
		LogEvent("Store", "MarketPurchase_Complete", productName, 1L);
		Analytics.Transaction(productName, (decimal)price, currency, null, null);
	}
}
