using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
	public string SceneName;

	public Slider Slider;

	public bool RunInEditorOnly;

	public float Delay;

	private AsyncOperation m_asyncOp;

	private float m_startTime;

	private void Awake()
	{
		if (RunInEditorOnly && !Application.isEditor)
		{
			Object.Destroy(this);
		}
		if (RunInEditorOnly && Application.isEditor && LevelManager.Instance != null)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		m_startTime = Time.unscaledTime + Delay;
	}

	private void Update()
	{
		if (m_asyncOp != null)
		{
			if (Slider != null)
			{
				Slider.value = m_asyncOp.progress;
			}
		}
		else
		{
			m_asyncOp = SceneManager.LoadSceneAsync(SceneName);
			m_asyncOp.allowSceneActivation = false;
		}
		if (Time.unscaledTime > m_startTime)
		{
			m_asyncOp.allowSceneActivation = true;
		}
	}
}
