using UnityEngine;

public class CameraFade : MonoBehaviour
{
	public GUIStyle m_BackgroundStyle = new GUIStyle();

	public Color m_CurrentColor = new Color(0f, 0f, 0f, 0f);

	public Color m_TargetColor = new Color(0f, 0f, 0f, 0f);

	public float StartDelay;

	public float FadeDuration = 1f;

	public bool DisableOnFinish;

	private Texture2D m_FadeTexture;

	private int m_FadeGUIDepth = -1000;

	private float m_startTime;

	public void Awake()
	{
	}

	public void Start()
	{
		m_FadeTexture = new Texture2D(1, 1);
		m_BackgroundStyle.normal.background = m_FadeTexture;
		m_startTime = Time.unscaledTime;
	}

	private void OnGUI()
	{
		float num = (Time.unscaledTime - m_startTime - StartDelay) / FadeDuration;
		Color screenOverlayColor = Color.Lerp(m_CurrentColor, m_TargetColor, num);
		if (num >= 0f && (num < 1f || !DisableOnFinish))
		{
			SetScreenOverlayColor(screenOverlayColor);
			if (screenOverlayColor.a > 0f)
			{
				GUI.depth = m_FadeGUIDepth;
				GUI.Label(new Rect(-10f, -10f, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
			}
		}
		else if (num >= 1f && DisableOnFinish)
		{
			Stop();
		}
	}

	private void SetScreenOverlayColor(Color c)
	{
		m_FadeTexture.SetPixel(0, 0, c);
		m_FadeTexture.Apply();
	}

	public void Stop()
	{
		base.enabled = false;
	}
}
