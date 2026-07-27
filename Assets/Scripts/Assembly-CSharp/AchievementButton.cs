using UnityEngine;
using UnityEngine.UI;

public class AchievementButton : MonoBehaviour
{
	public Color NormalColor;

	public Color EarnedColor;

	public Color NormalBackgroundColor;

	public Color EarnedBackgroundColor;

	public Image Border;

	public Image Background;

	public Image ProgressBackground;

	public Image Progress;

	public bool Earned { get; private set; }

	public void Start()
	{
		Progress.color = EarnedBackgroundColor;
	}

	public void SetEarned(bool earned)
	{
		Earned = earned;
		Border.color = ((!earned) ? NormalColor : EarnedColor);
		Background.color = ((!earned) ? NormalBackgroundColor : EarnedBackgroundColor);
	}

	public void SetProgress(float progress)
	{
		if (progress > 0.01f && progress < 1f)
		{
			ProgressBackground.enabled = true;
			Progress.enabled = true;
			Progress.fillAmount = progress;
		}
		else
		{
			ProgressBackground.enabled = false;
			Progress.enabled = false;
		}
	}
}
