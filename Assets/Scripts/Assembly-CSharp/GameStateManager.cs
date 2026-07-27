using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	public static GameStateManager Instance;

	public static readonly float TimeScale = 2f;

	public Action OnSetupComplete;

	public Action OnEditorSetupComplete;

	public Action OnPlaying;

	public Action<float, int> OnSolved;

	public Action OnReplayAvailable;

	public Action OnShowResults;

	public UIBase UserInterface;

	private static bool m_wasRecordingWhenReset;

	private static string s_lastLevelPlayed;

	private static int s_levelAttemptCount;

	private GameState m_oldState;

	private GameTimer m_gameTimer = new GameTimer();

	private bool m_resumeGameTimer;

	private float m_playTime;

	private float m_replayDelayStart;

	private readonly float m_replayDelayLength = 0.5f * TimeScale;

	private float m_lastUpdate;

	private readonly float m_timerUpdateInterval = 0.1f * TimeScale;

	public GameState State { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Debug.Log("Was Recording at reset: " + m_wasRecordingWhenReset);
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (State != m_oldState)
		{
			Debug.Log(string.Concat("State change: ", m_oldState, " to ", State));
			transitionState();
		}
		switch (State)
		{
		case GameState.Playing:
			if (Mathf.Abs(Time.time - m_lastUpdate) > m_timerUpdateInterval)
			{
				UserInterface.UpdateUI(getTimeInSeconds(m_gameTimer.GetTime()), TouchDrawPhysics.Instance.ShapeCount);
				m_lastUpdate = Time.time;
			}
			break;
		case GameState.VideoReplay:
			if (Time.realtimeSinceStartup - m_replayDelayStart > m_replayDelayLength)
			{
				SetState(GameState.ReportAnalytics);
			}
			break;
		}
	}

	public void SetState(GameState newState)
	{
		m_oldState = State;
		State = newState;
	}

	public void transitionState()
	{
		GameState oldState = m_oldState;
		if (oldState != GameState.Setup && oldState == GameState.VideoReplay)
		{
			string value = ((!LevelManager.CommunityLevel) ? LevelManager.Level.ToString() : ("(" + CommunityManager.CurrentTitle + ")"));
			EveryplayManager.Instance.StopRecording();
			EveryplayManager.Instance.SetMetadata("level", value);
			EveryplayManager.Instance.SetMetadata("time", m_playTime);
			EveryplayManager.Instance.SetMetadata("shapes", TouchDrawPhysics.Instance.ShapeCount);
			if (LevelManager.CommunityLevel)
			{
				EveryplayManager.Instance.SetMetadata("code", CommunityManager.CurrentLevelId);
				EveryplayManager.Instance.SetMetadata("author", CommunityManager.CurrentLevelStats.Author);
			}
			if (OnReplayAvailable != null)
			{
				OnReplayAvailable();
			}
		}
		m_oldState = State;
		switch (State)
		{
		case GameState.Setup:
			if (LevelManager.CommunityLevel)
			{
				TouchDrawLevel.RestoreLevel(CommunityManager.CurrentLevel, Vector2.zero);
				CommunityManager.CurrentLevel.SetupGoal();
				TouchDrawPhysics.Instance.SetShapeMaterial(CommunityManager.CurrentLevel.PenMaterial);
				TouchDrawPhysics.Instance.SetBoundary(TouchDrawEditor.ClampBottomLeft, TouchDrawEditor.ClampTopRight);
				StatsManager.Instance.PostCommunitySolve(CommunityManager.CurrentLevelIndex, LevelCompletion.Unsolved);
			}
			TouchDrawPhysics.Instance.ResetCounts();
			TouchDrawPhysics.Instance.EnablePhysics(LevelManager.Instance.IsStartActive());
			TouchDrawPhysics.Instance.SetCollisionMode(LevelManager.Instance.GetCollisionMode());
			UserInterface.OnSetup();
			UserInterface.UpdateUI(getTimeInSeconds(m_gameTimer.GetTime()), TouchDrawPhysics.Instance.ShapeCount);
			DataStore.Instance.MarketingSettings.LastLevelSolved = false;
			DataStore.Instance.MarketingSettings.LastLevelHadAd = false;
			if (OnSetupComplete != null)
			{
				OnSetupComplete();
			}
			SetState(GameState.WaitForPlayerStart);
			break;
		case GameState.EditorSetup:
			TouchDrawPhysics.Instance.SetCollisionMode(CollisionDetectionMode2D.None);
			m_gameTimer.EndTimer();
			UserInterface.OnSetup();
			UserInterface.UpdateUI(getTimeInSeconds(m_gameTimer.GetTime()), TouchDrawPhysics.Instance.ShapeCount);
			if (OnEditorSetupComplete != null)
			{
				OnEditorSetupComplete();
			}
			SetState(GameState.WaitForPlayerStart);
			break;
		case GameState.WaitForPlayerStart:
			TouchDrawPhysics.Instance.TouchEnabled = true;
			if (m_wasRecordingWhenReset && !EveryplayManager.Instance.IsRecording)
			{
				EveryplayManager.Instance.StartRecording();
				m_wasRecordingWhenReset = false;
			}
			break;
		case GameState.Playing:
		{
			m_lastUpdate = m_gameTimer.StartTimer();
			if (CountdownController.Instance.IsEnabled)
			{
				m_gameTimer.PauseTimer();
			}
			string levelKey = LevelManager.Instance.GetLevelKey();
			if (s_lastLevelPlayed != levelKey)
			{
				s_levelAttemptCount = 1;
			}
			else
			{
				s_levelAttemptCount++;
			}
			s_lastLevelPlayed = levelKey;
			if (OnPlaying != null)
			{
				OnPlaying();
			}
			break;
		}
		case GameState.Solved:
			DataStore.Instance.MarketingSettings.LastLevelSolved = true;
			m_playTime = getTimeInSeconds(m_gameTimer.EndTimer());
			UserInterface.UpdateUI(m_playTime, TouchDrawPhysics.Instance.ShapeCount);
			if (OnSolved != null)
			{
				OnSolved(m_playTime, TouchDrawPhysics.Instance.ShapeCount);
			}
			break;
		case GameState.ShowResults:
		{
			bool timeSolved = LevelManager.Instance.IsTimeSolved(m_playTime);
			bool shapeSolved = LevelManager.Instance.IsShapesSolved(TouchDrawPhysics.Instance.ShapeCount);
			int num = 0;
			int num2 = 0;
			LevelCompletion levelCompletion = LevelManager.Instance.GetLevelCompletion();
			if (!LevelManager.IsTryLevel)
			{
				LevelCompletion levelCompletion2 = LevelManager.Instance.SetLevelComplete(true, timeSolved, shapeSolved);
				if (!LevelManager.CommunityLevel)
				{
					if (levelCompletion == LevelCompletion.Unsolved)
					{
						DataStore.Instance.MarketingSettings.LevelsCompleted++;
					}
					bool flag3 = LevelManager.Instance.SetMinShapeCount(TouchDrawPhysics.Instance.ShapeCount);
					flag3 |= LevelManager.Instance.SetMinTime(m_playTime);
					if (levelCompletion2 > levelCompletion || flag3)
					{
						CloudOnceAPI.Instance.CloudSave();
					}
				}
				else if (TouchDrawPhysics.Instance.ShapeCount > 0)
				{
					if (!ParseAPI.Instance.GetCommunityThreeStar(CommunityManager.CurrentLevelId) && levelCompletion2 == LevelCompletion.Complete)
					{
						num2++;
					}
					if ((levelCompletion & LevelCompletion.Solved) != LevelCompletion.Solved && (levelCompletion2 & LevelCompletion.Solved) == LevelCompletion.Solved)
					{
						num++;
					}
					if ((levelCompletion & LevelCompletion.TimeSolved) != LevelCompletion.TimeSolved && (levelCompletion2 & LevelCompletion.TimeSolved) == LevelCompletion.TimeSolved)
					{
						num++;
					}
					if ((levelCompletion & LevelCompletion.ShapeSolved) != LevelCompletion.ShapeSolved && (levelCompletion2 & LevelCompletion.ShapeSolved) == LevelCompletion.ShapeSolved)
					{
						num++;
					}
					if (num > 0 || num2 > 0)
					{
						DataStore.Instance.CoinCount += num;
						DataStore.Instance.GemCount += num2;
						CloudOnceAPI.Instance.CloudSave();
					}
				}
				UserInterface.ShowGameComplete(levelCompletion, levelCompletion2, m_playTime, TouchDrawPhysics.Instance.ShapeCount, num, num2);
				AchievementManager.Instance.CalculateAchievements(true);
				if (OnShowResults != null)
				{
					OnShowResults();
				}
				if (LevelManager.CommunityLevel)
				{
					StatsManager.Instance.PostCommunitySolve(CommunityManager.CurrentLevelIndex, levelCompletion2);
				}
				else
				{
					StatsManager.Instance.PostLevelSolve(LevelManager.Instance.GetLevelKey(), 1, s_levelAttemptCount);
				}
				s_levelAttemptCount = 0;
				DataStore.Save();
			}
			else
			{
				LevelCompletion levelComplete = LevelManager.Instance.GetLevelComplete(true, timeSolved, shapeSolved);
				UserInterface.ShowGameComplete(levelCompletion, levelComplete, m_playTime, TouchDrawPhysics.Instance.ShapeCount, num, num2);
				if (OnShowResults != null)
				{
					OnShowResults();
				}
			}
			if (EveryplayManager.Instance.IsRecording)
			{
				m_replayDelayStart = Time.realtimeSinceStartup;
				SetState(GameState.VideoReplay);
			}
			else
			{
				SetState(GameState.ReportAnalytics);
			}
			break;
		}
		case GameState.VideoReplay:
			break;
		case GameState.ReportAnalytics:
			SetState(GameState.ShowAd);
			break;
		case GameState.ShowAd:
			if ((LevelManager.CommunityLevel || LevelManager.Level > 7) && AdContoller.Instance.IsTimeToShow())
			{
				bool flag = AdContoller.Instance.ShowStaticInterstitial(delegate(AdResult result)
				{
					if (result == AdResult.Completed)
					{
						DataStore.Instance.MarketingSettings.LastLevelHadAd |= true;
					}
				});
				bool flag2 = !StoreManager.Instance.HasPurchaseBeenMade() && flag;
				UserInterface.ShowNoAdsControl(flag2);
			}
			SetState(GameState.WaitForInput);
			break;
		case GameState.Retry:
			if (EveryplayManager.Instance.IsRecording)
			{
				EveryplayManager.Instance.StopRecording();
				m_wasRecordingWhenReset = true;
			}
			m_gameTimer.EndTimer();
			if (LevelManager.CommunityLevel)
			{
				TouchDrawPhysics.Instance.ClearShapes();
				SetState(GameState.Setup);
			}
			else
			{
				TouchDrawPhysics.Instance.EnablePhysics(false);
				LevelManager.Instance.RetryLevel();
			}
			break;
		case GameState.Cleanup:
			EveryplayManager.Instance.CleanUp();
			UserInterface.LoadMenu();
			break;
		case GameState.WaitForInput:
			break;
		}
	}

	public void PauseTimer()
	{
		m_gameTimer.PauseTimer();
	}

	public void ResumeTimer()
	{
		if (m_gameTimer.IsPaused && !CountdownController.Instance.IsEnabled)
		{
			m_gameTimer.ResumeTimer();
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && !m_gameTimer.IsPaused)
		{
			m_resumeGameTimer = true;
			m_gameTimer.PauseTimer();
		}
		else if (!pauseStatus && m_gameTimer.IsPaused && m_resumeGameTimer)
		{
			m_gameTimer.ResumeTimer();
			m_resumeGameTimer = false;
		}
	}

	private float getTimeInSeconds(float time)
	{
		time = Mathf.Floor(time * 10f) / 10f;
		return time;
	}
}
