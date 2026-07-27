using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CloudOnce.Internal.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.OurUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

namespace CloudOnce.Internal.Providers
{
	public sealed class GooglePlayGamesCloudProvider : CloudProviderBase<GooglePlayGamesCloudProvider>
	{
		private const string c_guestPreferenceKey = "GooglePlayWantsToUseGuest";

		private CloudOnceEvents cloudOnceEvents;

		private bool cloudSaveEnabled = true;

		private Texture2D playerImage;

		private bool initializing;

		public static bool DebugLogEnabled
		{
			get
			{
				return GooglePlayGames.OurUtils.Logger.DebugLogEnabled;
			}
			set
			{
				GooglePlayGames.OurUtils.Logger.DebugLogEnabled = value;
			}
		}

		public static bool IsGuestUserDefault { get; private set; }

		public override string PlayerID
		{
			get
			{
				return (!IsGpgsInitialized) ? string.Empty : PlayGamesPlatform.Instance.localUser.id;
			}
		}

		public override string PlayerDisplayName
		{
			get
			{
				return (!IsGpgsInitialized) ? string.Empty : PlayGamesPlatform.Instance.localUser.userName;
			}
		}

		public override Texture2D PlayerImage
		{
			get
			{
				return (!IsGpgsInitialized) ? Texture2D.whiteTexture : (playerImage ?? Texture2D.whiteTexture);
			}
		}

		public override bool IsSignedIn
		{
			get
			{
				return IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated();
			}
		}

		public bool CloudSaveInitialized { get; private set; }

		public override bool CloudSaveEnabled
		{
			get
			{
				return cloudSaveEnabled;
			}
			set
			{
				if (!CloudSaveInitialized)
				{
					Debug.LogWarning("Cloud Save has not been initialized. Call Cloud.Initialize before attempting to set CloudSaveEnabled.");
				}
				else
				{
					cloudSaveEnabled = value;
				}
			}
		}

		public bool IsGpgsInitialized { get; private set; }

		public override ICloudStorageProvider Storage { get; protected set; }

		public override void Initialize(bool activateCloudSave = true, bool autoSignIn = true, bool autoCloudLoad = true)
		{
			if (initializing)
			{
				return;
			}
			initializing = true;
			cloudSaveEnabled = activateCloudSave;
			PlayGamesClientConfiguration.Builder builder = new PlayGamesClientConfiguration.Builder();
			if (activateCloudSave)
			{
				builder.EnableSavedGames();
				CloudSaveInitialized = true;
			}
			PlayGamesPlatform.InitializeInstance(builder.Build());
			SubscribeOnAuthenticatedEvent();
			PlayGamesPlatform.DebugLogEnabled = false;
			Debug.Log("PlayGamesPlatform debug logs disabled.");
			IsGpgsInitialized = true;
			if (!IsGuestUserDefault && autoSignIn)
			{
				UnityAction<bool> callback = delegate
				{
					cloudOnceEvents.RaiseOnInitializeComplete();
					initializing = false;
				};
				SignIn(autoCloudLoad, callback);
				return;
			}
			if (IsGuestUserDefault && autoSignIn)
			{
				GooglePlayGames.OurUtils.Logger.d("Guest user mode active, ignoring auto sign-in. Please call SignIn directly.");
			}
			if (autoCloudLoad)
			{
				cloudOnceEvents.RaiseOnCloudLoadComplete(false);
			}
			cloudOnceEvents.RaiseOnInitializeComplete();
			initializing = false;
		}

		public override void SignIn(bool autoCloudLoad = true, UnityAction<bool> callback = null)
		{
			if (!IsGpgsInitialized)
			{
				Debug.LogWarning("SignIn called, but Google Play Game Services has not been initialized. Ignoring call.");
				CloudOnceUtils.SafeInvoke(callback, false);
				return;
			}
			if (autoCloudLoad)
			{
				SetUpAutoCloudLoad();
			}
			IsGuestUserDefault = false;
			GooglePlayGames.OurUtils.Logger.d("Attempting to sign in to Google Play Game Services.");
			PlayGamesPlatform.Instance.Authenticate(delegate(bool success)
			{
				if (!success)
				{
					GooglePlayGames.OurUtils.Logger.w("Failed to sign in to Google Play Game Services.");
					bool flag;
					try
					{
						flag = InternetConnectionUtils.GetConnectionStatus() != InternetConnectionStatus.Connected;
					}
					catch (NotSupportedException)
					{
						flag = Application.internetReachability == NetworkReachability.NotReachable;
					}
					if (flag)
					{
						GooglePlayGames.OurUtils.Logger.d("Failure seems to be due to lack of Internet. Will try to connect again next time.");
					}
					else
					{
						GooglePlayGames.OurUtils.Logger.d("Must assume the failure is due to player opting out of the sign-in process, setting guest user as default");
						IsGuestUserDefault = true;
					}
					cloudOnceEvents.RaiseOnSignInFailed();
					if (autoCloudLoad)
					{
						cloudOnceEvents.RaiseOnCloudLoadComplete(false);
					}
				}
				CloudOnceUtils.SafeInvoke(callback, success);
			});
		}

