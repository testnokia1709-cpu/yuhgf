using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectTimer : MonoBehaviour
{
	public Image TargetImage;

	public float TimeLimit;

	public float TimeMax;

	[HideInInspector]
	public List<Action> PerformActions = new List<Action>();

	private float m_startTime;

	private bool m_running;

	private void Start()
	{
		if (TimeMax < TimeLimit)
		{
			TimeMax = TimeLimit;
			if (TimeMax == 0f)
			{
				TimeMax = 10f;
			}
		}
		SetProgress(TimeLimit / TimeMax);
		TargetImage.enabled = true;
	}

	private void Update()
	{
		if (!m_running)
		{
			return;
		}
		float num = Time.time - m_startTime;
		float num2 = num / TimeLimit;
		float num3 = TimeLimit / TimeMax;
		SetProgress(num3 - num2 * num3);
		if (!(num > TimeLimit))
		{
			return;
		}
		foreach (Action performAction in PerformActions)
		{
			if (performAction != null)
			{
				performAction();
			}
		}
		TargetImage.enabled = false;
		m_running = false;
	}

	public void StartTimer()
	{
		m_startTime = Time.time;
		m_running = true;
	}

	public void SetProgress(float progress)
	{
		TargetImage.fillAmount = progress;
	}
}
