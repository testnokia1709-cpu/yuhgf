using System;
using System.Collections.Generic;
using NBidi;
using UnityEngine;
using UnityEngine.UI;

public class TextLibrary : MonoBehaviour
{
	public static TextLibrary Instance;

	public static string AppVersion;

	public static string[] CompleteMessages = new string[4] { "Nice one!", "Go Brain Go!", "Success!", "Brain it on!" };

	public static string Credits = "Development / Design\nAaron Lake\n\nMusic\n'Rolling at 5' Kevin Macleod (incompetech.com)\n\nTranslations\nAltaire Valdés\nAngelos Vlioras\nAntti Huittinen\nAriel Elior\nAppLingua.com\nBlas López\nClement Delpierre\nFredrik Moe\nLiam Schnell\nMahdi Chaman\nMaman Durahman\nMattias Andersson\nMayu Lake\nMehmet Hüdayi\nMika Jacobs\nNiklas Vennerstrøm\nPedro Ivo\nRobert Saric\nSeweryn Kalemba\nShirley Man\nSina Yasrebi\nSparkLightGames\nTayyib Cankat\nThanh Nam\nThitikorn Khamthita\nTörök József\nVictor Fabiano\nVital\nVojtěch Surovec\nYoung Kwon\n\nLevel 155 - Jason Freake\n\n(c) OrbitalNine 2017\n";

