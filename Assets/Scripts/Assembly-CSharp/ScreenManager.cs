using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
	public static ScreenManager Instance;

	public static MenuScreen MenuScreen;

	public List<GameObject> Panels = new List<GameObject>();

	private GameObject m_currentPanel;

	private GameObject m_previousPanel;

	public GameObject CurrentPanel
	{
		get
		{
			return m_currentPanel;
		}
	}

	public GameObject PreviousPanel
	{
		get
		{
			return m_previousPanel;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ShowPanel(GameObject panel)
	{
		foreach (GameObject panel2 in Panels)
		{
			panel2.transform.localPosition = new Vector3(-Screen.width, Screen.height, 0f);
		}
		panel.transform.localPosition = new Vector3(0f, 0f, 0f);
		m_previousPanel = m_currentPanel;
		m_currentPanel = panel;
	}
}
