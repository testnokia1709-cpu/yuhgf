using System;
using UnityEngine;

public class EveryplayManager : MonoBehaviour
{
	public static EveryplayManager Instance;

	public Action OnReady;

	public Action OnRecordStarted;

	public Action OnRecordStopped;

	public Action OnInitialized;

	private bool m_initialized;

	private float m_supportPingTime = -10f;

	private int m_supportPingCount;

	private float m_recordStartTime;

	private bool m_restartRecording;

	private static int s_maxRecordingTime = 2;

	private static int s_targetFramerate = 60;

	private static float s_supportPingTimeout = 10f;

	private static float s_supportPingAttemptCount = 10f;

	public bool IsSupported { get; private set; }

	public bool IsRecording
	{
		get
		{
			bool result = false;
			if (m_initialized)
			{
				result = Everyplay.IsRecording();
			}
			return result;
		}
	}

	public bool IsReady
	{
		get
		{
			return IsSupported;
		}
	}

	public float RecordStartTime
	{
		get
		{
			return m_recordStartTime;
		}
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
	}

	private void Start()
	{
		Everyplay.ReadyForRecording += Everyplay_ReadyForRecording;
		Everyplay.RecordingStarted += Everyplay_RecordingStarted;
		Everyplay.RecordingStopped += Everyplay_RecordingStopped;
		Everyplay.WasClosed += Everyplay_WasClosed;
		Everyplay.FaceCamSessionStarted += Everyplay_FaceCamSessionStarted;
		Everyplay.UploadDidStart += Everyplay_UploadDidStart;
		Everyplay.UploadDidComplete += Everyplay_UploadDidComplete;
		Everyplay.Initialize();
		Everyplay.SetTargetFPS(s_targetFramerate);
		Everyplay.SetMaxRecordingMinutesLength(s_maxRecordingTime);
		m_initialized = true;
		m_restartRecording = false;
	}

	private void Update()
	{
		if (m_initialized && !IsSupported && (float)m_supportPingCount < s_supportPingAttemptCount && Time.realtimeSinceStartup - m_supportPingTime > s_supportPingTimeout)
		{
			m_supportPingTime = Time.realtimeSinceStartup;
			IsSupported = Everyplay.IsRecordingSupported();
			Debug.Log("Everyplay recording supported(" + m_supportPingCount + "): " + IsSupported);
			m_supportPingCount++;
			if (IsSupported && OnReady != null)
			{
				OnReady();
			}
		}
	}

	private void Everyplay_ReadyForRecording(bool enabled)
	{
		Debug.Log("Everyplay_ReadyForRecording: " + enabled);
		if (OnInitialized != null)
		{
			OnInitialized();
		}
	}

	private void Everyplay_RecordingStopped()
	{
		Debug.Log("Everyplay_RecordingStopped.");
		if (OnRecordStopped != null)
		{
			OnRecordStopped();
		}
		if (m_restartRecording)
		{
			Debug.Log("Restarting the recording");
			Everyplay.StartRecording();
		}
	}

	private void Everyplay_RecordingStarted()
	{
		Debug.Log("Everyplay_RecordingStarted.");
		if (OnRecordStarted != null)
		{
			OnRecordStarted();
		}
	}

	private void Everyplay_WasClosed()
	{
		Debug.Log("Everyplay_WasClosed.");
	}

	private void Everyplay_FaceCamSessionStarted()
	{
		AnalyticsManager.LogEvent("Social", "Everyplay", "FaceCam_Started", 1L);
	}

	private void Everyplay_UploadDidStart(int videoId)
	{
		AnalyticsManager.LogEvent("Social", "Everyplay", "Upload_Started", 1L);
	}

	private void Everyplay_UploadDidComplete(int videoId)
	{
		AnalyticsManager.LogEvent("Social", "Everyplay", "Upload_Complete", 1L);
	}

	public bool StartRecording()
	{
		if (!m_initialized)
		{
			return false;
		}
		if (IsReady)
		{
			if (!Everyplay.IsRecording())
			{
				Debug.Log("Everyplay.StartRecording");
				Everyplay.StartRecording();
			}
			else
			{
				Debug.Log("Already recording");
			}
			m_recordStartTime = Time.realtimeSinceStartup;
			return true;
		}
		return false;
	}

	public void PauseRecording()
	{
		if (m_initialized && Everyplay.IsRecording())
		{
			Debug.Log("Everyplay.PauseRecording");
			Everyplay.PauseRecording();
		}
	}

	public void ResumeRecording()
	{
		if (m_initialized && Everyplay.IsPaused())
		{
			Debug.Log("Everyplay.ResumeRecording");
			Everyplay.ResumeRecording();
		}
	}

	public bool StopRecording()
	{
		if (!m_initialized)
		{
			return false;
		}
		if (Everyplay.IsRecording())
		{
			Debug.Log("Everyplay.StopRecording");
			Everyplay.StopRecording();
			return true;
		}
		return false;
	}

	public void ShowReplay()
	{
		if (m_initialized)
		{
			Debug.Log("Everyplay.PlayLastRecording");
			AnalyticsManager.LogEvent("Social", "Everyplay", "Show_Replay", 1L);
			Everyplay.PlayLastRecording();
		}
	}

	public void ShowShare()
	{
		if (m_initialized)
		{
			AnalyticsManager.LogEvent("Social", "Everyplay", "Show_Share", 1L);
			Everyplay.ShowSharingModal();
		}
	}

	public void ShowCommunity()
	{
		AnalyticsManager.LogEvent("Social", "Everyplay", "Show_Community", 1L);
		if (!m_initialized || !IsSupported)
		{
			Application.OpenURL(Marketing.CommunityURL);
		}
		else
		{
			Everyplay.Show();
		}
	}

	public void SetMetadata(string key, object value)
	{
		if (m_initialized)
		{
			Everyplay.SetMetadata(key, value);
		}
	}

	public void CleanUp()
	{
		if (m_initialized && Everyplay.IsRecording())
		{
			Everyplay.StopRecording();
		}
	}
}
