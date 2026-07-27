using UnityEngine;

public class Demo : MonoBehaviour
{
	public static Demo Instance;

	public int Level;

	public bool ShowWelcome = true;

	private void Awake()
	{
		if (Instance != null)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
		LevelManager.LoadLevel(Level);
	}

	private void Update()
	{
	}
}
