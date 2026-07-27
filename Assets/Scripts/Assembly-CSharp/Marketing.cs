using UnityEngine;

public class Marketing
{
	private static readonly string s_moregamesURL = "https://play.google.com/store/apps/developer?id=Orbital+Nine+Games";

	private static readonly string s_newgameURL = "market://details?id=com.orbital.donttouchit";

	private static readonly string s_reviewURL = "market://details?id=com.orbital.brainiton";

	public static readonly string WebsiteURL = "http://brainitongame.com/";

	public static readonly string FacebookPageURL = "http://www.facebook.com/brainitongame";

	public static readonly string TwitterPageURL = "http://twitter.com/OrbitalNine";

	public static readonly string PlaytestFeedbackURL = "https://docs.google.com/forms/d/15MkiRFCReE7XvQqhABhNn3y_YeFSJkq84_M6jIg24Hs/viewform";

	public static readonly string PlaytestFeedbackLevelURL = "https://docs.google.com/forms/d/15MkiRFCReE7XvQqhABhNn3y_YeFSJkq84_M6jIg24Hs/viewform?entry.822741564={0}";

	public static readonly string CommunityURL = "https://everyplay.com/brain-it-on-/videos";

	public static readonly string s_socialMessage_Twitter = "Sit back and relax with #PicturePuzzle";

	public static readonly string s_socialMessage_Facebook = "Sit back and relax with Picture Puzzle";

	public static readonly string s_socialName = "Picture Puzzle";

	public static readonly string s_socialCaption = "Download from the Google Play store for free";

	public static readonly string GameName = "Brain It On!";

	public static readonly int s_completedLevelsUntilReview = 25;

	public static readonly int s_timesToRemindToReview = 2;

	private static int s_playedCount;

	public static void ShowNewGame()
	{
		AnalyticsManager.LogEvent("Feedback", "NewGame", "Clicked", 1L);
		Application.OpenURL(s_newgameURL);
	}

	public static void ShowMoreGames()
	{
		AnalyticsManager.LogEvent("Feedback", "MoreGames", "Clicked", 1L);
		Application.OpenURL(s_moregamesURL);
	}

	public static void ShowRateUs()
	{
		AnalyticsManager.LogEvent("Feedback", "RateUs", "Clicked", 1L);
		Application.OpenURL(s_reviewURL);
	}

	public static void ShowFeedback()
	{
		AnalyticsManager.LogEvent("Feedback", "Feedback", "Clicked", 1L);
		EmailSender.Send("support@brainitongame.com", "Brain It On! Feedback / Bug", "* Please send screenshots of bugs if possible. Thank you very much. *");
	}

	public static void ShowWebsite()
	{
		AnalyticsManager.LogEvent("Feedback", "ViewWebsite", "Clicked", 1L);
		Application.OpenURL(WebsiteURL);
	}

	public static void ShowFacebookPage()
	{
		AnalyticsManager.LogEvent("Feedback", "ViewFacebook", "Clicked", 1L);
		Application.OpenURL(FacebookPageURL);
	}

	public static void ShowTwitterPage()
	{
		AnalyticsManager.LogEvent("Feedback", "ViewTwitter", "Clicked", 1L);
		Application.OpenURL(TwitterPageURL);
	}

	public static bool IsItTimeToShowAnAd()
	{
		s_playedCount++;
		if (s_playedCount >= DataStore.Instance.ConfigSettings.AdsInterval)
		{
			s_playedCount = 0;
			return true;
		}
		return false;
	}
}