	private static Dictionary<StringId, string> s_stringLibrary = new Dictionary<StringId, string>
	{
		{
			StringId.S_EMPTY,
			string.Empty
		},
		{
			StringId.S_SOCIAL_MESSAGE,
			"I solved level {0} in #BrainItOn"
		},
		{
			StringId.S_SOCIAL_STARS,
			" with ☆☆☆."
		},
		{
			StringId.S_SOCIAL_POSTTOTWITTER,
			"Share on Twitter"
		},
		{
			StringId.S_SOCIAL_TWITTERACTION,
			"Tweet"
		},
		{
			StringId.S_SOCIAL_POSTTOFACEBOOK,
			"Share on Facebook"
		},
		{
			StringId.S_SOCIAL_FACEBOOKACTION,
			"Post"
		},
		{
			StringId.S_SOCIAL_FACEBOOK_PLACEHOLDER,
			"Enter message..."
		},
		{
			StringId.S_LOADING,
			"Loading..."
		},
		{
			StringId.S_REVIEWQUESTION,
			"Would you like to rate this game?"
		},
		{
			StringId.S_REVIEW_YES,
			"I'm enjoying it, sure."
		},
		{
			StringId.S_REVIEW_BUG,
			"Report a bug"
		},
		{
			StringId.S_REVIEW_LATER,
			"Not now"
		},
		{
			StringId.S_ROWLOCKED,
			"This row is still locked. You have to solve 4 out of 5 levels in the previous row to unlock it."
		},
		{
			StringId.S_OK,
			"Ok"
		},
		{
			StringId.S_RESETDATA_QUESTION,
			"Do you want to clear your game progress?"
		},
		{
			StringId.S_RESETDATA_YES,
			"Clear Progress"
		},
		{
			StringId.S_CANCEL,
			"Cancel"
		},
		{
			StringId.S_NOTICEBOARD,
			"Notice Board"
		},
		{
			StringId.S_NOTICEBOARD_NONOTICES,
			"No notices available, please check back later."
		},
		{
			StringId.S_CLOSE,
			"Close"
		},
		{
			StringId.S_CREDITS,
			"Credits"
		},
		{
			StringId.S_FACEBOOK_LOGOUT_QUESTION,
			"Would you like to log out of Facebook?"
		},
		{
			StringId.S_FACEBOOK_LOGOUT_NO,
			"Stay Connected"
		},
		{
			StringId.S_FACEBOOK_LOGOUT_DONE,
			"Logged out and Facebook data cleared."
		},
		{
			StringId.S_DOWNLOAD_GAME,
			"Download Brain It On! for your mobile device to unlock all the puzzle sets."
		},
		{
			StringId.S_ITEM_UNLOCKED,
			"Congratulations! You've unlocked {0}."
		},
		{
			StringId.S_ITEM_UNLOCKED_OK,
			"Nice"
		},
		{
			StringId.S_CONNECTING_STORE,
			"Connecting to store..."
		},
		{
			StringId.S_STORE_ERROR,
			"We're sorry, an error occurred while contacting the store, please try again later."
		},
		{
			StringId.S_PURCHASES_RESTORED,
			"Your purchases have been restored."
		},
		{
			StringId.S_PURCHASE,
			"Purchase"
		},
		{
			StringId.S_PURCHASE_COMPLETE,
			"Purchase complete. Thank you for your support!"
		},
		{
			StringId.S_PURCHASE_NO,
			"Not Now"
		},
		{
			StringId.S_INDIE_SUPPORT,
			"Thank you for supporting small indie developers!"
		},
		{
			StringId.S_SETTINGS_TITLE,
			"Settings"
		},
		{
			StringId.S_RESTORE_PURCHASES,
			"Restore Purchases"
		},
		{
			StringId.S_RESETGAME,
			"Reset Game"
		},
		{
			StringId.S_COMINGSOON,
			"Coming Soon..."
		},
		{
			StringId.S_LOGIN,
			"Login"
		},
		{
			StringId.S_LOGOUT,
			"Log Out"
		},
		{
			StringId.S_NOADS,
			"No Ads"
		},
		{
			StringId.S_FACEBOOK_NOTSUPPORTED,
			"Sorry, Facebook Login is only supported on mobile devices currently."
		},
		{
			StringId.S_LEVEL_COMPLETE,
			"Congratulations!"
		},
		{
			StringId.S_BUYNOW,
			"Buy Now"
		},
		{
			StringId.S_SOLVED,
			"Solved"
		},
		{
			StringId.S_QUIT_CONFIRMATION,
			"Are you sure you want to quit?"
		},
		{
			StringId.S_YES,
			"Yes"
		},
		{
			StringId.S_NO,
			"No"
		},
		{
			StringId.S_GETAHINT,
			"Watch a video to get a hint?"
		},
		{
			StringId.S_WATCHVIDEO_SKIPPED,
			"Please watch the entire video to get a hint."
		},
		{
			StringId.S_HINT_TITLE,
			"Hint"
		},
		{
			StringId.S_WATCHVIDEO_FAILED,
			"An error occurred loading your video. Please try again later."
		},
		{
			StringId.S_LEVELEDITOR,
			"Level Editor"
		},
		{
			StringId.S_ACHIEVEMENTS,
			"Achievements"
		},
		{
			StringId.S_STORE,
			"Store"
		},
		{
			StringId.S_UNLOCK,
			"Unlock"
		},
		{
			StringId.S_SOLVEDTIMES,
			"Solved {0} times"
		},
		{
			StringId.S_NOTENOUGHSTARS,
			"You do not have enough stars yet, please collect more to unlock this pack."
		},
		{
			StringId.S_LANGUAGE,
			"Language"
		},
		{
			StringId.S_RECORDS,
			"Best Results"
		},
		{
			StringId.S_BUYFULLGAME,
			"Buy the full game"
		},
		{
			StringId.S_BUYEXPLANATION1,
			"- Unlocks all levels"
		},
		{
			StringId.S_BUYEXPLANATION2,
			"- Unlocks all hints"
		},
		{
			StringId.S_BUYEXPLANATION3,
			"- Removes all ads"
		},
		{
			StringId.S_BUYEXPLANATION4,
			"(The price is discounted if you have already purchased items)"
		},
		{
			StringId.S_GOAL,
			"Goal:"
		},
		{
			StringId.S_TOTALTIME,
			"Total time: {0:mm:ss}"
		},
		{
			StringId.S_TOTALSHAPE,
			"Total shapes: {0}"
		},
		{
			StringId.S_GAMECENTER,
			"Sign in using your Game Center to safeguard your progress and to play on multiple iOS devices."
		},
		{
			StringId.S_UNLOCKCOMMUNITY,
			"You must solve all the levels to unlock the community levels."
		},
		{
			StringId.S_SERVERERROR,
			"Error contacting server, please try again later."
		},
		{
			StringId.S_LEVELEDITOR_NOTAVAILABLE,
			"The level editor is only available if you've purchased the full game."
		},
		{
			StringId.S_PROMPT_DELETE,
			"Are you sure you want to delete this level?"
		},
		{
			StringId.S_DELETED,
			"Level deleted successfully."
		},
		{
			StringId.S_FUEL_SELECTLEVEL,
			"Select a Level"
		},
		{
			StringId.S_FUEL_CURRENTSCORE,
			"Your current score: {0}"
		},
		{
			StringId.S_FUEL_WATCHVIDEO,
			"Watch a video to try again?"
		},
		{
			StringId.S_EDITOR_PHYSICS_ACTIVE,
			"Physics active on start"
		},
		{
			StringId.S_COMMUNITY_LEVELS,
			"Community Levels"
		},
		{
			StringId.S_EDITOR_GROUND,
			"Ground"
		},
		{
			StringId.S_LEVEL_ERROR,
			"Error loading level. Please upgrade to the latest version of the game."
		},
		{
			StringId.S_TRY,
			"Try"
		},
		{
			StringId.S_TRY_GETAHINT,
			"Watch a video to try a level?"
		},
		{
			StringId.S_TRY_WATCHVIDEO_SKIPPED,
			"Please watch the entire video to earn your reward."
		},
		{
			StringId.S_DRAW_A_SHAPE,
			"Draw a shape"
		},
		{
			StringId.S_BALL_TOUCH_LEFTWALL,
			"Make the ball hit the left wall"
		},
		{
			StringId.S_003,
			"Tilt the shape to the right"
		},
		{
			StringId.S_004,
			"Tip the glass onto the ground"
		},
		{
			StringId.S_005,
			"Get the ball out of the bowl"
		},
		{
			StringId.S_PLACE_OBJECT_IN_GLASS,
			"Place an object inside the glass"
		},
		{
			StringId.S_007,
			"Place only two balls in the glass"
		},
		{
			StringId.S_008,
			"Place an object in the small bin"
		},
		{
			StringId.S_009,
			"Get the ball off the stand"
		},
		{
			StringId.S_BALL_TOUCH_GROUND,
			"Make the ball touch the ground"
		},
		{
			StringId.S_011,
			"Make an object touch the red area"
		},
		{
			StringId.S_012,
			"Remove the cap"
		},
		{
			StringId.S_013,
			"Lift the ball off the ground"
		},
		{
			StringId.S_014,
			"Get the ball out of the jar"
		},
		{
			StringId.S_CLEAR_OBJECTS_FROM_PLATFORM,
			"Clear all objects from the platform"
		},
		{
			StringId.S_016,
			"Lift the orange box off the ground"
		},
		{
			StringId.S_017,
			"Make the ring touch the right wall"
		},
		{
			StringId.S_018,
			"Make the ball touch the left wall"
		},
		{
			StringId.S_BALL_INSIDE_ORANGE_BOX,
			"Place the ball in the orange box"
		},
		{
			StringId.S_020,
			"Balance the platform"
		},
		{
			StringId.S_BALL_TOUCH_RIGHTWALL,
			"Make the ball touch the right wall"
		},
		{
			StringId.S_022,
			"Get the ball out of the container"
		},
		{
			StringId.S_023,
			"Place the ball in the orange box"
		},
		{
			StringId.S_024,
			"Get the ball out of the vase"
		},
		{
			StringId.S_025,
			"Flip the glass upside down"
		},
		{
			StringId.S_026,
			"Get the ball out of the shape"
		},
		{
			StringId.S_BALLS_INSIDE_ORANGE_BOX,
			"Place the balls in the orange box"
		},
		{
			StringId.S_028,
			"Make the ball hit the right wall"
		},
		{
			StringId.S_029,
			"Place an object inside the glass"
		},
		{
			StringId.S_030,
			"Place the ball in the orange box"
		},
		{
			StringId.S_031,
			"Place the ball in the orange box"
		},
		{
			StringId.S_032,
			"Place the ball in the orange box"
		},
		{
			StringId.S_033,
			"Place the ball in the orange box"
		},
		{
			StringId.S_OBJECT_INSIDE_ORANGE_BOX,
			"Place an object inside the orange box"
		},
		{
			StringId.S_GET_BALL_OUT_OF_SHAPE,
			"Get the ball out of the shape"
		},
		{
			StringId.S_BALL_TOUCH_LEFTRIGHT,
			"Make the ball hit the left and right wall"
		},
		{
			StringId.S_037,
			"Make the ball touch the right wall"
		},
		{
			StringId.S_BALL_TOUCH_CEILING,
			"Make the ball touch the ceiling"
		},
		{
			StringId.S_039,
			"Make the ball hit the right wall"
		},
		{
			StringId.S_040,
			"Place the ball in the orange box"
		},
		{
			StringId.S_041,
			"Make the ball hit the right wall"
		},
		{
			StringId.S_042,
			"Make the ball hit the right wall"
		},
		{
			StringId.S_GET_ORANGEBALL_OUT_OF_BOX,
			"Get the orange ball out of the box"
		},
		{
			StringId.S_044,
			"Place the balls in their boxes"
		},
		{
			StringId.S_045,
			"Place the ball in the orange box"
		},
		{
			StringId.S_046,
			"Flip the box over"
		},
		{
			StringId.S_047,
			"Tip the shape onto its side"
		},
		{
			StringId.S_SHAPE_TOUCH_CEILING,
			"Make the shape touch the ceiling"
		},
		{
			StringId.S_049,
			"Make the ball hit the left wall"
		},
		{
			StringId.S_050,
			"Make the ball hit the left wall"
		},
		{
			StringId.S_OBJECT_TOUCH_RIGHTWALL,
			"Make an object hit the right wall"
		},
		{
			StringId.S_052,
			"Make the orange ball hit the ground"
		},
		{
			StringId.S_OBJECT_TOUCH_CEILING,
			"Make an object touch the ceiling"
		},
		{
			StringId.S_054,
			"Make the ball touch the ground"
		},
		{
			StringId.S_055,
			"Make the ball hit the left wall"
		},
		{
			StringId.S_056,
			"Place the ball in the right box and the plank in the left box"
		},
		{
			StringId.S_057,
			"Make the ball touch the ground"
		},
		{
			StringId.S_058,
			"Lift the triangle off the ground"
		},
		{
			StringId.S_059,
			"Place the plank in the orange box"
		},
		{
			StringId.S_THREEBALLS_INSIDE_ORANGE_BOX,
			"Place three balls in the orange box"
		},
		{
			StringId.S_061,
			"Take the cap off the box"
		},
		{
			StringId.S_062,
			"Take the shell off the dome"
		},
		{
			StringId.S_SHAPE_IN_ORANGE_BOX,
			"Place a shape in the orange box"
		},
		{
			StringId.S_OBJECT_TOUCH_LEFTRIGHTWALL,
			"Make the same object touch the left and right wall"
		},
		{
			StringId.S_065,
			"Place the ball in the orange box"
		},
		{
			StringId.S_SORT_THE_BALLS,
			"Sort the colored balls"
		},
		{
			StringId.S_OBJECT_TOUCH_GROUND,
			"Make an object touch the ground"
		},
		{
			StringId.S_068,
			"Make the ball touch the right wall"
		},
		{
			StringId.S_069,
			"Make the ball touch the right wall"
		},
		{
			StringId.S_070,
			"Stop both gears"
		},
		{
			StringId.S_OBJECT_TOUCH_ALL_WALLS,
			"Make an object touch all four walls"
		},
		{
			StringId.S_BALLS_TOUCH_CEILING,
			"Make the balls touch the ceiling"
		},
		{
			StringId.S_073,
			"Make an object touch the left wall"
		},
		{
			StringId.S_SHAPE_IN_GLASS,
			"Place the shape inside the glass"
		},
		{
			StringId.S_076,
			"Make the orange ball touch the right wall"
		},
		{
			StringId.S_077,
			"Make the ball touch the right wall"
		},
		{
			StringId.S_BALL_INSIDE_GLASS,
			"Place the ball in the glass"
		},
		{
			StringId.S_SHAPE_TOUCH_RIGHTWALL,
			"Make the shape touch the right wall"
		},
		{
			StringId.S_080,
			"Place the balls in their boxes"
		},
		{
			StringId.S_MAGNET_TOUCH,
			"Make the magnets touch"
		},
		{
			StringId.S_MAGNET_SEPARATE,
			"Separate the magnets"
		},
		{
			StringId.S_BALLS_TOUCH_GROUND,
			"Make the balls touch the ground"
		},
		{
			StringId.S_SORT_BY_COLOR,
			"Sort by color"
		},
		{
			StringId.S_ACHIEVEMENTS_AVAILABLE,
			"Achievements are available if you've purchased the game."
		},
		{
			StringId.S_ACHIEVEMENT_THREESTARS,
			"Solve {0} levels with 3 stars."
		},
		{
			StringId.S_ACHIEVEMENT_BELOWSHAPEGOAL,
			"Solve {0} levels below the shape goal."
		},
		{
			StringId.S_ACHIEVEMENT_DRAWSHAPES,
			"Draw {0} shapes."
		},
		{
			StringId.S_ACHIEVEMENT_ONESHAPE,
			"Solve {0} levels with only 1 shape."
		},
		{
			StringId.S_ACHIEVEMENT_ONESECOND,
			"Solve {0} levels in less than 1 second."
		},
		{
			StringId.S_ACHIEVEMENT_FIVEBELOWTIME,
			"Solve {0} levels 5 seconds below the time goal."
		},
		{
			StringId.S_ACHIEVEMENT_TENBELOWTIME,
			"Solve {0} levels 10 seconds below the time goal."
		}
	};

