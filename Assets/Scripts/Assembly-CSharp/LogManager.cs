using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LogManager : MonoBehaviour
{
	public static LogManager Instance;

	private static readonly int HISTORY_SIZE = 50;

	private Queue<string> m_logQueue;

	public string GetLogHistory()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine();
		if (m_logQueue.Count == 0)
		{
			stringBuilder.AppendLine("No errors in log.");
		}
		else
		{
			stringBuilder.AppendLine("Error log:");
			foreach (string item in m_logQueue.Reverse())
			{
				stringBuilder.AppendLine(item);
			}
		}
		return stringBuilder.ToString();
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(this);
		m_logQueue = new Queue<string>(HISTORY_SIZE);
	}

	private void OnEnable()
	{
		Application.logMessageReceived += logCallback;
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= logCallback;
	}

	private void logCallback(string logString, string stackTrace, LogType type)
	{
		switch (type)
		{
		case LogType.Error:
			m_logQueue.Enqueue(logString);
			break;
		case LogType.Exception:
			m_logQueue.Enqueue(logString + ": " + stackTrace);
			break;
		}
		if (m_logQueue.Count > HISTORY_SIZE)
		{
			m_logQueue.Dequeue();
		}
	}
}
