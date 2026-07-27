using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEditorUI : UIBase
{
	public static LevelEditorUI Instance;

	public Button ButtonRetry;

	public Button ButtonDelete;

	public Button ButtonRotate;

	public Button ButtonClone;

	public Button ButtonRestart;

	public GameObject ButtonMenu;

	public GameObject GamePanel;

	public GameObject PhysicsLevel;

	public GameObject LeftPanel;

	public GameObject RightPanel;

	public GameObject KickerCanvas;

	public GameObject ObjectPickerPanel;

	public GameObject GoalPickerPanel;

	public GameObject Grid;

	public GameObject MaterialPanel;

	public GameObject SolvedPanel;

	public GameObject EditingControls;

	public GameObject LevelControls;

	public GameObject HintControls;

	public GameObject PlayControls;

	public GameObject GameStateControls;

	public GameObject ImportExportPanel;

	public Text GoalText;

	public Text HintText;

	public Image TimeGoalIcon;

	public Image ShapeGoalIcon;

	public Text TimeGoalText;

	public Text ShapeGoalText;

	public Text SolvedTimeText;

	public Text SolvedShapeText;

	public ObjectTimer TimeGoalTimer;

	public Toggle ColorWhiteToggle;

	public Toggle ColorRedToggle;

	public Toggle ColorMagentaToggle;

	public Toggle ColorYellowToggle;

	public Toggle ColorGreenToggle;

	public Toggle ColorCyanToggle;

	public Image ImagePlayPen;

	public GameObject MoveControl;

	public Color GoalItemColor;

	public Color GoalItemSelectedColor;

	public Toggle TogglePhysicsActive;

	public Toggle ToggleGround;

	public ToggleButton TogglePlayEdit;

	public Camera LevelCamera;

	public Camera UICamera;

	private List<Action> m_deferredActions;

	private bool m_waitOneFrame;

	private List<GameObject> m_kickerList = new List<GameObject>();

	private bool m_editMode = true;

	private bool m_levelSolved;

	private float m_levelSolveTime;

	private bool m_hintShown;

	private bool m_hintResumePhysics;

	private static string s_levelFilename = "editorlevel.json";

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		m_deferredActions = new List<Action>();
		TimeGoalText.text = "0.0123456789";
		TimeGoalText.text = "0.0";
		TouchDrawEditor.Instance.UserInterface = this;
	}

	private void Start()
	{
		GameStateManager.Instance.UserInterface = this;
		GameStateManager instance = GameStateManager.Instance;
		instance.OnPlaying = (Action)Delegate.Combine(instance.OnPlaying, new Action(OnPlaying));
		GameStateManager instance2 = GameStateManager.Instance;
		instance2.OnSolved = (Action<float, int>)Delegate.Combine(instance2.OnSolved, new Action<float, int>(OnSolved));
		GameStateManager instance3 = GameStateManager.Instance;
		instance3.OnShowResults = (Action)Delegate.Combine(instance3.OnShowResults, new Action(OnShowResults));
		GameStateManager instance4 = GameStateManager.Instance;
		instance4.OnReplayAvailable = (Action)Delegate.Combine(instance4.OnReplayAvailable, new Action(OnReplayAvailable));
		TouchDrawPhysics instance5 = TouchDrawPhysics.Instance;
		instance5.OnDrawShape = (Action<float>)Delegate.Combine(instance5.OnDrawShape, new Action<float>(OnDrawShape));
		TouchDrawPhysics.Instance.CanDraw = CanDraw;
		TextLibrary.Instance.SetFont(getAllText());
		LocalizeText();
		DialogManager.CloseDialog();
		UICamera.transform.position = LevelCamera.transform.position;
		GoalText.gameObject.SetActive(false);
		HintText.gameObject.SetActive(false);
		KickerCanvas.SetActive(false);
		CountdownController.Instance.ClearText();
		ObjectPickerPanel.SetActive(false);
		GamePanel.SetActive(true);
		ButtonRestart.gameObject.SetActive(false);
		HintControls.SetActive(false);
		GameStateControls.SetActive(false);
		SolvedPanel.SetActive(false);
		ShapeGoalText.text = LevelManager.Instance.GetShapeGoal().ToString();
		TimeGoalText.text = string.Format("{0:0.0}", LevelManager.Instance.GetTimeGoal());
		string levelData = DataFile.LoadText(s_levelFilename);
		TouchDrawEditor.Instance.LoadLevel(TouchDrawLevel.DecodeLevel(levelData));
		UpdateUI(TouchDrawEditor.Instance.Level.TimeGoal, TouchDrawEditor.Instance.Level.ShapeGoal);
		UpdateControls();
		TouchDrawPhysics.Instance.EnablePhysics(false);
		LevelManager.CommunityLevel = false;
		m_deferredActions.Add(delegate
		{
			VerticalLayoutGroup component = RightPanel.GetComponent<VerticalLayoutGroup>();
			component.enabled = false;
		});
	}

	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
		{
			MenuClicked();
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
	}

	private void OnDrawShape(float mass)
	{
	}

	private bool CanDraw()
	{
		return !m_editMode;
	}

	public override void OnSetup()
	{
		CountdownController.Instance.StopCountdown();
		if (!m_editMode)
		{
			TouchDrawPhysics.Instance.EnablePhysics(TouchDrawEditor.Instance.Level.ActiveOnStart);
			TouchDrawEditor.Instance.TouchEnabled = false;
			UpdateUI(TouchDrawEditor.Instance.Level.TimeGoal, TouchDrawEditor.Instance.Level.ShapeGoal);
			GamePanel.SetActive(true);
		}
		else
		{
			TouchDrawEditor.Instance.TouchEnabled = true;
			TouchDrawPhysics.Instance.EnablePhysics(false);
		}
	}

	private void OnPlaying()
	{
		TouchDrawPhysics.Instance.EnablePhysics(true);
		GoalText.gameObject.SetActive(false);
	}

	private void OnSolved(float time, int shapes)
	{
		if (TouchDrawRecorder.Instance.IsRecording)
		{
			TouchDrawRecorder.Instance.StopRecording(false);
			TouchDrawEditor.Instance.Level.StoreHint(TouchDrawRecorder.Instance);
		}
		m_levelSolved = true;
		m_levelSolveTime = time;
		m_deferredActions.Add(delegate
		{
			TouchDrawEditor.Instance.Level.TimeGoal = time;
			TouchDrawEditor.Instance.Level.ShapeGoal = shapes;
			UpdateUI(time, shapes);
		});
		SolvedShapeText.text = shapes.ToString();
		SolvedTimeText.text = string.Format("{0:0.0}", time);
		TouchDrawPhysics.Instance.EnablePhysics(false);
		SolvedPanel.SetActive(true);
	}

	private void OnShowResults()
	{
		PhysicsLevel.SetActive(false);
	}

	private void OnReplayAvailable()
	{
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
		list.Add(GoalText);
		list.Add(TimeGoalText);
		list.Add(ShapeGoalText);
		return list;
	}

	public void RetryClicked()
	{
		DialogManager.ShowDialog("This will clear all objects in the level, do you wish to continue?", TextLibrary.Get(StringId.S_YES), TextLibrary.Get(StringId.S_NO), delegate(int o)
		{
			if (o == 0)
			{
				TouchDrawEditor.Instance.ClearLevel();
				TouchDrawEditor.Instance.SaveLevel();
				string data = TouchDrawLevel.EncodeLevel(TouchDrawEditor.Instance.Level);
				DataFile.SaveText(s_levelFilename, data);
				GameStateManager.Instance.SetState(GameState.Retry);
			}
		});
	}

	public void MenuClicked()
	{
		if (m_editMode)
		{
			TouchDrawEditor.Instance.SaveLevel();
			string data = TouchDrawLevel.EncodeLevel(TouchDrawEditor.Instance.Level);
			DataFile.SaveText(s_levelFilename, data);
		}
		GameStateManager.Instance.SetState(GameState.Cleanup);
	}

	public void PenSelectClicked(string penTypeStr)
	{
		LevelPenType pen = (LevelPenType)Enum.Parse(typeof(LevelPenType), penTypeStr);
		TouchDrawEditor.Instance.SetPen(pen);
		UpdateControls();
	}

	public void GoalSelectClicked()
	{
	}

	public override void OnGoalSelected()
	{
		updateGoalPickerItems();
	}

	public void ObjectPickerClicked()
	{
		if (!ObjectPickerPanel.activeSelf)
		{
			ObjectPickerPanel.SetActive(true);
		}
		else
		{
			ObjectPickerPanel.SetActive(false);
		}
	}

	public void GoalPickerClicked()
	{
		if (!GoalPickerPanel.activeSelf)
		{
			UpdateControls();
			GoalPickerPanel.SetActive(true);
			LocalizeText();
			TouchDrawEditor.Instance.TouchEnabled = false;
			updateGoalPickerItems();
		}
		else
		{
			GoalPickerPanel.SetActive(false);
			TouchDrawEditor.Instance.TouchEnabled = true;
		}
	}

	private void updateGoalPickerItems()
	{
		int num = 0;
		foreach (StringId goalString in TouchDrawDefinition.Instance.GoalStrings)
		{
			GameObject gameObject = TouchDrawEditor.Instance.GoalPickerContainer.transform.GetChild(num).gameObject;
			Image componentInChildren = gameObject.GetComponentInChildren<Image>();
			componentInChildren.color = ((TouchDrawEditor.Instance.Level.Goal != goalString) ? GoalItemColor : GoalItemSelectedColor);
			num++;
		}
	}

	public void TogglePhysicsActiveClicked()
	{
		TouchDrawEditor.Instance.SetActiveOnStart(TogglePhysicsActive.isOn);
	}

	public void ToggleGroundClicked()
	{
		TouchDrawEditor.Instance.SetGround(ToggleGround.isOn);
	}

	public void UpdateControls()
	{
		Color white = Color.white;
		switch (TouchDrawEditor.Instance.Level.PenMaterial)
		{
		case LevelPenType.Normal:
			ColorWhiteToggle.isOn = true;
			white = Color.white;
			break;
		case LevelPenType.Red:
			ColorRedToggle.isOn = true;
			white = Color.red;
			break;
		case LevelPenType.Bouncy:
			ColorMagentaToggle.isOn = true;
			white = Color.magenta;
			break;
		case LevelPenType.Floaty:
			ColorYellowToggle.isOn = true;
			white = Color.yellow;
			break;
		case LevelPenType.Rough:
			ColorGreenToggle.isOn = true;
			white = Color.green;
			break;
		case LevelPenType.Icy:
			ColorCyanToggle.isOn = true;
			white = Color.cyan;
			break;
		}
		TogglePhysicsActive.isOn = TouchDrawEditor.Instance.Level.ActiveOnStart;
		ToggleGround.isOn = TouchDrawEditor.Instance.Level.Ground;
	}

	public void TogglePlayEditChanged(Toggle toggle)
	{
		bool editMode = m_editMode;
		m_editMode = toggle.isOn;
		if (!SetEditMode(m_editMode))
		{
			m_editMode = editMode;
			toggle.isOn = m_editMode;
		}
	}

	public void ContinueEditingClicked()
	{
		SetEditMode(true);
		SolvedPanel.SetActive(false);
		TogglePlayEdit.isOn = true;
	}

	public bool SetEditMode(bool edit)
	{
		bool result = true;
		if (edit)
		{
			TouchDrawEditor.Instance.TouchEnabled = true;
			TouchDrawEditor.Instance.Level.ResetGoal();
			TouchDrawPhysics.Instance.ClearShapes();
			TouchDrawEditor.Instance.RestoreLevel();
			TouchDrawRecorder.Instance.StopRecording(false);
			CountdownController.Instance.StopCountdown();
			Grid.SetActive(true);
			GoalText.gameObject.SetActive(false);
			EditingControls.SetActive(true);
			LevelControls.SetActive(true);
			HintControls.SetActive(false);
			ButtonRetry.gameObject.SetActive(true);
			ButtonMenu.gameObject.SetActive(true);
			ImportExportPanel.SetActive(true);
			ButtonRestart.gameObject.SetActive(false);
			GameStateControls.SetActive(false);
			ImagePlayPen.color = Color.white;
			UpdateUI(TouchDrawEditor.Instance.Level.TimeGoal, TouchDrawEditor.Instance.Level.ShapeGoal);
			if (m_levelSolved)
			{
				TouchDrawEditor.Instance.SaveLevel();
				string data = TouchDrawLevel.EncodeLevel(TouchDrawEditor.Instance.Level);
				DataFile.SaveText(s_levelFilename, data);
			}
			TouchDrawPhysics.Instance.EnablePhysics(false);
		}
		else
		{
			TouchDrawEditor.Instance.ClearSelection();
			TouchDrawEditor.Instance.SaveLevel();
			string data2 = TouchDrawLevel.EncodeLevel(TouchDrawEditor.Instance.Level);
			DataFile.SaveText(s_levelFilename, data2);
			if (TouchDrawEditor.Instance.Level.SetupGoal())
			{
				TouchDrawPhysics.Instance.ResetCounts();
				TouchDrawEditor.Instance.Level.TimeGoal = 0f;
				TouchDrawEditor.Instance.Level.ShapeGoal = 0;
				Grid.SetActive(false);
				GoalText.text = TextLibrary.Get(TouchDrawEditor.Instance.Level.Goal);
				GoalText.gameObject.SetActive(true);
				EditingControls.SetActive(false);
				LevelControls.SetActive(false);
				HintControls.SetActive(true);
				ButtonRetry.gameObject.SetActive(false);
				ButtonMenu.gameObject.SetActive(false);
				ImportExportPanel.SetActive(false);
				MoveControl.SetActive(false);
				GoalPickerPanel.SetActive(false);
				ObjectPickerPanel.SetActive(false);
				ButtonRestart.gameObject.SetActive(true);
				GameStateControls.SetActive(true);
				ImagePlayPen.color = TouchDrawPhysics.GetPenColor(TouchDrawEditor.Instance.Level.PenMaterial);
				GameStateManager.Instance.SetState(GameState.EditorSetup);
				TouchDrawRecorder.Instance.StartRecording();
				TouchDrawPhysics.Instance.EnablePhysics(true);
			}
			else
			{
				TouchDrawEditor.Instance.Level.ResetGoal();
				TouchDrawPhysics.Instance.EnablePhysics(false);
				TogglePlayEdit.isOn = true;
				result = false;
			}
		}
		return result;
	}

	public void ImportLevelClicked()
	{
		DialogManager.ShowInputDialog("Import Level", "Paste the level data here", TextLibrary.Get(StringId.S_OK), 0, delegate(int o)
		{
			if (o == 0)
			{
				string text = DialogManager.Instance.InputText.text;
				TouchDrawLevel touchDrawLevel = TouchDrawLevel.DecodeLevel(text);
				if (touchDrawLevel != null)
				{
					TouchDrawEditor.Instance.LoadLevel(touchDrawLevel);
					UpdateUI(TouchDrawEditor.Instance.Level.TimeGoal, TouchDrawEditor.Instance.Level.ShapeGoal);
					UpdateControls();
				}
				else
				{
					DialogManager.ShowDialog("Unable to load level data. Please ensure you have copied the entire level data string.", TextLibrary.Get(StringId.S_OK));
				}
			}
		});
	}

	public void EmailLevelClicked()
	{
		ContinueEditingClicked();
		DialogManager.ShowInputDialog("Please enter a level name", TouchDrawEditor.Instance.Level.Name, TextLibrary.Get(StringId.S_OK), 24, delegate(int o)
		{
			if (o == 0)
			{
				string text = DialogManager.Instance.InputText.text;
				if (!string.IsNullOrEmpty(text))
				{
					TouchDrawEditor.Instance.Level.Name = text;
				}
				TouchDrawEditor.Instance.SaveLevel();
				string text2 = TouchDrawLevel.EncodeLevel(TouchDrawEditor.Instance.Level);
				DataFile.SaveText(s_levelFilename, text2);
				if (text2 != null)
				{
					EmailSender.Send(string.Empty, "Brain It On! Level - " + TouchDrawEditor.Instance.Level.Name, text2);
				}
				else
				{
					DialogManager.ShowDialog("Error encoding data. Please contact support@brainitongame.com.", TextLibrary.Get(StringId.S_OK));
				}
			}
		});
	}

	public void SubmitLevelClicked()
	{
		if (CloudOnceAPI.Instance.IsSignedIn())
		{
			TimeSpan timeSpan = NTPTime.GetTime() - new DateTime(DataStore.Instance.LastSubmittedLevelTicks);
			if (timeSpan < TimeSpan.FromHours(12.0))
			{
				TimeSpan timeSpan2 = TimeSpan.FromHours(12.0) - timeSpan;
				string body = string.Format("Sorry, you can only submit a new level once every 12 hours. (Time remaining: {0:D2}:{1:D2}:{2:D2})", timeSpan2.Hours, timeSpan2.Minutes, timeSpan2.Seconds);
				DialogManager.ShowDialog(body, TextLibrary.Get(StringId.S_OK));
				return;
			}
			if (TouchDrawEditor.Instance.Level.ShapeGoal == 0)
			{
				DialogManager.ShowDialog("Please make sure to solve this level with at least 1 shape before submitting.", TextLibrary.Get(StringId.S_OK));
				return;
			}
			ContinueEditingClicked();
			DialogManager.ShowInputDialog("Please enter a level name", TouchDrawEditor.Instance.Level.Name, TextLibrary.Get(StringId.S_OK), 30, delegate(int o)
			{
				if (o == 0)
				{
					string text = DialogManager.Instance.InputText.text;
					if (string.IsNullOrEmpty(text))
					{
						text = DialogManager.Instance.InputText.placeholder.GetComponent<Text>().text;
					}
					if (string.IsNullOrEmpty(text))
					{
						text = "Untitled";
					}
					TouchDrawEditor.Instance.Level.Name = text;
					TouchDrawEditor.Instance.SaveLevel();
					string text2 = TouchDrawLevel.EncodeLevel(TouchDrawEditor.Instance.Level);
					DataFile.SaveText(s_levelFilename, text2);
					if (text2 != null)
					{
						ParseAPI.Instance.PostCommunityLevel(CloudOnceAPI.Instance.PlayerDisplayName, CloudOnceAPI.Instance.PlayerID, text, text2, delegate(bool success)
						{
							m_deferredActions.Add(delegate
							{
								if (success)
								{
									DialogManager.ShowDialog("Your level was submitted successfully.", TextLibrary.Get(StringId.S_OK));
									ParseAPI.Instance.ClearCachedLevels(LevelRequestFilter.User);
									DataStore.Instance.LastSubmittedLevelTicks = NTPTime.GetTime().Ticks;
									DataStore.Save();
								}
								else
								{
									DialogManager.ShowDialog("Error contacting server, please try again later.", TextLibrary.Get(StringId.S_OK));
								}
							});
						});
					}
					else
					{
						DialogManager.ShowDialog("Error encoding data. Please contact support@brainitongame.com.", TextLibrary.Get(StringId.S_OK));
					}
				}
			});
		}
		else
		{
			DialogManager.ShowDialog("Please sign in to submit your level.", TextLibrary.Get(StringId.S_OK));
		}
	}

	public void RotateSelectedObject()
	{
		TouchDrawEditor.Instance.RotateObject();
	}

	public void DeleteObjectClicked()
	{
		TouchDrawEditor.Instance.DeleteObject();
	}

	public void CloneSelectedObject()
	{
		TouchDrawEditor.Instance.CloneObject();
	}

	public void RestartClicked()
	{
		if (!m_editMode)
		{
			TouchDrawPhysics.Instance.ClearShapes();
			TouchDrawEditor.Instance.RestoreLevel();
			TouchDrawRecorder.Instance.StopRecording(false);
			TouchDrawEditor.Instance.Level.SetupGoal();
			TouchDrawPhysics.Instance.ResetCounts();
			GameStateManager.Instance.SetState(GameState.EditorSetup);
			TouchDrawRecorder.Instance.StartRecording();
			GoalText.gameObject.SetActive(true);
			SolvedPanel.SetActive(false);
			ButtonRestart.gameObject.SetActive(true);
		}
	}

	public void ShowHintClicked()
	{
		if (m_hintShown)
		{
			TouchDrawPhysics.Instance.EnablePhysics(m_hintResumePhysics);
			TouchDrawPhysics.Instance.TouchEnabled = true;
			CountdownController.Instance.ResumeCountdown();
			GameStateManager.Instance.ResumeTimer();
			HintText.gameObject.SetActive(false);
			GoalText.gameObject.SetActive(!m_hintResumePhysics);
			PlayControls.SetActive(true);
			HintController.Hide();
			m_hintShown = false;
		}
		else
		{
			m_hintResumePhysics = TouchDrawPhysics.Instance.IsPhysicsEnabled;
			TouchDrawPhysics.Instance.EnablePhysics(false);
			TouchDrawPhysics.Instance.TouchEnabled = false;
			CountdownController.Instance.PauseCountdown();
			GameStateManager.Instance.PauseTimer();
			TouchDrawEditor.Instance.ClearSelection();
			HintText.gameObject.SetActive(true);
			GoalText.gameObject.SetActive(false);
			PlayControls.SetActive(false);
			HintController.Show();
			m_hintShown = true;
		}
	}

	public void AdjustGoalTimeClicked()
	{
		if (m_levelSolved)
		{
			DialogManager.ShowInputDialog("Enter a new goal time", TouchDrawEditor.Instance.Level.TimeGoal.ToString("F1"), TextLibrary.Get(StringId.S_OK), 6, delegate
			{
				if (!string.IsNullOrEmpty(DialogManager.Instance.InputText.text))
				{
					string text = DialogManager.Instance.InputText.text;
					float result = 0f;
					if (float.TryParse(text, out result))
					{
						if (result >= m_levelSolveTime)
						{
							result = Mathf.Min(result, 60f);
							TouchDrawEditor.Instance.Level.TimeGoal = result;
							SolvedTimeText.text = string.Format("{0:0.0}", result);
							UpdateUI(TouchDrawEditor.Instance.Level.TimeGoal, TouchDrawEditor.Instance.Level.ShapeGoal);
						}
						else
						{
							DialogManager.ShowDialog("Please enter a time greater than: " + m_levelSolveTime, TextLibrary.Get(StringId.S_OK));
						}
					}
					else
					{
						DialogManager.ShowDialog("Please enter a valid time value (#.# -> 7.5).", TextLibrary.Get(StringId.S_OK));
					}
				}
			});
		}
		else
		{
			DialogManager.ShowDialog("Please solve the level before adjusting the time.", TextLibrary.Get(StringId.S_OK));
		}
	}

	public void EnableTouchDraw(bool enable)
	{
		TouchDrawPhysics.Instance.EnablePhysics(enable);
		TouchDrawPhysics.Instance.TouchEnabled = enable;
	}

	public override void UpdateUI(float duration, int shapes)
	{
		TimeGoalText.color = Color.white;
		TimeGoalIcon.color = Color.white;
		ShapeGoalText.color = Color.white;
		ShapeGoalIcon.color = Color.white;
		TimeGoalText.text = string.Format("{0:0.0}", duration);
		ShapeGoalText.text = string.Format("{0}", shapes);
		TimeGoalTimer.SetProgress(0f);
		m_levelSolved = TouchDrawEditor.Instance.Level.TimeGoal != 0f && TouchDrawEditor.Instance.Level.ShapeGoal != 0;
	}

	public override void OnObjectAdded()
	{
	}

	public override void OnObjectSelected()
	{
		ButtonDelete.interactable = TouchDrawEditor.Instance.IsObjectSelected || TouchDrawEditor.Instance.IsMultiObjectSelected;
		ButtonRotate.interactable = TouchDrawEditor.Instance.IsObjectSelected && !TouchDrawEditor.Instance.IsMultiObjectSelected;
		ButtonClone.interactable = TouchDrawEditor.Instance.IsObjectSelected || TouchDrawEditor.Instance.IsMultiObjectSelected;
	}

	public override void OnObjectDeselected()
	{
		ButtonDelete.interactable = TouchDrawEditor.Instance.IsObjectSelected || TouchDrawEditor.Instance.IsMultiObjectSelected;
		ButtonRotate.interactable = TouchDrawEditor.Instance.IsObjectSelected && !TouchDrawEditor.Instance.IsMultiObjectSelected;
		ButtonClone.interactable = TouchDrawEditor.Instance.IsObjectSelected || TouchDrawEditor.Instance.IsMultiObjectSelected;
	}

	public void SetGoalText(string text)
	{
		GoalText.text = text;
	}

	public override void LoadMenu()
	{
		DialogManager.ShowLoadingDialog();
		m_deferredActions.Add(delegate
		{
			ScreenManager.MenuScreen = MenuScreen.Community;
			SceneManager.LoadSceneAsync("LevelSelect");
		});
	}

	public void PerformAction(Action action)
	{
		m_deferredActions.Add(action);
	}
}
