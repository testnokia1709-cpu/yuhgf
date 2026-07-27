using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : UIBase
{
	public static UIManager Instance;

	public Button ButtonRetry;

	public Button ButtonCompleteRetry;

	public Button ButtonReplay;

	public GameObject ButtonExit;

	public Button ButtonNext;

	public Button ButtonClose;

	public Button ButtonRecord;

	public Image RecordStatusImage;

	public Button ButtonShare;

	public Button ButtonNoAds;

	public GameObject ButtonHint;

	public GameObject ButtonCloseHint;

	public GameObject GamePanel;

	public GameObject GameCompletePanel;

	public GameObject AdPanel;

	public GameObject PhysicsLevel;

	public GameObject GoalSummaryPanel;

	public GameObject CompleteButtonPanel;

	public GameObject LeftPanel;

	public GameObject RightPanel;

	public GameObject KickerCanvas;

	public GameObject CornerRibbon;

	public Image ImagePenType;

	public List<Sprite> LevelTextures;

	public RawImage ScreenshotImage;

	public Text LevelNumberText;

	public Text LevelSolveCountText;

	public Text CompleteText;

	public Text GoalText;

	public Text HintText;

	public Text SolvedTimeText;

	public Text SolvedShapeText;

	public Text TimeGoalText;

	public Image TimeGoalIcon;

	public Text ShapeGoalText;

	public Image ShapeGoalIcon;

	public Text SummaryTimeGoalText;

	public Image SummaryTimeGoalIcon;

	public Text SummaryShapeGoalText;

	public Image SummaryShapeGoalIcon;

	public ObjectTimer TimeGoalTimer;

	public GameObject RecordPanel;

	public LocalizedText SolvedText;

	public Text RecordShapeText;

	public Text RecordTimeText;

	public Image StarTime;

	public Image StarShapes;

	public Sprite TwitterSprite;

	public Sprite FacebookSprite;

	public Animator CameraFlash;

	public Color SuccessfulColor;

	public Color FailureColor;

	public Color RatedColor;

	public Image RateIcon;

	public Text CommunityLevelTitle;

	public Text CommunityLevelAuthor;

	public Text HeartCountText;

	public Text CoinEarnedText;

	public Text GemEarnedText;

	public Camera LevelCamera;

	public Camera UICamera;

	public GameObject ButtonMenu;

	public GameObject RetryButton;

	public bool HintsAvailable;

	private float m_duration;

	private int m_shapeCount;

	private List<Action> m_deferredActions;

	private int m_deferredDelay;

	private int m_levelSolvedCount;

	private static Color s_GoalTextColorOver = new Color(1f, 1f, 1f, 0.35f);

	private static string s_screenshotFilename = "screenshot.png";

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		m_deferredActions = new List<Action>();
		SummaryTimeGoalText.text = "0.0123456789";
		SummaryTimeGoalText.text = "0.0";
	}

	public void OnDisable()
	{
		GameStateManager instance = GameStateManager.Instance;
		instance.OnSetupComplete = (Action)Delegate.Remove(instance.OnSetupComplete, new Action(OnSetupComplete));
	}

	private void Start()
	{
		GameStateManager.Instance.UserInterface = this;
		GameStateManager instance = GameStateManager.Instance;
		instance.OnSetupComplete = (Action)Delegate.Combine(instance.OnSetupComplete, new Action(OnSetupComplete));
		GameStateManager.Instance.OnPlaying = OnPlaying;
		GameStateManager.Instance.OnSolved = OnSolved;
		GameStateManager.Instance.OnShowResults = OnShowResults;
		GameStateManager.Instance.OnReplayAvailable = OnReplayAvailable;
		EveryplayManager.Instance.OnReady = OnEveryplayReady;
		EveryplayManager.Instance.OnRecordStarted = OnEveryplayRecordStarted;
		EveryplayManager.Instance.OnRecordStopped = OnEveryplayRecordStopped;
		EveryplayManager.Instance.OnInitialized = OnEveryplayInitialized;
		TouchDrawPhysics.Instance.OnDrawShape = OnDrawShape;
		TouchDrawPhysics.Instance.CanDraw = CanDraw;
		TextLibrary.Instance.SetFont(getAllText());
		LocalizeText();
		DialogManager.CloseDialog();
		UICamera.transform.position = LevelCamera.transform.position;
		if (!LevelManager.CommunityLevel)
		{
			LevelNumberText.text = LevelManager.Level.ToString();
		}
		if (Demo.Instance != null)
		{
			ButtonExit.SetActive(false);
			ButtonClose.interactable = false;
			ButtonNext.interactable = false;
		}
		GameCompletePanel.SetActive(false);
		GoalText.gameObject.SetActive(false);
		HintText.gameObject.SetActive(false);
		KickerCanvas.SetActive(false);
		GamePanel.SetActive(false);
		CountdownController.Instance.ClearText();
		ButtonHint.SetActive(HintsAvailable && DataStore.Instance.ConfigSettings.HintsEnabled);
		SummaryShapeGoalText.text = LevelManager.Instance.GetShapeGoal().ToString();
		SummaryTimeGoalText.text = string.Format("{0:0.0}", LevelManager.Instance.GetTimeGoal());
		string levelKey = LevelManager.Instance.GetLevelKey();
		if (LevelManager.CommunityLevel || !DataStore.Instance.LevelsMinShapeCount.ContainsKey(levelKey) || !DataStore.Instance.LevelsMinTime.ContainsKey(levelKey))
		{
			RecordPanel.SetActive(false);
		}
		else
		{
			RecordShapeText.text = DataStore.Instance.LevelsMinShapeCount[levelKey].ToString();
			RecordTimeText.text = DataStore.Instance.LevelsMinTime[levelKey].ToString("0.0");
		}
		UpdateRecordControls();
		ButtonReplay.gameObject.SetActive(false);
		ButtonShare.gameObject.SetActive(true);
		if (Demo.Instance != null && Demo.Instance.ShowWelcome)
		{
			TouchDrawPhysics.Instance.TouchEnabled = false;
			DialogManager.ShowDialog("Welcome to Brain It On!\n(Deceptively challenging puzzles for your brain)\n\nWarning: You are skipping a bunch of easy levels so this may be frustrating. Please hang in there. \n\nInstructions: Click and drag the mouse to create shapes, use the shapes to solve the puzzle.", "Got it, let's go!", delegate
			{
				TouchDrawPhysics.Instance.TouchEnabled = true;
			});
			Demo.Instance.ShowWelcome = false;
		}
		GameStateManager.Instance.SetState(GameState.Setup);
	}

	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			if (ButtonCloseHint.activeSelf)
			{
				CloseHintClicked();
			}
			else
			{
				GameStateManager.Instance.SetState(GameState.Cleanup);
			}
		}
		if (m_deferredActions.Count > 0 && m_deferredDelay == 0)
		{
			m_deferredActions[0]();
			m_deferredActions.RemoveAt(0);
		}
		if (m_deferredDelay > 0)
		{
			m_deferredDelay--;
		}
	}

	public void ChangeLanguage(string language)
	{
		TextLibrary.Instance.LoadLanguage(language);
		TextLibrary.Instance.SetFont(getAllText());
		TextLibrary.Instance.SetFont(DialogManager.Instance.GetAllText());
		LocalizeText();
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
		SetGoalText(LevelManager.Instance.GetLevelHint());
		LevelSolveCountText.text = string.Format(TextLibrary.Get(StringId.S_SOLVEDTIMES), m_levelSolvedCount.ToString("#,#", CultureInfo.InvariantCulture));
	}

	private void OnDrawShape(float mass)
	{
	}

	private bool CanDraw()
	{
		return true;
	}

	private void OnEveryplayReady()
	{
		m_deferredActions.Add(delegate
		{
			UpdateRecordControls();
		});
	}

	private void OnEveryplayInitialized()
	{
	}

	private void OnEveryplayRecordStarted()
	{
		m_deferredActions.Add(delegate
		{
			UpdateRecordControls();
		});
	}

	private void OnEveryplayRecordStopped()
	{
		m_deferredActions.Add(delegate
		{
			UpdateRecordControls();
		});
	}

	private void OnSetupComplete()
	{
		if (LevelManager.CommunityLevel)
		{
			int solveCount = CommunityManager.CurrentLevelStats.SolveCount;
			if (solveCount > 0)
			{
				LevelSolveCountText.text = string.Format(TextLibrary.Get(StringId.S_SOLVEDTIMES), solveCount.ToString("#,#", CultureInfo.InvariantCulture));
			}
			else
			{
				LevelSolveCountText.text = string.Empty;
			}
			return;
		}
		string levelKey = LevelManager.Instance.GetLevelKey();
		if (DataStore.Instance.ConfigSettings.ShowSolveCount)
		{
			StatsManager.Instance.GetLevelSolveCount(levelKey, delegate(int result)
			{
				m_deferredActions.Add(delegate
				{
					m_levelSolvedCount = result;
					if (result > 0)
					{
						LevelSolveCountText.text = string.Format(TextLibrary.Get(StringId.S_SOLVEDTIMES), m_levelSolvedCount.ToString("#,#", CultureInfo.InvariantCulture));
					}
					else
					{
						LevelSolveCountText.text = string.Empty;
					}
				});
			});
		}
		else
		{
			LevelSolveCountText.text = string.Empty;
		}
		int num = -1;
		if (DataStore.Instance.LevelImageVersion.ContainsKey(levelKey))
		{
			num = DataStore.Instance.LevelImageVersion[levelKey];
		}
		int headVersion = LevelManager.Instance.GetLevelVersion();
		if (!LevelManager.IsTryLevel && (num == -1 || num != headVersion))
		{
			Screenshot.Instance.DestinationImage = null;
			Screenshot.Instance.EnableObjects.Clear();
			Screenshot.Instance.ImageSize = new Vector2((int)LevelManager.ThumbnailSize.x, (int)LevelManager.ThumbnailSize.y);
			Screenshot.Instance.Capture((int)LevelManager.ThumbnailSize.x, (int)LevelManager.ThumbnailSize.y, true, null, delegate
			{
				GamePanel.SetActive(true);
				GoalText.gameObject.SetActive(true);
				DataFile.SaveImage(levelKey, Screenshot.Instance.TextureData);
				if (!DataStore.Instance.LevelImageVersion.ContainsKey(levelKey))
				{
					DataStore.Instance.LevelImageVersion.Add(levelKey, headVersion);
				}
				else
				{
					DataStore.Instance.LevelImageVersion[levelKey] = headVersion;
				}
				LevelManager.ThumbnailCache.Remove(levelKey);
				m_deferredActions.Add(delegate
				{
					DataStore.Save();
				});
			});
		}
		else
		{
			GamePanel.SetActive(true);
			GoalText.gameObject.SetActive(true);
		}
	}

	private void OnPlaying()
	{
		GoalText.gameObject.SetActive(false);
		TouchDrawPhysics.Instance.EnablePhysics(true);
	}

	private void OnSolved(float time, int shapes)
	{
		m_deferredActions.Add(delegate
		{
			CameraFlash.SetTrigger("Flash");
			AudioManager.Instance.PlayEffect(AudioLibrary.Instance.EffectScreenshot);
			EnableTouchDraw(false);
		});
	}

	private void OnShowResults()
	{
		PhysicsLevel.SetActive(false);
	}

	private void OnReplayAvailable()
	{
		ButtonReplay.gameObject.SetActive(true);
	}

	public void OnPurchaseCompleted(bool result, bool cancelled, PurchasableItem item, int failCode)
	{
		if (!(Instance != null))
		{
			return;
		}
		if (result)
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_PURCHASE_COMPLETE), TextLibrary.Get(StringId.S_OK));
			if (item == PurchasableItem.NO_ADS)
			{
				ShowNoAdsControl(false);
			}
			DataStore.Save();
		}
		else if (!cancelled)
		{
			DialogManager.ShowDialog(TextLibrary.Get(StringId.S_STORE_ERROR) + " (" + failCode + ")", TextLibrary.Get(StringId.S_OK));
		}
		else
		{
			DialogManager.CloseDialog();
		}
	}

	private List<Text> getAllText()
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
		list.Add(HintText);
		list.Add(LevelNumberText);
		list.Add(LevelSolveCountText);
		list.Add(CompleteText);
		list.Add(GoalText);
		list.Add(SolvedTimeText);
		list.Add(SolvedShapeText);
		list.Add(TimeGoalText);
		list.Add(ShapeGoalText);
		list.Add(SummaryTimeGoalText);
		list.Add(SummaryShapeGoalText);
		list.Add(RecordShapeText);
		list.Add(RecordTimeText);
		return list;
	}

	public void CaptureScreenshot()
	{
		if (LevelManager.IsTryLevel)
		{
			Screenshot.Instance.DestinationImage = ScreenshotImage;
			Screenshot.Instance.EnableObjects.Add(CornerRibbon);
			Screenshot.Instance.Capture(Screen.width, Screen.height, true, null, delegate
			{
				GameStateManager.Instance.SetState(GameState.ShowResults);
			});
			return;
		}
		m_deferredActions.Add(delegate
		{
			Screenshot.Instance.DestinationImage = ScreenshotImage;
			Screenshot.Instance.EnableObjects.Add(CornerRibbon);
			Screenshot.Instance.Capture(Screen.width, Screen.height, true, null, delegate
			{
				string levelKey = LevelManager.Instance.GetLevelKey();
				CaptureThumbnail(levelKey, delegate
				{
					if (!LevelManager.CommunityLevel)
					{
						int levelVersion = LevelManager.Instance.GetLevelVersion();
						if (!DataStore.Instance.LevelImageVersion.ContainsKey(levelKey))
						{
							DataStore.Instance.LevelImageVersion.Add(levelKey, levelVersion);
						}
						else
						{
							DataStore.Instance.LevelImageVersion[levelKey] = levelVersion;
						}
						LevelManager.ThumbnailCache.Remove(levelKey);
					}
					GameStateManager.Instance.SetState(GameState.ShowResults);
				});
			});
		});
	}

	public void CaptureThumbnail(string key, Action onComplete)
	{
		int num = (int)((!LevelManager.CommunityLevel) ? LevelManager.ThumbnailSize.x : CommunityManager.ThumbnailSize.x);
		int num2 = (int)((!LevelManager.CommunityLevel) ? LevelManager.ThumbnailSize.y : CommunityManager.ThumbnailSize.y);
		Screenshot.Instance.DestinationImage = null;
		Screenshot.Instance.EnableObjects.Clear();
		Screenshot.Instance.ImageSize = new Vector2(num, num2);
		Screenshot.Instance.Capture(num, num2, true, null, delegate
		{
			DataFile.SaveImage(key, Screenshot.Instance.TextureData);
			onComplete();
		});
	}

	public void RetryClicked()
	{
		GameStateManager.Instance.SetState(GameState.Retry);
	}

	public void SolvedRetryClicked()
	{
		GameStateManager.Instance.SetState(GameState.Retry);
	}

	public void MenuClicked()
	{
		GameStateManager.Instance.SetState(GameState.Cleanup);
	}

	public void LevelSelectClicked()
	{
		GameStateManager.Instance.SetState(GameState.Cleanup);
	}

	public void NextClicked()
	{
		DialogManager.ShowLoadingDialog();
		if (!LevelManager.Instance.LoadNextLevel(false))
		{
			if (LevelManager.CommunityLevel)
			{
				ScreenManager.MenuScreen = MenuScreen.Community;
			}
			else
			{
				ScreenManager.MenuScreen = MenuScreen.LevelSelect;
			}
			SceneManager.LoadScene("LevelSelect");
		}
		else
		{
			DialogManager.CloseDialog();
			GameStateManager.Instance.SetState(GameState.Retry);
		}
	}

	public void ShowReplayClicked()
	{
		m_deferredActions.Add(delegate
		{
			if (EveryplayManager.Instance != null)
			{
				if (DataStore.Instance.ConfigSettings.ShowShareInsteadOfReplay)
				{
					EveryplayManager.Instance.ShowShare();
				}
				else
				{
					EveryplayManager.Instance.ShowReplay();
				}
			}
		});
	}

	public void HintClicked()
	{
		if (EveryplayManager.Instance.IsRecording)
		{
			EveryplayManager.Instance.StopRecording();
		}
		EnableTouchDraw(false);
		if (HintController.wasShown() || StoreManager.Instance.IsGameOwned())
		{
			showHint();
			return;
		}
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_GETAHINT), TextLibrary.Get(StringId.S_YES), TextLibrary.Get(StringId.S_NO), delegate(int hintChoice)
		{
			if (hintChoice == 0)
			{
				ShowAdPanel(true);
				if (!ShowRewardedVideo(HintVideoFailedCallback, HintVideoSkippedCallback, HintVideoCompletedCallback, HintVideoSucceedCallback, HintVideoCloseCallback))
				{
					DialogManager.ShowDialog(TextLibrary.Get(StringId.S_WATCHVIDEO_FAILED), TextLibrary.Get(StringId.S_OK), delegate
					{
						m_deferredActions.Add(delegate
						{
							ShowAdPanel(false);
							EnableTouchDraw(true);
						});
					});
				}
			}
			else
			{
				m_deferredActions.Add(delegate
				{
					EnableTouchDraw(true);
				});
			}
		});
	}

	private void HintVideoFailedCallback()
	{
		Debug.Log("HintVideoFailedCallback");
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_WATCHVIDEO_FAILED), TextLibrary.Get(StringId.S_OK), delegate
		{
			m_deferredActions.Add(delegate
			{
				ShowAdPanel(false);
				EnableTouchDraw(true);
			});
		});
	}

	private void HintVideoSkippedCallback()
	{
		Debug.Log("HintVideoSkippedCallback");
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_WATCHVIDEO_SKIPPED), TextLibrary.Get(StringId.S_OK), delegate
		{
			ShowAdPanel(false);
			EnableTouchDraw(true);
		});
	}

	private void HintVideoCompletedCallback()
	{
		Debug.Log("HintVideoCompletedCallback");
	}

	private void HintVideoSucceedCallback()
	{
		Debug.Log("HintVideoSucceedCallback");
		showHint();
	}

	private void HintVideoCloseCallback()
	{
		ShowAdPanel(false);
	}

	private void showHint()
	{
		if (HintController.Instance != null)
		{
			HintText.gameObject.SetActive(true);
			GoalText.gameObject.SetActive(false);
			LevelSolveCountText.gameObject.SetActive(false);
			ButtonHint.SetActive(false);
			LeftPanel.SetActive(false);
			RightPanel.SetActive(false);
			TouchDrawPhysics.Instance.ShapesParent.SetActive(false);
			ButtonCloseHint.SetActive(true);
			HintController.Show();
		}
		else
		{
			Debug.LogError("No hint found!");
		}
		ShowAdPanel(false);
	}

	public void CloseHintClicked()
	{
		HintText.gameObject.SetActive(false);
		GoalText.gameObject.SetActive(GameStateManager.Instance.State != GameState.Playing);
		LevelSolveCountText.gameObject.SetActive(true);
		HintController.Hide();
		ButtonHint.SetActive(true);
		LeftPanel.SetActive(true);
		RightPanel.SetActive(true);
		TouchDrawPhysics.Instance.ShapesParent.SetActive(true);
		ButtonCloseHint.SetActive(false);
		m_deferredActions.Add(delegate
		{
			EnableTouchDraw(true);
		});
	}

	public void RecordVideoClicked()
	{
		if (!EveryplayManager.Instance.IsRecording)
		{
			m_deferredActions.Add(delegate
			{
				EveryplayManager.Instance.StartRecording();
			});
		}
		else if (Time.realtimeSinceStartup - EveryplayManager.Instance.RecordStartTime > 1f)
		{
			EveryplayManager.Instance.StopRecording();
		}
	}

	public void UpdateRecordControls()
	{
		if (!EveryplayManager.Instance.IsSupported)
		{
			if (ButtonRecord != null)
			{
				ButtonRecord.gameObject.SetActive(false);
			}
			return;
		}
		if (ButtonRecord != null)
		{
			ButtonRecord.interactable = EveryplayManager.Instance.IsReady;
		}
		if (EveryplayManager.Instance.IsRecording)
		{
			if (RecordStatusImage != null)
			{
				RecordStatusImage.color = Color.red;
			}
			Debug.Log("UpdateRecordControls: Red");
		}
		else
		{
			if (RecordStatusImage != null)
			{
				RecordStatusImage.color = Color.white;
			}
			Debug.Log("UpdateRecordControls: White");
		}
	}

	private string getSolutionText()
	{
		string text = string.Format(TextLibrary.Get(StringId.S_SOCIAL_MESSAGE), LevelManager.Level);
		if (LevelManager.CommunityLevel)
		{
			text = "I solved this level in #BrainItOn";
		}
		bool flag = LevelManager.Instance.IsTimeSolved(m_duration);
		bool flag2 = LevelManager.Instance.IsShapesSolved(m_shapeCount);
		if (flag && flag2)
		{
			text = text + " " + TextLibrary.Get(StringId.S_SOCIAL_STARS);
		}
		return text;
	}

	public void ShareClicked()
	{
		m_deferredDelay = 1;
		m_deferredActions.Add(delegate
		{
			Texture2D texture = Screenshot.Capture(Screen.width, Screen.height, true);
			Screenshot.SaveImage(Application.persistentDataPath + "/" + s_screenshotFilename, texture);
			m_deferredDelay = 5;
			m_deferredActions.Add(delegate
			{
				string filePath = Application.persistentDataPath + "/" + s_screenshotFilename;
				string solutionText = getSolutionText();
				NativeShare.Share(solutionText, filePath, null, Marketing.GameName, "image/png", true, string.Empty);
			});
		});
	}

	public void RateLevelClicked()
	{
		RateIcon.color = RatedColor;
		string currentLevelId = CommunityManager.CurrentLevelId;
		if (!ParseAPI.Instance.IsCommunityLevelLiked(currentLevelId))
		{
			ParseAPI.Instance.PostCommunityLike(currentLevelId, true);
			HeartCountText.text = string.Format("{0}", CommunityManager.CurrentLevelStats.LikeCount + 1);
		}
	}

	public void EnableTouchDraw(bool enable)
	{
		if (enable && GameStateManager.Instance.State == GameState.WaitForPlayerStart)
		{
			TouchDrawPhysics.Instance.TouchEnabled = enable;
			TouchDrawPhysics.Instance.EnablePhysics(LevelManager.Instance.IsStartActive());
		}
		else
		{
			TouchDrawPhysics.Instance.EnablePhysics(enable);
			TouchDrawPhysics.Instance.TouchEnabled = enable;
		}
		if (!enable)
		{
			CountdownController.Instance.PauseCountdown();
			GameStateManager.Instance.PauseTimer();
		}
		else
		{
			CountdownController.Instance.ResumeCountdown();
			GameStateManager.Instance.ResumeTimer();
		}
	}

	public override void OnSetup()
	{
		CountdownController.Instance.StopCountdown();
		GameCompletePanel.SetActive(false);
		GamePanel.SetActive(true);
		PhysicsLevel.SetActive(true);
		setupPenType();
		ButtonRetry.interactable = true;
		SetGoalText(LevelManager.Instance.GetLevelHint());
		GoalText.gameObject.SetActive(true);
	}

	public override void UpdateUI(float duration, int shapes)
	{
		float timeGoal = LevelManager.Instance.GetTimeGoal();
		int shapeGoal = LevelManager.Instance.GetShapeGoal();
		float num = Mathf.Max(timeGoal - duration, 0f);
		bool flag = duration > timeGoal;
		bool flag2 = shapes > shapeGoal;
		SummaryTimeGoalText.color = ((!flag) ? Color.white : s_GoalTextColorOver);
		SummaryTimeGoalIcon.color = ((!flag) ? Color.white : s_GoalTextColorOver);
		SummaryShapeGoalText.color = ((!flag2) ? Color.white : s_GoalTextColorOver);
		SummaryShapeGoalIcon.color = ((!flag2) ? Color.white : s_GoalTextColorOver);
		SummaryTimeGoalText.text = ((!flag) ? string.Format("{0:0.0}", num) : "0.0");
		SummaryShapeGoalText.text = string.Format("{0}/{1}", shapes, shapeGoal);
		TimeGoalTimer.SetProgress(Mathf.Max(num / timeGoal, 0f));
	}

	public void SetGoalText(string text)
	{
		GoalText.text = text;
	}

	public override void ShowGameComplete(LevelCompletion previouslyCompleted, LevelCompletion completed, float gameDuration, int shapeCount, int coinsEarned, int gemsEarned)
	{
		m_duration = gameDuration;
		m_shapeCount = shapeCount;
		GamePanel.SetActive(false);
		CompleteText.text = TextLibrary.GetCompleteMessage();
		((RectTransform)GameCompletePanel.transform).anchoredPosition = Vector2.zero;
		GameCompletePanel.SetActive(true);
		Color color = new Color(1f, 1f, 1f, 0.35f);
		bool flag = LevelManager.Instance.IsTimeSolved(gameDuration);
		bool flag2 = LevelManager.Instance.IsShapesSolved(shapeCount);
		StarTime.gameObject.SetActive(flag);
		StarShapes.gameObject.SetActive(flag2);
		StarTime.color = ((flag || (previouslyCompleted & LevelCompletion.TimeSolved) != LevelCompletion.TimeSolved) ? Color.white : color);
		StarShapes.color = ((flag2 || (previouslyCompleted & LevelCompletion.ShapeSolved) != LevelCompletion.ShapeSolved) ? Color.white : color);
		SolvedText.Text.color = SuccessfulColor;
		SolvedTimeText.text = string.Format("{0:0.0}", m_duration);
		SolvedShapeText.text = string.Format("{0}", shapeCount);
		SolvedTimeText.color = ((!flag) ? FailureColor : SuccessfulColor);
		SolvedShapeText.color = ((!flag2) ? FailureColor : SuccessfulColor);
		TimeGoalText.text = string.Format("{0:0.0}", LevelManager.Instance.GetTimeGoal());
		ShapeGoalText.text = string.Format("{0}", LevelManager.Instance.GetShapeGoal());
		TimeGoalIcon.color = ((!flag) ? color : Color.white);
		TimeGoalText.color = ((!flag) ? color : Color.white);
		ShapeGoalIcon.color = ((!flag2) ? color : Color.white);
		ShapeGoalText.color = ((!flag2) ? color : Color.white);
		ButtonNoAds.gameObject.SetActive(false);
		Debug.Log("IsTryLevel: " + LevelManager.IsTryLevel);
		bool isTryLevel = LevelManager.IsTryLevel;
		ButtonCompleteRetry.gameObject.SetActive(!isTryLevel);
		ButtonNext.gameObject.SetActive(!isTryLevel);
		if (LevelManager.CommunityLevel)
		{
			string currentLevelId = CommunityManager.CurrentLevelId;
			int num = CommunityManager.CurrentLevelStats.LikeCount;
			if (ParseAPI.Instance.IsCommunityLevelLiked(currentLevelId))
			{
				RateIcon.color = RatedColor;
			}
			if (ParseAPI.Instance.IsCommunityLevelLikedThisSession(currentLevelId))
			{
				num++;
			}
			CommunityLevelTitle.text = string.Format("\"{0}\"", (!string.IsNullOrEmpty(CommunityManager.CurrentLevelStats.Title)) ? CommunityManager.CurrentLevelStats.Title : "Untitled");
			CommunityLevelAuthor.text = CommunityManager.CurrentLevelStats.Author;
			HeartCountText.text = string.Format("{0}", num);
			CoinEarnedText.text = string.Format("{0}", coinsEarned);
			GemEarnedText.text = string.Format("{0}", gemsEarned);
		}
	}

	public override void ShowNoAdsControl(bool enabled)
	{
		ButtonNoAds.gameObject.SetActive(enabled);
	}

	public override void LoadMenu()
	{
		DialogManager.ShowLoadingDialog();
		m_deferredActions.Add(delegate
		{
			if (LevelManager.CommunityLevel)
			{
				ScreenManager.MenuScreen = MenuScreen.Community;
			}
			else
			{
				ScreenManager.MenuScreen = MenuScreen.LevelSelect;
			}
			SceneManager.LoadSceneAsync("LevelSelect");
		});
	}

	public void PerformAction(Action action)
	{
		m_deferredActions.Add(action);
	}

	public void ShowAdPanel(bool show)
	{
		((RectTransform)AdPanel.transform).anchoredPosition = Vector2.zero;
		AdPanel.SetActive(show);
	}

	public void PurchaseNoAds()
	{
		DialogManager.ShowDialog(TextLibrary.Get(StringId.S_CONNECTING_STORE));
		m_deferredActions.Add(delegate
		{
			StoreManager.Instance.Purchase(PurchasableItem.NO_ADS, OnPurchaseCompleted);
		});
	}

	private bool ShowRewardedVideo(Action failedCallback, Action skippedCallback, Action completedCallback, Action succeedCallback, Action closeCallback)
	{
		return AdContoller.Instance.ShowRewardedVideo(delegate(AdResult result)
		{
			switch (result)
			{
			case AdResult.Failed:
				failedCallback();
				return;
			case AdResult.Skipped:
				skippedCallback();
				return;
			case AdResult.Close:
				closeCallback();
				return;
			case AdResult.Completed:
				completedCallback();
				break;
			}
			succeedCallback();
		});
	}

	private void setupPenType()
	{
		switch (LevelManager.Instance.GetPenType())
		{
		case LevelPenType.Bouncy:
			ImagePenType.color = Color.magenta;
			break;
		case LevelPenType.Floaty:
			ImagePenType.color = Color.yellow;
			break;
		case LevelPenType.Red:
			ImagePenType.color = Color.red;
			break;
		case LevelPenType.Icy:
			ImagePenType.color = Color.cyan;
			break;
		case LevelPenType.Rough:
			ImagePenType.color = Color.green;
			break;
		default:
			ImagePenType.color = Color.white;
			break;
		}
	}
}
