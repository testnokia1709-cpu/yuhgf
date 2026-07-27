using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUIEvents : MonoBehaviour
{
	public static LevelSelectUIEvents Instance;

	public Color VSHomeDisableColor;

	public Color TournamentEnableColor;

	public Color TournamentDisableColor;

	public Color VSLevelSelectEnableColor;

	public Color VSLevelSelectDisableColor;

	public Color VSTextEnableColor;

	public Color VSTextDisableColor;

	public Text StarsText;

	public Text MedalsText;

	public Color LevelCompleteColor;

	public Color LevelIncompleteColor;

	public Color RatedColor;

	public Color NotRatedColor;

	public Color FeaturedColor;

	public Color NotFeaturedColor;

	public ToggleButton ToggleMusic;

	public Toggle ToggleReplays;

	public BuyButton BuyButton;

	public GameObject NoticeButton;

	public GameObject NoticeAlert;

	public GameObject EveryplayButton;

	public Text VersionText;

	public Image LevelEditorLock;

	public GameObject CloudLoginButton;

	public GameObject CloudLoginTitleButton;

	public Image CloudLoginButtonIcon;

	public Image CloudLoginTitleButtonIcon;

	public Sprite GooglePlayGamesSprite;

	public Sprite GameCenterSprite;

	public GameObject GameCenterPanel;

	public Button BuyGameButton;

	public StoreButton ButtonStoreLevelSelect;

	public StoreButton ButtonStoreCommunity;

	public StoreButton ButtonStoreTitle;

	public LevelButtonGrid LevelButtonGrid;

	public LevelButtonGrid CommmunityGrid;

	public GameObject CommunityDetailPanel;

	public Button CommunityNext;

	public Button CommunityPrevious;

	public ToggleButton CommunityNewest;

	public ToggleButton CommunityTop;

	public ToggleButton CommunityTopAllTime;

	public ToggleButton CommunityUser;

	public ToggleButton CommunityUserSpecific;

	public ToggleButton CommunityFeatured;

	public ToggleButton CommunityEasy;

	public ToggleButton CommunityMedium;

	public ToggleButton CommunityHard;

	public Text CommunitySelectedDesignerName;

	public ImageBar PageBar;

	public Button LevelNext;

	public Button LevelPrevious;

	public GameObject StoreSaleTags;

	public Image ImageSale25;

	public Image ImageSale50;

	public Image ImageSale75;

	public Text TextCloudLogout;

	public Text TextBuyGameThankYou;

	public Text TextDiscount;

	public Text TextCoins;

	public Text TextGems;

	public GameObject TitlePanel;

	public GameObject LevelSelectPanel;

	public GameObject SettingsPanel;

	public GameObject InputBlockPanel;

	public GameObject AchievementsPanel;

	public GameObject StorePanel;

	public GameObject CommunityPanel;

	public GameObject LanguagePanel;

	public GameObject TitleScreenParent;

	public GameObject TitleScreenTemplate;

	public GameObject OwnerCrown;

	public GameObject ThumbnailCapture;

	public GameObject MainCamera;

	public GameObject NextGameButton;

	public GameObject Cheats;

	public bool EnableCheats;

	public Text BetaRoundText;

	public List<AchievementButton> AchievementButtonList;

	private static bool s_levelEditorUnlocked;

	private static int m_communitySelectedIndex;

	private static string m_communityAuthorFilter;

	private bool m_firstFrame;

	private List<Action> m_deferredActions = new List<Action>();

	private bool m_waitOneFrame;

	private int m_cheatCount;

	private int m_pageIndex;

	private bool m_communitySession;

	private int m_selectedCommunityLevelIndex;

	private bool m_ensureFirstAdLoaded = true;

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		S3Config instance = S3Config.Instance;
		instance.OnReceviedConfig = (Action<bool>)Delegate.Combine(instance.OnReceviedConfig, (Action<bool>)delegate(bool changed)
		{
			if (this != null && changed)
			{
				updateControls();
				customizeBuyButton(m_pageIndex);
			}
		});
		CloudOnceAPI.Instance.OnCloundSignInChanged = delegate(bool signedIn)
		{
			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				if (this != null)
				{
					setupCloudLogin(signedIn);
					updateLevelSelect();
				}
			});
		};
		StoreManager.Instance.OnInitializedComplete = delegate
		{
			updateTileButtons(m_pageIndex);
		};
	}

	private void Start()
	{
		Cheats.SetActive(EnableCheats);
		string text = "v" + TextLibrary.AppVersion;
		VersionText.text = text;
		DialogManager.CloseDialog();
		setupCloudLogin(CloudOnceAPI.Instance.IsSignedIn());
		updateLockedControls();
		Debug.Log("Loading Language...");
		if (DataStore.Instance.GameSettings.Language != string.Empty)
		{
			TextLibrary.Instance.LoadLanguage(DataStore.Instance.GameSettings.Language);
		}
		TextLibrary.Instance.SetFont(Instance.GetAllText());
		TextLibrary.Instance.SetFont(DialogManager.Instance.GetAllText());
		LocalizeText();
		Debug.Log("Update controls...");
		m_pageIndex = LevelManager.Instance.GetLevelPackIndex(DataStore.Instance.LastPlayed.Level);
		if (m_pageIndex == -1)
		{
			m_pageIndex = 0;
		}
		updateControls();
		updateTileButtons(m_pageIndex);
		m_firstFrame = true;
		TouchDrawPhysics.Instance.EnablePhysics(true);
		if (ScreenManager.MenuScreen == MenuScreen.LevelSelect)
		{
			LevelSelectClicked();
			MarketingSettings settings = DataStore.Instance.MarketingSettings;
			if (settings.HasReviewed || settings.ViewedReviewReminder >= Marketing.s_timesToRemindToReview || !settings.LastLevelSolved || settings.LastLevelHadAd || settings.LevelsCompleted < Marketing.s_completedLevelsUntilReview)
			{
				return;
			}
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_REVIEWQUESTION), TextLibrary.Get(StringId.S_REVIEW_YES), TextLibrary.Get(StringId.S_REVIEW_BUG), TextLibrary.Get(StringId.S_REVIEW_LATER), delegate(int o)
			{
				switch (o)
				{
				case 0:
					Marketing.ShowRateUs();
					settings.HasReviewed = true;
					break;
				case 1:
					Marketing.ShowFeedback();
					settings.HasReviewed = true;
					break;
				case 2:
					settings.LevelsCompleted = 0;
					break;
				}
				settings.ViewedReviewReminder++;
				m_deferredActions.Add(delegate
				{
					DataStore.Instance.MarketingSettings = settings;
					DataStore.Save();
				});
			});
		}
		else if (ScreenManager.MenuScreen == MenuScreen.Community)
		{
			if (ParseAPI.Instance.CurrentRequestFilter == LevelRequestFilter.Newest)
			{
				CommunityNewest.isOn = true;
			}
			else if (ParseAPI.Instance.CurrentRequestFilter == LevelRequestFilter.TopRated)
			{
				CommunityTop.isOn = true;
			}
			else if (ParseAPI.Instance.CurrentRequestFilter == LevelRequestFilter.User)
			{
				CommunityUser.isOn = true;
			}
			else if (ParseAPI.Instance.CurrentRequestFilter == LevelRequestFilter.TopAllTime)
			{
				CommunityTopAllTime.isOn = true;
			}
			clearHomeScreen();
			ScreenManager.Instance.ShowPanel(CommunityPanel);
			updateCommunityControls();
			DialogManager.CloseDialog();
		}
		else
		{
			showHomeScreen();
		}
	}

	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			if (DialogManager.Instance.IsShown)
			{
				DialogManager.CloseDialog();
			}
			else if (ScreenManager.Instance.CurrentPanel != TitlePanel)
			{
				ButtonHomeClicked();
			}
			else
			{
				quitApplication();
			}
		}
		if (m_firstFrame)
		{
			m_firstFrame = false;
		}
		if (m_deferredActions.Count > 0 && m_waitOneFrame)
		{
			m_deferredActions[0]();
			m_deferredActions.RemoveAt(0);
			m_waitOneFrame = false;
		}
		if (m_deferredActions.Count > 0)
		{
			m_waitOneFrame = true;
		}
	}

	public void LoadLevel(int level)
	{
		DialogManager.ShowLoadingDialog();
		m_deferredActions.Add(delegate
		{
			LevelManager.CommunityLevel = false;
			LevelManager.LoadLevel(level);
		});
	}

	public void PlayStretchAnimation(GameObject obj)
	{
		Animation component = obj.GetComponent<Animation>();
		component.Play("Stretch");
	}

	public void PlayButtonClicked(GameObject obj)
	{
		TileButton component = obj.GetComponent<TileButton>();
		int level = component.LevelNumber;
		if (LevelManager.Instance.GetLevelLocked(level))
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_ROWLOCKED), TextLibrary.Get(StringId.S_OK));
			return;
		}
		DialogManager.ShowLoadingDialog();
		m_deferredActions.Add(delegate
		{
			if (m_ensureFirstAdLoaded)
			{
				m_ensureFirstAdLoaded = false;
				AdContoller.Instance.EnsureAdsAreLoaded();
			}
			LevelManager.CommunityLevel = false;
			LevelManager.LoadLevel(level);
			DataStore.Instance.LastPlayed.Level = level;
		});
	}

	public List<Text> GetAllText()
	{
		List<Text> list = new List<Text>();
		LocalizedText[] array = (LocalizedText[])Resources.FindObjectsOfTypeAll(typeof(LocalizedText));
		LocalizedText[] array2 = array;
		foreach (LocalizedText localizedText in array2)
		{
			if (localizedText.Text != null)
			{
				list.Add(localizedText.Text);
			}
		}
		return list;
	}

	public void LocalizeText()
	{
		LocalizedText[] array = (LocalizedText[])Resources.FindObjectsOfTypeAll(typeof(LocalizedText));
		LocalizedText[] array2 = array;
		foreach (LocalizedText localizedText in array2)
		{
			if (localizedText.Text != null)
			{
				localizedText.Text.text = TextLibrary.Get(localizedText.StringId);
			}
		}
	}

	private void setupCloudLogin(bool signedIn)
	{
		if (Instance == null)
		{
			return;
		}
		Sprite sprite = null;
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			NextGameButton.SetActive(DataStore.Instance.ConfigSettings.ShowNextGameiOS);
			sprite = GameCenterSprite;
			GameCenterPanel.gameObject.SetActive(!signedIn);
			CloudLoginButton.SetActive(false);
			CloudLoginTitleButton.SetActive(false);
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			NextGameButton.SetActive(DataStore.Instance.ConfigSettings.ShowNextGameAndroid);
			sprite = GooglePlayGamesSprite;
			GameCenterPanel.gameObject.SetActive(false);
			CloudLoginButton.SetActive(true);
			if (NextGameButton.activeSelf)
			{
				CloudLoginTitleButton.SetActive(false);
			}
			else
			{
				CloudLoginTitleButton.SetActive(!signedIn);
			}
		}
		else
		{
			GameCenterPanel.gameObject.SetActive(false);
			CloudLoginButton.SetActive(false);
			CloudLoginTitleButton.SetActive(false);
			NextGameButton.SetActive(true);
		}
		CloudLoginButtonIcon.sprite = sprite;
		CloudLoginTitleButtonIcon.sprite = sprite;
	}

	private void allowInput(bool allowed)
	{
		if (!allowed)
		{
			InputBlockPanel.SetActive(true);
		}
		else
		{
			InputBlockPanel.SetActive(false);
		}
	}

	private void clearHomeScreen()
	{
		TouchDrawPhysics.Instance.ClearShapes();
		TouchDrawRecorder.Instance.Stop();
		TitleScreenTemplate.SetActive(false);
		foreach (Transform item in TitleScreenParent.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}

	private void showHomeScreen()
	{
		clearHomeScreen();
		TitleScreenTemplate.SetActive(true);
		ScreenManager.Instance.ShowPanel(TitlePanel);
		TouchDrawRecorder.Instance.Playback();
	}

	public void ButtonHomeClicked()
	{
		showHomeScreen();
		updateControls();
	}

	public void SettingsClicked()
	{
		clearHomeScreen();
		ScreenManager.Instance.ShowPanel(SettingsPanel);
		updateControls();
	}

	public void LanguageClicked()
	{
		ScreenManager.Instance.ShowPanel(LanguagePanel);
		updateControls();
	}

	public void LanguageSelectClicked(string language)
	{
		ChangeLanguage(language);
		SettingsClicked();
		m_deferredActions.Add(delegate
		{
			DataStore.Instance.GameSettings.Language = language;
			DataStore.Save();
		});
	}

	public void ChangeLanguage(string language)
	{
		TextLibrary.Instance.LoadLanguage(language);
		TextLibrary.Instance.SetFont(GetAllText());
		TextLibrary.Instance.SetFont(DialogManager.Instance.GetAllText());
		LocalizeText();
	}

	public void LanguageBlankClicked()
	{
		DialogManager.ShowDialog("Do you have time to help translate Brain It On! into another language? Please contact me at support@brainitongame.com, thank you.", TextLibrary.Get(StringId.S_CLOSE));
	}

	public void AchievementsClicked()
	{
		if (CloudOnceAPI.Instance.IsSignedIn())
		{
			CloudOnceAPI.Instance.AchievementOverlay();
			return;
		}
		AchievementManager.Instance.CalculateAchievements();
		AchievementManager.Instance.UpdateControls();
		ScreenManager.Instance.ShowPanel(AchievementsPanel);
	}

	public void LeaderboardClicked()
	{
		if (CloudOnceAPI.Instance.IsSignedIn())
		{
			CloudOnceAPI.Instance.LeaderboardOverlay();
			return;
		}
		CloudOnceAPI.Instance.SignIn(delegate(bool signedIn)
		{
			if (signedIn)
			{
				CloudOnceAPI.Instance.LeaderboardOverlay();
			}
		});
	}

	public void StoreClicked()
	{
		clearHomeScreen();
		ScreenManager.Instance.ShowPanel(StorePanel);
		updateStoreControls();
		updateControls();
	}

	public void BackClicked()
	{
		if (ScreenManager.Instance.PreviousPanel == TitlePanel || ScreenManager.Instance.CurrentPanel == CommunityPanel)
		{
			showHomeScreen();
		}
		else if (ScreenManager.Instance.PreviousPanel == CommunityPanel && ScreenManager.Instance.CurrentPanel == StorePanel)
		{
			ScreenManager.Instance.ShowPanel(CommunityPanel);
		}
		else
		{
			ScreenManager.Instance.ShowPanel(LevelSelectPanel);
			updateLevelSelect();
		}
		updateControls();
	}

	public void LevelSelectClicked()
	{
		clearHomeScreen();
		ScreenManager.Instance.ShowPanel(LevelSelectPanel);
		updateLevelSelect();
		updateTileButtons(m_pageIndex);
		updateControls();
	}

	public void EveryplayClicked()
	{
		EveryplayManager.Instance.ShowCommunity();
	}

	public void ResetDataClicked(GameObject obj)
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_RESETDATA_QUESTION), TextLibrary.Get(StringId.S_RESETDATA_YES), TextLibrary.Get(StringId.S_CANCEL), delegate(int o)
		{
			if (o == 0)
			{
				DataStore.Reset();
				DataStore.Save();
				CloudOnceAPI.Instance.CloudSave(true);
				updateControls();
				updateTileButtons(m_pageIndex);
			}
		});
	}

	public void ClearCommunityImagesClicked()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_RESETDATA_QUESTION), TextLibrary.Get(StringId.S_RESETDATA_YES), TextLibrary.Get(StringId.S_CANCEL), delegate(int o)
		{
			if (o == 0)
			{
				DialogManager.ShowLoadingDialog();
				m_deferredActions.Add(delegate
				{
					HashSet<string> allLevelKeysAsSet = LevelManager.GetAllLevelKeysAsSet();
					foreach (KeyValuePair<string, int> item in DataStore.Instance.FriendsScore)
					{
						allLevelKeysAsSet.Add(FacebookSettings.s_imagePrefix + item.Key);
					}
					DataFile.ClearAllImages(TimeSpan.FromDays(14.0), allLevelKeysAsSet);
					updateCommunityControls();
					DialogManager.CloseDialog();
				});
			}
		});
	}

	public void SolveAllClicked()
	{
		LevelManager.Instance.SetAllLevelsComplete();
	}

	public void ClearLocksClicked()
	{
		LevelManager.Instance.UnlockLevels();
	}

	public void LockAllLevelsClicked()
	{
		LevelManager.Instance.LockLevels();
		DataStore.Save();
	}

	public void ShowNoticeClicked()
	{
		ConfigSettings configSettings = DataStore.Instance.ConfigSettings;
		if (configSettings.IsNoticeAvailable())
		{
			string body = configSettings.NoticeContents;
			if (Application.platform == RuntimePlatform.Android)
			{
				body = configSettings.NoticeContentsAndroid;
			}
			DialogManager.ShowNotice(body, TextLibrary.Get(StringId.S_NOTICEBOARD), TextLibrary.Get(StringId.S_CLOSE), delegate
			{
				DataStore.Instance.ConfigSettings.NoticeReadID = DataStore.Instance.ConfigSettings.GetNoticeID();
				DataStore.Save();
				updateControls();
			});
		}
		else
		{
			DialogManager.ShowNotice(TextLibrary.Get(StringId.S_NOTICEBOARD_NONOTICES), TextLibrary.Get(StringId.S_NOTICEBOARD), TextLibrary.Get(StringId.S_CLOSE));
		}
	}

	public void ToggleMusicChanged(GameObject obj)
	{
		Toggle componentInChildren = obj.GetComponentInChildren<Toggle>();
		if (componentInChildren != null)
		{
			DataStore.Instance.GameSettings.MusicOn = componentInChildren.isOn;
			DataStore.Save();
		}
	}

	public void ToggleReplaysChanged(GameObject obj)
	{
		Toggle componentInChildren = obj.GetComponentInChildren<Toggle>();
		if (componentInChildren != null)
		{
			componentInChildren.graphic.gameObject.SetActive(componentInChildren.isOn);
		}
	}

	public void SendLogEmailClicked()
	{
		EmailSender.Send("support@brainitongame.com", "Brain It On! Error Log", LogManager.Instance.GetLogHistory());
	}

	public void NewGameClicked()
	{
		Marketing.ShowNewGame();
	}

	public void MoreGamesClicked()
	{
		Marketing.ShowMoreGames();
	}

	public void RateUsClicked()
	{
		Marketing.ShowRateUs();
		DataStore.Instance.MarketingSettings.HasReviewed = true;
		DataStore.Save();
	}

	public void FeedbackClicked()
	{
		Marketing.ShowFeedback();
	}

	public void WebsiteClicked()
	{
		Marketing.ShowWebsite();
	}

	public void FacebookPageClicked()
	{
		Marketing.ShowFacebookPage();
	}

	public void TwitterPageClicked()
	{
		Marketing.ShowTwitterPage();
	}

	public void PreviousClicked()
	{
		m_pageIndex = Math.Max(m_pageIndex - 1, 0);
		PageBar.SelectedMask = 1 << m_pageIndex;
		updateTileButtons(m_pageIndex);
	}

	public void NextClicked()
	{
		m_pageIndex = Math.Min(m_pageIndex + 1, PageBar.ImageList.Count - 1);
		PageBar.SelectedMask = 1 << m_pageIndex;
		updateTileButtons(m_pageIndex);
	}

	public void PageBarClicked()
	{
		m_pageIndex = PageBar.IndexClicked;
		PageBar.SelectedMask = 1 << m_pageIndex;
		updateTileButtons(m_pageIndex);
	}

	public void LevelEditorClicked()
	{
		if (!StoreManager.Instance.IsGameOwned() && !s_levelEditorUnlocked)
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_LEVELEDITOR_NOTAVAILABLE), TextLibrary.Get(StringId.S_CANCEL), TextLibrary.Get(StringId.S_BUYNOW), delegate(int o)
			{
				if (o == 1)
				{
					PurchaseGameClicked();
				}
			});
		}
		else
		{
			DialogManager.ShowLoadingDialog();
			m_deferredActions.Add(delegate
			{
				SceneManager.LoadSceneAsync("LevelEditor");
			});
		}
	}

	public void CheatClicked()
	{
		m_cheatCount++;
		if (m_cheatCount >= 5)
		{
			s_levelEditorUnlocked = true;
			LevelEditorLock.gameObject.SetActive(false);
			m_cheatCount = 0;
		}
	}

	public void CreditsClicked()
	{
		DialogManager.ShowNotice(TextLibrary.Credits, TextLibrary.Get(StringId.S_CREDITS), TextLibrary.Get(StringId.S_CLOSE));
	}

	public void CloudLoginClicked()
	{
		if (!CloudOnceAPI.Instance.IsSignedIn())
		{
			CloudOnceAPI.Instance.SignIn(delegate
			{
			});
		}
	}

	public void CloudConnectClicked()
	{
		if (CloudOnceAPI.Instance.IsSignedIn())
		{
			CloudOnceAPI.Instance.SignOut();
			TextCloudLogout.text = TextLibrary.Get(StringId.S_LOGIN);
		}
		else
		{
			CloudOnceAPI.Instance.SignIn(delegate(bool signedIn)
			{
				TextCloudLogout.text = TextLibrary.Get((!signedIn) ? StringId.S_LOGIN : StringId.S_LOGOUT);
			});
		}
	}

	public void PurchaseNoAds()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_CONNECTING_STORE));
		m_deferredActions.Add(delegate
		{
			StoreManager.Instance.Purchase(PurchasableItem.NO_ADS, OnPurchaseCompleted);
		});
	}

	public void PurchaseGameClicked()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_CONNECTING_STORE));
		int num = 0;
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.NO_ADS))
		{
			num++;
		}
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_2))
		{
			num++;
		}
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_3))
		{
			num++;
		}
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_4))
		{
			num++;
		}
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_5))
		{
			num++;
		}
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_6))
		{
			num++;
		}
		if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_7))
		{
			num++;
		}
		PurchasableItem item = PurchasableItem.FULL_GAME_100;
		if (num == 1)
		{
			item = PurchasableItem.FULL_GAME_25;
		}
		else if (num == 2)
		{
			item = PurchasableItem.FULL_GAME_50;
		}
		else if (num >= 3)
		{
			item = PurchasableItem.FULL_GAME_75;
		}
		switch (DataStore.Instance.ConfigSettings.Sale)
		{
		case 1:
			item = PurchasableItem.FULL_GAME_25;
			break;
		case 2:
			item = PurchasableItem.FULL_GAME_50;
			break;
		case 3:
			item = PurchasableItem.FULL_GAME_75;
			break;
		}
		StoreManager.Instance.Purchase(item, OnPurchaseCompleted);
	}

	public void GiftPackClicked()
	{
		int totalStarsCount = LevelManager.Instance.GetTotalStarsCount();
		int starsForGift = LevelManager.Instance.GetStarsForGift(m_pageIndex);
		PurchasableItem item = LevelManager.GetPackItem(m_pageIndex);
		if (item == PurchasableItem.INVALID)
		{
			Debug.LogError("Invalid purchase selected: " + m_pageIndex);
		}
		else if (totalStarsCount >= starsForGift)
		{
			Debug.Log("Earned enough stars, gifting pack.");
			DialogManager.ShowDialog(string.Format(TextLibrary.Get(StringId.S_ITEM_UNLOCKED), StoreManager.Instance.GetProductName(item)), TextLibrary.Get(StringId.S_ITEM_UNLOCKED_OK), delegate
			{
			});
			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				StoreManager.Instance.Gift(item);
				LevelManager.Instance.UnlockFirstRowOfPack(item);
				DataStore.Save();
				CloudOnceAPI.Instance.CloudSave();
				updateTileButtons(m_pageIndex);
			});
		}
		else
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_NOTENOUGHSTARS), TextLibrary.Get(StringId.S_OK), delegate
			{
			});
		}
	}

	public void PurchasePack()
	{
		PurchasableItem packItem = LevelManager.GetPackItem(m_pageIndex);
		if (packItem == PurchasableItem.INVALID)
		{
			Debug.LogError("Invalid purchase selected: " + m_pageIndex);
			return;
		}
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_CONNECTING_STORE));
		StoreManager.Instance.Purchase(packItem, OnPurchaseCompleted);
	}

	public void OnPurchaseCompleted(bool result, bool cancelled, PurchasableItem item, int failCode)
	{
		if (result)
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_PURCHASE_COMPLETE), TextLibrary.Get(StringId.S_OK));
			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				LevelManager.Instance.UnlockAllLevelsInPack(item);
				updateStoreControls();
				updateLevelSelect();
				updateTileButtons(m_pageIndex);
				updateLockedControls();
				DataStore.Save();
			});
		}
		else if (!cancelled)
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_STORE_ERROR) + " (" + failCode + ")", TextLibrary.Get(StringId.S_OK));
		}
		else
		{
			Debug.Log("Purchase cancelled.");
			DialogManager.CloseDialog();
		}
	}

	public void ComingSoonClicked()
	{
		DialogManager.ShowDialog("These levels are still under construction. They will be available soon.", TextLibrary.Get(StringId.S_OK), delegate
		{
		});
	}

	public void TryLevelClicked()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_TRY_GETAHINT), TextLibrary.Get(StringId.S_YES), TextLibrary.Get(StringId.S_NO), delegate(int hintChoice)
		{
			if (hintChoice == 0 && !AdContoller.Instance.ShowRewardedVideo(delegate(AdResult result)
			{
				switch (result)
				{
				case AdResult.Failed:
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_WATCHVIDEO_FAILED), TextLibrary.Get(StringId.S_OK), delegate
					{
					});
					break;
				case AdResult.Skipped:
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_TRY_WATCHVIDEO_SKIPPED), TextLibrary.Get(StringId.S_OK), delegate
					{
					});
					break;
				case AdResult.Completed:
					DialogManager.ShowLoadingDialog();
					m_deferredActions.Add(delegate
					{
						LevelManager.CommunityLevel = false;
						int startLevelFromPack = LevelManager.GetStartLevelFromPack(m_pageIndex);
						int level = UnityEngine.Random.Range(startLevelFromPack, startLevelFromPack + LevelManager.PackLevelCount / 2 - 1);
						LevelManager.LoadLevel(level, true);
						DataStore.Instance.LastPlayed.Level = level;
						Debug.Log("IsTryLevel: " + LevelManager.IsTryLevel);
					});
					break;
				}
			}))
			{
				DialogManager.ShowDialog(TextLibrary.Get(StringId.S_WATCHVIDEO_FAILED), TextLibrary.Get(StringId.S_OK), delegate
				{
				});
			}
		});
	}

	public bool IsPackPremium(int pageIndex)
	{
		PurchasableItem packItem = LevelManager.GetPackItem(pageIndex);
		if (packItem == PurchasableItem.FULL_GAME_100)
		{
			return true;
		}
		return false;
	}

	public bool IsPackOwned(int pageIndex)
	{
		if (StoreManager.Instance.IsGameOwned())
		{
			return true;
		}
		LevelPack pack = LevelManager.GetPack(pageIndex);
		if (pack.Item == PurchasableItem.INVALID || pack.Free)
		{
			return true;
		}
		return StoreManager.Instance.CheckIfOwned(pack.Item);
	}

	public void RestorePurchasesClicked()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_CONNECTING_STORE));
		m_deferredActions.Add(delegate
		{
			StoreManager.Instance.RestorePurchases(delegate(bool result)
			{
				if (result)
				{
					unlockLevelsWithPurchases();
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_PURCHASES_RESTORED), TextLibrary.Get(StringId.S_OK));
				}
				else
				{
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_STORE_ERROR), TextLibrary.Get(StringId.S_OK));
				}
			});
		});
	}

	public void ClearPurchasesClicked()
	{
		DataStore.Instance.Purchases.Clear();
		DataStore.Instance.FreeItems.Clear();
		DataStore.Save();
		CloudOnceAPI.Instance.CloudSave(true);
	}

	public void UnlockPurchasesClicked()
	{
		StoreManager.Instance.Gift(PurchasableItem.PACK_2);
		StoreManager.Instance.Gift(PurchasableItem.PACK_3);
		StoreManager.Instance.Gift(PurchasableItem.PACK_4);
		StoreManager.Instance.Gift(PurchasableItem.PACK_5);
		StoreManager.Instance.Gift(PurchasableItem.PACK_6);
		StoreManager.Instance.Gift(PurchasableItem.PACK_7);
		StoreManager.Instance.Gift(PurchasableItem.PACK_8);
		StoreManager.Instance.Gift(PurchasableItem.PACK_9);
		StoreManager.Instance.Gift(PurchasableItem.PACK_10);
		StoreManager.Instance.Gift(PurchasableItem.PACK_11);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_2);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_3);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_4);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_5);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_6);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_7);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_8);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_9);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_10);
		LevelManager.Instance.UnlockFirstRowOfPack(PurchasableItem.PACK_11);
		DataStore.Save();
	}

	public void UnlockFullGameClicked()
	{
		StoreManager.Instance.Gift(PurchasableItem.FULL_GAME_100);
		LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.FULL_GAME_100);
		DataStore.Save();
	}

	private void unlockLevelsWithPurchases()
	{
		Debug.Log("Checking for levels that have been purchased");
		if (StoreManager.Instance.IsGameOwned())
		{
			LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.FULL_GAME_100);
		}
		else
		{
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_2))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_2);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_3))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_3);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_4))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_4);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_5))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_5);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_6))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_6);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_7))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_7);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_8))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_8);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_9))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_9);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_10))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_10);
			}
			if (StoreManager.Instance.CheckIfPurchased(PurchasableItem.PACK_11))
			{
				LevelManager.Instance.UnlockAllLevelsInPack(PurchasableItem.PACK_11);
			}
		}
		m_deferredActions.Add(delegate
		{
			DataStore.Save();
		});
	}

	private void updateTileButtons(int pageIndex)
	{
		LevelPrevious.gameObject.SetActive(pageIndex != 0);
		LevelNext.gameObject.SetActive(pageIndex != LevelManager.Instance.PackCount - 1);
		int num = 1 + pageIndex * LevelButtonGrid.TileCount;
		TileButton tileButton = null;
		foreach (GameObject tile in LevelButtonGrid.Tiles)
		{
			tileButton = tile.GetComponent<TileButton>();
			customizeTileButton(tileButton, num++);
		}
		customizeBuyButton(pageIndex);
		if (tileButton != null)
		{
			LevelManager.ThumbnailSize = new Vector2(Mathf.Floor(tileButton.ScreenshotImage.rectTransform.rect.width) * 2f, Mathf.Floor(tileButton.ScreenshotImage.rectTransform.rect.height) * 2f);
		}
	}

	private void customizeTileButton(TileButton tButton, int index)
	{
		bool flag = !IsPackOwned(m_pageIndex);
		tButton.LevelNumber = index;
		LevelCompletion levelCompletion = LevelManager.Instance.GetLevelCompletion(index);
		bool flag2 = (levelCompletion & LevelCompletion.Complete) == LevelCompletion.Complete;
		bool levelLocked = LevelManager.Instance.GetLevelLocked(index);
		tButton.LockImage.enabled = levelLocked;
		tButton.ScreenshotImage.texture = null;
		tButton.Button.onClick.AddListener(delegate
		{
			PlayButtonClicked(tButton.gameObject);
		});
		string levelKey = LevelManager.Instance.GetLevelKey(index);
		bool active = false;
		if (!DataStore.Instance.LevelImageVersion.ContainsKey(levelKey) && LevelManager.Instance.IsLevelNew(index))
		{
			active = true;
		}
		tButton.NewTag.SetActive(active);
		int num = (int)tButton.ScreenshotImage.rectTransform.rect.width;
		int num2 = (int)tButton.ScreenshotImage.rectTransform.rect.height;
		Texture2D texture2D = null;
		texture2D = ((!LevelManager.ThumbnailCache.ContainsKey(levelKey)) ? DataFile.LoadImage(levelKey) : LevelManager.ThumbnailCache[levelKey]);
		if (!LevelManager.ThumbnailCache.ContainsKey(levelKey))
		{
			LevelManager.ThumbnailCache.Add(levelKey, texture2D);
		}
		else
		{
			LevelManager.ThumbnailCache[levelKey] = texture2D;
		}
		if (texture2D != null)
		{
			tButton.ScreenshotImage.texture = texture2D;
			tButton.ScreenshotImage.color = Color.white;
			tButton.LevelNumberText.color = Color.white;
			tButton.LevelNumberTextOutline.enabled = true;
		}
		else
		{
			tButton.ScreenshotImage.color = Color.clear;
			tButton.LevelNumberText.color = Color.black;
			tButton.LevelNumberTextOutline.enabled = false;
		}
		ImageBar starBar = tButton.StarBar;
		starBar.SelectedMask = (int)levelCompletion;
		Button button = tButton.Button;
		button.interactable = !flag;
		tButton.Cover.SetActive(levelLocked);
		tButton.LevelNumberText.text = index.ToString();
		tButton.ButtonImage.color = ((!flag2) ? LevelIncompleteColor : LevelCompleteColor);
	}

	private void customizeBuyButton(int index)
	{
		bool flag = IsPackOwned(index);
		bool packAvailable = LevelManager.Instance.GetPackAvailable(index);
		bool active = !flag || !packAvailable;
		int starsForGift = LevelManager.Instance.GetStarsForGift(index);
		if (flag && packAvailable)
		{
			LevelManager.Instance.UnlockFirstRowOfPack(LevelManager.GetPackItem(index));
		}
		BuyButton.gameObject.SetActive(active);
		BuyButton.BuyNowButton.gameObject.SetActive(packAvailable);
		BuyButton.UnlockButton.gameObject.SetActive(packAvailable);
		BuyButton.ComingSoonButton.gameObject.SetActive(!packAvailable);
		BuyButton.MoreLevelsButton.gameObject.SetActive(!packAvailable);
		BuyButton.TryButton.gameObject.SetActive(false);
		BuyButton.StarsText.text = starsForGift.ToString();
		BuyButton.BuyText.Text.text = TextLibrary.Get(BuyButton.BuyText.StringId);
		BuyButton.UnlockText.Text.text = TextLibrary.Get(BuyButton.UnlockText.StringId);
		if (!packAvailable)
		{
			BuyButton.ComingSoonText.Text.text = TextLibrary.Get(BuyButton.ComingSoonText.StringId);
		}
		else if (IsPackPremium(index))
		{
			BuyButton.UnlockButton.gameObject.SetActive(false);
			BuyButton.TryButton.gameObject.SetActive(true);
		}
	}

	private void updateLevelSelect()
	{
		int num = AchievementManager.Instance.CalculateAchievements();
		MedalsText.text = num.ToString();
		StarsText.text = LevelManager.Instance.GetTotalStarsCount().ToString();
	}

	public void CommunityClicked()
	{
		if (!m_communitySession)
		{
			AnalyticsManager.LogGameEvent("Commmunity", "Started");
			m_communitySession = true;
		}
		if (!ParseAPI.Instance.GetCachedLevels())
		{
			DialogManager.ShowLoadingDialog();
		}
		ParseAPI.Instance.RequestCommunityData(null, delegate(bool success)
		{
			if (success)
			{
				m_deferredActions.Add(delegate
				{
					updateCommunityControls(0, delegate
					{
						clearHomeScreen();
						ScreenManager.Instance.ShowPanel(CommunityPanel);
						DialogManager.CloseDialog();
					});
				});
			}
			else
			{
				m_deferredActions.Add(delegate
				{
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_SERVERERROR), TextLibrary.Get(StringId.S_OK));
				});
			}
		});
	}

	public void CommunityGetNewLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.Newest);
	}

	public void CommunityGetFeaturedLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.Featured);
	}

	public void CommunityGetTopLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.TopRated);
	}

	public void CommunityGetTopLevelsOfAllTimeClicked()
	{
		getCommunityLevels(LevelRequestFilter.TopAllTime);
	}

	public void CommunityGetUserLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.User);
	}

	public void CommunityGetUserSpecificLevelsClicked()
	{
		CommunityLevel communityLevel = CommunityManager.Levels[m_selectedCommunityLevelIndex];
		if (m_communityAuthorFilter != communityLevel.Author)
		{
			m_communityAuthorFilter = communityLevel.Author;
			ParseAPI.Instance.ClearCachedLevels(LevelRequestFilter.UserSpecific);
		}
		getCommunityLevels(LevelRequestFilter.UserSpecific, m_communityAuthorFilter);
	}

	public void CommunityRefreshUserSpecificLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.UserSpecific);
	}

	public void CommunityGetEasyLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.Easy);
	}

	public void CommunityGetMediumLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.Medium);
	}

	public void CommunityGetHardLevelsClicked()
	{
		getCommunityLevels(LevelRequestFilter.Hard);
	}

	private void getCommunityLevels(LevelRequestFilter filterType, string filter = null)
	{
		LevelRequestFilter oldFilter = ParseAPI.Instance.CurrentRequestFilter;
		ParseAPI.Instance.CurrentRequestFilter = filterType;
		if (!ParseAPI.Instance.GetCachedLevels())
		{
			DialogManager.ShowLoadingDialog();
		}
		ParseAPI.Instance.RequestCommunityData(filter, delegate(bool success)
		{
			m_deferredActions.Add(delegate
			{
				if (success)
				{
					DialogManager.CloseDialog();
				}
				else
				{
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_SERVERERROR), TextLibrary.Get(StringId.S_OK));
					ParseAPI.Instance.CurrentRequestFilter = oldFilter;
				}
				m_communitySelectedIndex = 0;
				updateCommunityControls();
			});
		});
	}

	public void CommunityPageClicked(int pageDelta)
	{
		updateCommunityControls(pageDelta, delegate
		{
		});
	}

	private int updateCommunityIndex(int index, int pageSize, int pageDelta, int maxIndex)
	{
		index += pageDelta * pageSize;
		if (index < 0)
		{
			index = 0;
		}
		else if (index > maxIndex)
		{
			index = maxIndex / pageSize * pageSize;
		}
		return index;
	}

	private void showCommunityDetailPanel(int index)
	{
		CommunityDetailPanel.SetActive(true);
		CommunityLevel level = CommunityManager.Levels[index];
		CommunityItemTemplate item = CommunityDetailPanel.GetComponentInChildren<CommunityItemTemplate>();
		item.TitleText.text = ((!string.IsNullOrEmpty(level.Title)) ? level.Title : "Unknown");
		item.AuthorText.text = level.Author;
		item.ThumbnailImage.texture = DataFile.LoadImage(level.ObjectId);
		item.PlayButton.onClick.RemoveAllListeners();
		item.PlayButton.onClick.AddListener(delegate
		{
			DialogManager.ShowLoadingDialog();
			LevelManager.CommunityLevel = true;
			SceneManager.LoadScene("CommunityGame");
			CommunityManager.LoadCurrentLevel(index);
		});
		item.StarBar.SelectedMask = (int)CommunityManager.GetLevelCompletion(level.ObjectId);
		bool flag = ParseAPI.Instance.IsCommunityLevelLiked(level.ObjectId);
		item.RatingImage.color = ((!flag) ? NotRatedColor : RatedColor);
		float num = ((level.AttemptCount <= 0) ? 0f : ((float)level.SolveCount / (float)level.AttemptCount));
		float num2 = ((level.AttemptCount <= 0) ? 0f : ((float)level.ThreeStarCount / (float)level.AttemptCount));
		int num3 = level.LikeCount + (ParseAPI.Instance.IsCommunityLevelLikedThisSession(level.ObjectId) ? 1 : 0);
		item.SolveRateText.text = ((!(num > 0f)) ? "-" : string.Format("{0}{1}", Mathf.CeilToInt(num * 100f), "%"));
		item.ClearRateText.text = ((!(num > 0f)) ? "-" : string.Format("{0}{1}", Mathf.CeilToInt(num2 * 100f), "%"));
		item.RatingText.gameObject.SetActive(num3 > 0);
		item.RatingText.text = num3.ToString();
		string localLevelKey = level.ObjectId;
		item.RatingButton.onClick.RemoveAllListeners();
		item.RatingButton.onClick.AddListener(delegate
		{
			if (!ParseAPI.Instance.IsCommunityLevelLiked(localLevelKey))
			{
				level.LikeCount++;
				ParseAPI.Instance.PostCommunityLike(localLevelKey, true);
				item.RatingImage.color = RatedColor;
				item.RatingText.gameObject.SetActive(true);
				item.RatingText.text = level.LikeCount.ToString();
			}
		});
		bool isOwner = level.Author == CloudOnceAPI.Instance.PlayerDisplayName;
		item.DeleteButton.gameObject.SetActive(isOwner);
		item.DeleteButton.onClick.RemoveAllListeners();
		item.DeleteButton.onClick.AddListener(delegate
		{
			if (isOwner)
			{
				DialogManager.ShowDialog(TextLibrary.Get(StringId.S_PROMPT_DELETE), TextLibrary.Get(StringId.S_YES), TextLibrary.Get(StringId.S_NO), delegate(int o)
				{
					if (o == 0)
					{
						ParseAPI.Instance.DeleteCommunityLevel(localLevelKey, delegate(bool success)
						{
							m_deferredActions.Add(delegate
							{
								if (success)
								{
									DialogManager.ShowDialog(TextLibrary.Get(StringId.S_DELETED), TextLibrary.Get(StringId.S_OK), delegate
									{
										CloseCommunityDetailPanel();
										updateCommunityControls();
									});
								}
								else
								{
									DialogManager.ShowDialog(TextLibrary.Get(StringId.S_SERVERERROR), TextLibrary.Get(StringId.S_OK), delegate
									{
										CloseCommunityDetailPanel();
										updateCommunityControls();
									});
								}
							});
						});
					}
				});
			}
		});
		bool flag2 = CloudOnceAPI.Instance.PlayerDisplayName == "OrbitalNineGames";
		item.FeatureButton.gameObject.SetActive(flag2);
		item.FeatureImage.color = ((level.Featured != 1) ? NotFeaturedColor : FeaturedColor);
		item.FeatureButton.onClick.RemoveAllListeners();
		if (!flag2)
		{
			return;
		}
		item.FeatureButton.onClick.AddListener(delegate
		{
			bool feature = level.Featured == 0;
			ParseAPI.Instance.FeatureCommunityLevel(localLevelKey, feature ? 1 : 0, delegate(bool success)
			{
				m_deferredActions.Add(delegate
				{
					if (success)
					{
						DialogManager.ShowDialog("Level " + ((!feature) ? "un-" : string.Empty) + "featured successfully.", TextLibrary.Get(StringId.S_OK), delegate
						{
							CloseCommunityDetailPanel();
							updateCommunityControls();
						});
					}
					else
					{
						DialogManager.ShowDialog(TextLibrary.Get(StringId.S_SERVERERROR), TextLibrary.Get(StringId.S_OK), delegate
						{
							CloseCommunityDetailPanel();
							updateCommunityControls();
						});
					}
				});
			});
		});
	}

	public void CloseCommunityDetailPanel()
	{
		CommunityDetailPanel.SetActive(false);
	}

	private void updateCommunityControls(int pageDelta = 0, Action OnComplete = null)
	{
		List<CommunityLevel> levels = CommunityManager.Levels;
		CommunityUserSpecific.gameObject.SetActive(m_communityAuthorFilter != null);
		CommunitySelectedDesignerName.gameObject.SetActive(ParseAPI.Instance.CurrentRequestFilter == LevelRequestFilter.UserSpecific);
		CommunitySelectedDesignerName.text = m_communityAuthorFilter;
		switch (ParseAPI.Instance.CurrentRequestFilter)
		{
		case LevelRequestFilter.Newest:
			CommunityNewest.isOn = true;
			break;
		case LevelRequestFilter.TopRated:
			CommunityTop.isOn = true;
			break;
		case LevelRequestFilter.TopAllTime:
			CommunityTopAllTime.isOn = true;
			break;
		case LevelRequestFilter.User:
			CommunityUser.isOn = true;
			break;
		case LevelRequestFilter.UserSpecific:
			CommunityUserSpecific.isOn = true;
			break;
		case LevelRequestFilter.Featured:
			CommunityFeatured.isOn = true;
			break;
		case LevelRequestFilter.Easy:
			CommunityEasy.isOn = true;
			break;
		case LevelRequestFilter.Medium:
			CommunityMedium.isOn = true;
			break;
		case LevelRequestFilter.Hard:
			CommunityHard.isOn = true;
			break;
		}
		int tileCount = CommmunityGrid.Tiles.Count;
		m_communitySelectedIndex = updateCommunityIndex(m_communitySelectedIndex, tileCount, pageDelta, levels.Count);
		CommunityPrevious.gameObject.SetActive(m_communitySelectedIndex != 0);
		CommunityNext.gameObject.SetActive(m_communitySelectedIndex + tileCount < levels.Count - 1);
		CommunityDetailPanel.SetActive(false);
		TextCoins.text = string.Format("{0}", DataStore.Instance.CoinCount);
		TextGems.text = string.Format("{0}", DataStore.Instance.GemCount);
		int requiredThumbnailCount = getRequiredThumbnailCount(m_communitySelectedIndex, tileCount);
		if (requiredThumbnailCount > 0)
		{
			DialogManager.ShowLoadingDialog();
		}
		Action action = delegate
		{
			captureCommunityThumbnails(m_communitySelectedIndex, tileCount);
			int num = m_communitySelectedIndex;
			foreach (GameObject tile in CommmunityGrid.Tiles)
			{
				TileButton component = tile.GetComponent<TileButton>();
				if (num >= levels.Count())
				{
					component.gameObject.SetActive(false);
					component.ScreenshotImage.texture = null;
					component.ScreenshotImage.color = Color.white;
				}
				else
				{
					string objectId = levels[num].ObjectId;
					if (objectId == null)
					{
						component.gameObject.SetActive(false);
					}
					else
					{
						LevelCompletion levelCompletion = CommunityManager.GetLevelCompletion(objectId);
						component.gameObject.SetActive(true);
						component.LevelNumberText.enabled = false;
						component.LockImage.enabled = false;
						component.NewTag.SetActive(false);
						component.ScreenshotImage.texture = DataFile.LoadImage(objectId);
						component.ScreenshotImage.color = Color.white;
						component.StarBar.gameObject.SetActive(levelCompletion != LevelCompletion.Unsolved);
						component.StarBar.SelectedMask = (int)levelCompletion;
						component.Button.onClick.RemoveAllListeners();
						int localIndex = num;
						component.Button.onClick.AddListener(delegate
						{
							m_selectedCommunityLevelIndex = localIndex;
							showCommunityDetailPanel(localIndex);
						});
					}
					num++;
				}
			}
			DialogManager.CloseDialog();
			if (OnComplete != null)
			{
				OnComplete();
			}
		};
		if (OnComplete != null)
		{
			m_deferredActions.Add(action);
		}
		else
		{
			action();
		}
	}

	private int getRequiredThumbnailCount(int pageIndex, int pageSize)
	{
		int num = 0;
		for (int i = pageIndex; i < pageIndex + pageSize; i++)
		{
			if (i < CommunityManager.Levels.Count)
			{
				CommunityLevel communityLevel = CommunityManager.Levels[i];
				if (!DataFile.ExistsImage(communityLevel.ObjectId))
				{
					num++;
				}
			}
		}
		return num;
	}

	private void captureCommunityThumbnails(int pageIndex, int pageSize)
	{
		float num = Screen.height / 4;
		float x = num;
		CommunityManager.ThumbnailSize = new Vector2(x, num);
		MainCamera.SetActive(false);
		ThumbnailCapture.SetActive(true);
		for (int i = pageIndex; i < pageIndex + pageSize; i++)
		{
			if (i >= CommunityManager.Levels.Count)
			{
				continue;
			}
			CommunityLevel communityLevel = CommunityManager.Levels[i];
			if (!DataFile.ExistsImage(communityLevel.ObjectId))
			{
				TouchDrawLevel touchDrawLevel = CommunityManager.LoadLevel(communityLevel.Data);
				if (touchDrawLevel != null)
				{
					TouchDrawLevel.RestoreLevel(touchDrawLevel, ThumbnailCapture.transform.position);
					captureThumbnail(CommunityManager.ThumbnailSize, communityLevel.ObjectId);
					TouchDrawLevel.ClearObjects(touchDrawLevel);
				}
			}
		}
		TouchDrawPhysics.Instance.SetShapeMaterial(LevelPenType.Normal);
		ThumbnailCapture.SetActive(false);
		MainCamera.SetActive(true);
	}

	private void captureThumbnail(Vector2 size, string key)
	{
		Screenshot.Instance.DestinationImage = null;
		Screenshot.Instance.EnableObjects.Clear();
		Screenshot.Instance.ImageSize = size;
		Screenshot.Instance.Screenshot_Sync((int)size.x, (int)size.y);
		DataFile.SaveImage(key, Screenshot.Instance.TextureData);
	}

	private void updateLockedControls()
	{
		bool flag = StoreManager.Instance.IsGameOwned();
		LevelEditorLock.gameObject.SetActive(!flag && !s_levelEditorUnlocked);
		OwnerCrown.SetActive(flag);
	}

	private void updateControls()
	{
		TextCloudLogout.text = ((!CloudOnceAPI.Instance.IsSignedIn()) ? TextLibrary.Get(StringId.S_LOGIN) : TextLibrary.Get(StringId.S_LOGOUT));
		ButtonStoreLevelSelect.SetVisible(!StoreManager.Instance.IsGameOwned());
		ButtonStoreCommunity.SetVisible(!StoreManager.Instance.IsGameOwned());
		ButtonStoreTitle.SetVisible(!StoreManager.Instance.IsGameOwned());
		NoticeAlert.SetActive(DataStore.Instance.ConfigSettings.IsNoticeAvailable() && DataStore.Instance.ConfigSettings.GetNoticeID() > DataStore.Instance.ConfigSettings.NoticeReadID);
		ToggleMusic.SetToggle(DataStore.Instance.GameSettings.MusicOn);
		ToggleReplays.graphic.gameObject.SetActive(ToggleReplays.isOn);
		PageBar.SelectedMask = 1 << m_pageIndex;
		bool flag = !StoreManager.Instance.IsGameOwned() && DataStore.Instance.ConfigSettings.Sale != 0;
		ButtonStoreLevelSelect.SetSale(flag);
		ButtonStoreCommunity.SetSale(flag);
		ButtonStoreTitle.SetSale(flag);
		StoreSaleTags.SetActive(flag);
		ImageSale25.gameObject.SetActive(DataStore.Instance.ConfigSettings.Sale == 1);
		ImageSale50.gameObject.SetActive(DataStore.Instance.ConfigSettings.Sale == 2);
		ImageSale75.gameObject.SetActive(DataStore.Instance.ConfigSettings.Sale == 3);
	}

	private void updateStoreControls()
	{
		if (StoreManager.Instance.IsGameOwned())
		{
			TextBuyGameThankYou.gameObject.SetActive(true);
			TextDiscount.gameObject.SetActive(false);
			BuyGameButton.gameObject.SetActive(false);
		}
		else
		{
			TextBuyGameThankYou.gameObject.SetActive(false);
			TextDiscount.gameObject.SetActive(StoreManager.Instance.HasPurchaseBeenMade());
			BuyGameButton.gameObject.SetActive(true);
		}
	}

	private void quitApplication()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_QUIT_CONFIRMATION), TextLibrary.Get(StringId.S_YES), TextLibrary.Get(StringId.S_NO), delegate(int c)
		{
			if (c == 0)
			{
				Application.Quit();
			}
		});
	}
}
