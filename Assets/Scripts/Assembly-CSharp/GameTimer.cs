using UnityEngine;

public class GameTimer
{
	private float m_startTime;

	private float m_storedTime;

	private bool m_paused = true;

	private bool m_started;

	public bool IsPaused
	{
		get
		{
			return m_paused;
		}
	}

	public void ResetTimer()
	{
		m_storedTime = 0f;
	}

	public float StartTimer()
	{
		m_storedTime = 0f;
		m_startTime = Time.realtimeSinceStartup;
		m_paused = false;
		m_started = true;
		return m_startTime;
	}

	public void PauseTimer()
	{
		if (!m_paused)
		{
			m_storedTime += Time.realtimeSinceStartup - m_startTime;
		}
		m_paused = true;
	}

	public void ResumeTimer()
	{
		m_startTime = Time.realtimeSinceStartup;
		m_paused = false;
	}

	public float EndTimer()
	{
		float result = 0f;
		if (m_started)
		{
			result = GetTime();
		}
		m_paused = true;
		m_started = false;
		m_storedTime = 0f;
		return result;
	}

	public float GetTime()
	{
		float num = m_storedTime;
		if (!m_paused)
		{
			num += Time.realtimeSinceStartup - m_startTime;
		}
		return num;
	}
}
