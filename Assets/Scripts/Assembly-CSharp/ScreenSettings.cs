using UnityEngine;

public class ScreenSettings : MonoBehaviour
{
	public bool FullScreen;

	public bool NeverSleep = true;

	private void Awake()
	{
		Screen.fullScreen = FullScreen;
		if (NeverSleep)
		{
			Screen.sleepTimeout = -1;
		}
	}
}
