using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	public float WarningLevel;

	public Image ProgressImage;

	public Animator Animator;

	public float Progress { get; private set; }

	private void Start()
	{
		ProgressImage.fillAmount = Progress;
	}

	private void Update()
	{
	}

	public void SetProgress(float progress)
	{
		Progress = progress;
		ProgressImage.fillAmount = progress;
		if (progress < WarningLevel)
		{
			Animator.SetTrigger("Warning");
		}
		else
		{
			Animator.SetTrigger("Normal");
		}
	}
}
