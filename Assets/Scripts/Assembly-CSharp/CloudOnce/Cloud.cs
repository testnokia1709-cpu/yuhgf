using System;
using CloudOnce.Internal;
using CloudOnce.Internal.Providers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce
{
	public static class Cloud
	{
		private static readonly CloudOnceEvents s_cloudOnceEvents;

		private static Interval s_autoLoadInterval;

		private static bool s_isProviderInitialized;

		public static string ServiceName
		{
			get
			{
				return Provider.ServiceName;
			}
		}

		public static string PlayerID
		{
			get
			{
				return Provider.PlayerID;
			}
		}

		public static string PlayerDisplayName
		{
			get
			{
				return Provider.PlayerDisplayName;
			}
		}

		public static Texture2D PlayerImage
		{
			get
			{
				return Provider.PlayerImage;
			}
		}

		public static bool IsSignedIn
		{
			get
			{
				return Provider.IsSignedIn;
			}
		}

		public static bool CloudSaveEnabled
		{
			get
			{
				return Provider.CloudSaveEnabled;
			}
			set
			{
				Provider.CloudSaveEnabled = value;
			}
		}

		public static Interval AutoLoadInterval
		{
			get
			{
				return s_autoLoadInterval;
			}
			set
			{
				s_autoLoadInterval = value;
			}
		}

		public static GenericLeaderboardsWrapper Leaderboards { get; private set; }

		public static GenericAchievementsWrapper Achievements { get; private set; }

		public static ICloudStorageProvider Storage
		{
			get
			{
				return Provider.Storage;
			}
		}

		private static ICloudProvider Provider
		{
			get
			{
				if (!s_isProviderInitialized)
				{
					CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.InternalInit(s_cloudOnceEvents);
					s_isProviderInitialized = true;
				}
				return CloudProviderBase<GooglePlayGamesCloudProvider>.Instance;
			}
		}

		public static event UnityAction OnInitializeComplete
		{
			add
			{
				s_cloudOnceEvents.OnInitializeComplete += value;
			}
			remove
			{
				s_cloudOnceEvents.OnInitializeComplete -= value;
			}
		}

		public static event UnityAction<bool> OnSignedInChanged
		{
			add
			{
				s_cloudOnceEvents.OnSignedInChanged += value;
			}
			remove
			{
				s_cloudOnceEvents.OnSignedInChanged -= value;
			}
		}

		public static event UnityAction OnSignInFailed
		{
			add
			{
				s_cloudOnceEvents.OnSignInFailed += value;
			}
			remove
			{
				s_cloudOnceEvents.OnSignInFailed -= value;
			}
		}

		public static event UnityAction<Texture2D> OnPlayerImageDownloaded
		{
			add
			{
				s_cloudOnceEvents.OnPlayerImageDownloaded += value;
			}
			remove
			{
				s_cloudOnceEvents.OnPlayerImageDownloaded -= value;
			}
		}

		public static event UnityAction<bool> OnCloudSaveComplete
		{
			add
			{
				s_cloudOnceEvents.OnCloudSaveComplete += value;
			}
			remove
			{
				s_cloudOnceEvents.OnCloudSaveComplete -= value;
			}
		}

		public static event UnityAction<bool> OnCloudLoadComplete
		{
			add
			{
				s_cloudOnceEvents.OnCloudLoadComplete += value;
			}
			remove
			{
				s_cloudOnceEvents.OnCloudLoadComplete -= value;
			}
		}

		public static event UnityAction<string[]> OnNewCloudValues
		{
			add
			{
				s_cloudOnceEvents.OnNewCloudValues += value;
			}
			remove
			{
				s_cloudOnceEvents.OnNewCloudValues -= value;
			}
		}

		static Cloud()
		{
			s_cloudOnceEvents = new CloudOnceEvents();
			Achievements = new GenericAchievementsWrapper();
			Leaderboards = new GenericLeaderboardsWrapper();
		}

		public static void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
		{
			Provider.Initialize(activateCloudSave, autoSignIn, autoCloudLoad);
		}

		public static void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
		{
			Provider.SignIn(autoCloudLoad, callback);
		}

		public static void SignOut()
		{
			Provider.SignOut();
		}

		public static void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			Provider.LoadUsers(userIDs, callback);
		}
	}
}