	public static List<SystemLanguage> s_supportedLanguages = new List<SystemLanguage>
	{
		SystemLanguage.English,
		SystemLanguage.French,
		SystemLanguage.German,
		SystemLanguage.Italian,
		SystemLanguage.Spanish,
		SystemLanguage.Russian,
		SystemLanguage.ChineseSimplified,
		SystemLanguage.ChineseTraditional,
		SystemLanguage.Japanese,
		SystemLanguage.Korean,
		SystemLanguage.Polish,
		SystemLanguage.Portuguese,
		SystemLanguage.Turkish,
		SystemLanguage.Dutch,
		SystemLanguage.Hebrew,
		SystemLanguage.Arabic,
		SystemLanguage.Slovak,
		SystemLanguage.Vietnamese,
		SystemLanguage.Hungarian,
		SystemLanguage.Swedish,
		SystemLanguage.Indonesian,
		SystemLanguage.Catalan,
		SystemLanguage.Danish,
		SystemLanguage.Greek,
		SystemLanguage.Finnish,
		SystemLanguage.Norwegian,
		SystemLanguage.Thai,
		SystemLanguage.Czech
	};

	public Font FontWestern;

	public Font FontArabic;

	public Font FontPersian;

	private static SystemLanguage s_systemLanguage;

	private static string s_overrideLanguage;

