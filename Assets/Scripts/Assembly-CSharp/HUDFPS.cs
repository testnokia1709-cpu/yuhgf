using UnityEngine;
using UnityEngine.UI;

public class HUDFPS : MonoBehaviour
{
	public static HUDFPS Instance;

	public Text DisplayText;

	public float updateInterval = 0.5f;

	private float accum;

	private int frames;

	private float timeleft;

	public float FPS { get; private set; }

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
		if (!DisplayText)
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			base.enabled = false;
		}
		else
		{
			timeleft = updateInterval;
		}
	}

	private void Update()
	{
		timeleft -= Time.deltaTime;
		accum += Time.timeScale / Time.deltaTime;
		frames++;
		if ((double)timeleft <= 0.0)
		{
			FPS = accum / (float)frames;
			string text = string.Format("{0:F2} FPS", FPS);
			DisplayText.text = text;
			if (FPS < 30f)
			{
				DisplayText.color = Color.yellow;
			}
			else if (FPS < 10f)
			{
				DisplayText.color = Color.red;
			}
			else
			{
				DisplayText.color = Color.green;
			}
			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
