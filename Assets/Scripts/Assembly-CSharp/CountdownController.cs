using System;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
	public static CountdownController Instance;

	public Text Text;

	public int Duration = 3;

	private Action m_onComplete;

	private bool m_enabled;

	private GameTimer m_timer = new GameTimer();

	private bool m_resumeGameTimer;

	public bool IsEnabled
	{
		get
		{
			return m_enabled;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		m_enabled = false;
	}

	private void Update()
	{
		if (!m_enabled)
		{
			return;
		}
		float time = m_timer.GetTime();
		if (time < (float)Duration)
		{
			SetText(Mathf.CeilToInt((float)Duration - time).ToString());
			return;
		}
		m_enabled = false;
		ClearText();
		if (m_onComplete != null)
		{
			m_onComplete();
		}
	}

	public void StartCountdown(Action onComplete)
	{
		if (!m_enabled)
		{
			GameStateManager.Instance.PauseTimer();
			m_timer.StartTimer();
			m_enabled = true;
			m_onComplete = onComplete;
		}
	}

	public void StopCountdown()
	{
		m_timer.ResetTimer();
		m_enabled = false;
		ClearText();
		m_onComplete = null;
		GameStateManager.Instance.ResumeTimer();
	}

	public void PauseCountdown()
	{
		if (m_enabled)
		{
			m_timer.PauseTimer();
			Text.gameObject.SetActive(false);
		}
	}

	public void ResumeCountdown()
	{
		if (m_enabled)
		{
			m_timer.ResumeTimer();
			Text.gameObject.SetActive(true);
		}
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus && !m_timer.IsPaused)
		{
			m_resumeGameTimer = true;
			m_timer.PauseTimer();
		}
		else if (!pauseStatus && m_timer.IsPaused && m_resumeGameTimer)
		{
			m_timer.ResumeTimer();
			m_resumeGameTimer = false;
		}
	}

	public void SetText(string text)
	{
		Text.text = text;
		if (!m_timer.IsPaused)
		{
			Text.gameObject.SetActive(true);
		}
	}

	public void ClearText()
	{
		Text.gameObject.SetActive(false);
		Text.text = string.Empty;
	}
}