	private static List<string> s_rightToLeftLanguages = new List<string> { "Arabic", "Hebrew", "Farsi" };

	public static string Get(StringId id)
	{
		string text = s_stringLibrary[id];
		if (s_rightToLeftLanguages.Contains(s_overrideLanguage))
		{
			text = global::NBidi.NBidi.LogicalToVisual(text);
		}
		text = text.Replace("\\n", "\n");
		return text.TrimEnd();
	}

	public static string GetCompleteMessage()
	{
		int num = UnityEngine.Random.Range(0, CompleteMessages.Length - 1);
		return CompleteMessages[num];
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
		s_systemLanguage = Application.systemLanguage;
		loadLanguage(s_systemLanguage);
		TextAsset textAsset = Resources.Load("bundleversion") as TextAsset;
		AppVersion = textAsset.text;
	}

	public void SetFont(List<Text> textObjects)
	{
		Font font = FontWestern;
		if (s_overrideLanguage == SystemLanguage.Arabic.ToString())
		{
			font = FontArabic;
		}
		else if (s_overrideLanguage == "Farsi")
		{
			font = FontPersian;
		}
		foreach (Text textObject in textObjects)
		{
			if (textObject != null)
			{
				textObject.font = font;
			}
		}
	}

	public void SetFont(Text text)
	{
		Font font = FontWestern;
		if (s_overrideLanguage == SystemLanguage.Arabic.ToString())
		{
			font = FontArabic;
		}
		else if (s_overrideLanguage == "Farsi")
		{
			font = FontPersian;
		}
		text.font = font;
	}

