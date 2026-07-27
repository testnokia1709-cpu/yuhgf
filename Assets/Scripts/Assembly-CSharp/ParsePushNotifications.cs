using Parse;
using UnityEngine;

public class ParsePushNotifications : MonoBehaviour
{
	public bool RequestOnStart;

	private ParseObject m_currentInstallation;

	private bool m_requestMade;

	private bool m_registrationSuccessful;

	private void Start()
	{
		if (PlayerPrefs.HasKey("currentInstallation"))
		{
			Debug.Log("Found Parse Registration");
			string objectId = PlayerPrefs.GetString("currentInstallation");
			m_currentInstallation = ParseObject.CreateWithoutData("_Installation", objectId);
		}
		if (m_currentInstallation == null && RequestOnStart)
		{
			RequestPushNotifications();
		}
	}

	private void FixedUpdate()
	{
		if (m_requestMade && !m_registrationSuccessful)
		{
			registerInstallation();
		}
	}

	public void RequestPushNotifications()
	{
	}

	private void registerInstallation()
	{
		m_registrationSuccessful = true;
	}
}
