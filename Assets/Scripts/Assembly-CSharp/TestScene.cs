using UnityEngine;
using UnityEngine.SceneManagement;

public class TestScene : MonoBehaviour
{
	private bool m_loadLevel;

	private string m_levelName;

	private void Start()
	{
		if (LevelManager.Instance == null)
		{
			Object.DontDestroyOnLoad(this);
			m_levelName = SceneManager.GetActiveScene().name;
			SceneManager.LoadScene("Splash");
			m_loadLevel = true;
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (m_loadLevel && LevelManager.Instance != null)
		{
			m_loadLevel = false;
			LevelManager.LoadLevel(m_levelName);
			Object.Destroy(this);
		}
	}
}