	public void LoadLanguage(string language)
	{
		bool flag = true;
		try
		{
			SystemLanguage sysLanguage = (SystemLanguage)Enum.Parse(typeof(SystemLanguage), language);
			loadLanguage(sysLanguage);
		}
		catch (Exception)
		{
			flag = false;
		}
		if (!flag)
		{
			s_overrideLanguage = language;
			loadTranslation(language);
		}
	}

	private void loadLanguage(SystemLanguage sysLanguage)
	{
		s_overrideLanguage = sysLanguage.ToString();
		string language;
		if (s_supportedLanguages.Contains(sysLanguage))
		{
			language = sysLanguage.ToString();
		}
		else
		{
			switch (sysLanguage)
			{
			case SystemLanguage.SerboCroatian:
				language = "Croatian";
				break;
			case SystemLanguage.Chinese:
				language = SystemLanguage.ChineseSimplified.ToString();
				break;
			default:
				language = SystemLanguage.English.ToString();
				break;
			}
		}
		loadTranslation(language);
	}

	private void loadTranslation(string language)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(language, typeof(TextAsset));
		if (!(textAsset != null))
		{
			return;
		}
		string[] array = textAsset.ToString().Split('\n');
		List<StringId> list = new List<StringId>(s_stringLibrary.Keys);
		foreach (StringId item in list)
		{
			int num = (int)(item - 1);
			if (num >= 0 && num < array.Length)
			{
				s_stringLibrary[item] = array[num];
			}
		}
	}
}
