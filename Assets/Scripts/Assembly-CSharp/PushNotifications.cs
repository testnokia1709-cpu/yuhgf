using UnityEngine;

public class PushNotifications : MonoBehaviour
{
	public static PushNotifications Instance;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Clear()
	{
	}

	public string GetNotification(int index)
	{
		return null;
	}

	public int GetNotificationCount()
	{
		return 0;
	}
}