		public override void SignOut()
		{
			GooglePlayGames.OurUtils.Logger.d("Signing out of Google Play Game Services.");
			PlayGamesPlatform.Instance.SignOut();
			ActivateGuestUserMode();
		}

		public override void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback)
		{
			if (!IsGpgsInitialized)
			{
				Debug.LogWarning("LoadUsers called, but Google Play Game Services has not been initialized. Ignoring call.");
				CloudOnceUtils.SafeInvoke(callback, new IUserProfile[0]);
			}
			else
			{
				PlayGamesPlatform.Instance.LoadUsers(userIDs, callback);
			}
		}

		public void InternalInit(CloudOnceEvents events)
		{
			cloudOnceEvents = events;
			Storage = new GooglePlayGamesCloudSaveWrapper(events);
			base.ServiceName = "Google Play Game Services";
		}

		public void ActivateGuestUserMode()
		{
			IsGuestUserDefault = true;
			cloudOnceEvents.RaiseOnSignedInChanged(false);
		}

		protected override void OnAwake()
		{
			IsGuestUserDefault = PlayerPrefs.GetInt("GooglePlayWantsToUseGuest", 0) == 1;
		}

		protected override void OnOnDestroy()
		{
			PlayerPrefs.SetInt("GooglePlayWantsToUseGuest", IsGuestUserDefault ? 1 : 0);
		}

		private static void UpdateAchievementsData(IAchievement[] achievements)
		{
			Type typeFromHandle = typeof(Achievements);
			Dictionary<string, UnifiedAchievement> dictionary = new Dictionary<string, UnifiedAchievement>();
			PropertyInfo[] properties = typeFromHandle.GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.PropertyType == typeof(UnifiedAchievement))
				{
					dictionary[propertyInfo.Name] = (UnifiedAchievement)propertyInfo.GetValue(null, null);
				}
			}
			foreach (IAchievement achievement in achievements)
			{
				try
				{
					dictionary[dictionary.Single((KeyValuePair<string, UnifiedAchievement> pair) => pair.Value.ID == achievement.id).Key].UpdateData(achievement.completed, achievement.percentCompleted, achievement.hidden);
				}
				catch
				{
				}
			}
		}

		private void SetUpAutoCloudLoad()
		{
			GooglePlayGamesCloudSaveWrapper googlePlayGamesCloudSaveWrapper = (GooglePlayGamesCloudSaveWrapper)Storage;
			googlePlayGamesCloudSaveWrapper.SubscribeToAuthenticationEvent();
		}

		private void SubscribeOnAuthenticatedEvent()
		{
			PlayGamesPlatform.Instance.OnAuthenticated -= OnAuthenticated;
			PlayGamesPlatform.Instance.OnAuthenticated += OnAuthenticated;
		}

		private void OnAuthenticated()
		{
			PlayGamesHelperObject.RunOnGameThread(delegate
			{
				cloudOnceEvents.RaiseOnSignedInChanged(true);
				GooglePlayGames.OurUtils.Logger.d("Successfully signed in to Google Play Game Services.");
				IsGuestUserDefault = false;
				GetPlayerImage();
				PlayGamesPlatform.Instance.LoadAchievements(UpdateAchievementsData);
			});
		}

		private void GetPlayerImage()
		{
			string userImageUrl = PlayGamesPlatform.Instance.GetUserImageUrl();
			if (!string.IsNullOrEmpty(userImageUrl))
			{
				StartCoroutine(DownloadPlayerImage(userImageUrl));
			}
		}

		private IEnumerator DownloadPlayerImage(string url)
		{
			WWW www = new WWW(url);
			yield return www;
			playerImage = www.texture;
			cloudOnceEvents.RaiseOnPlayerImageDownloaded(playerImage);
		}
	}
}
