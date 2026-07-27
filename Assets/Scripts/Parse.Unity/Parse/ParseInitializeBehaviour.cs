using UnityEngine;

namespace Parse
{
	public class ParseInitializeBehaviour : MonoBehaviour
	{
		private static bool isInitialized;

		[SerializeField]
		public string applicationID;

		[SerializeField]
		public string dotnetKey;

		[SerializeField]
		public string serverURL;

		public virtual void Awake()
		{
			Initialize();
			base.gameObject.name = "ParseInitializeBehaviour";
			if (!PlatformHooks.IsIOS)
			{
				return;
			}
			PlatformHooks.RegisterDeviceTokenRequest(delegate(byte[] deviceToken)
			{
				if (deviceToken != null)
				{
					ParseInstallation currentInstallation = ParseInstallation.CurrentInstallation;
					currentInstallation.SetDeviceTokenFromData(deviceToken);
					currentInstallation.SaveAsync();
				}
			});
		}

		public void OnApplicationPause(bool paused)
		{
			if (PlatformHooks.IsAndroid)
			{
				PlatformHooks.CallStaticJavaUnityMethod("com.parse.ParsePushUnityHelper", "setApplicationPaused", new object[1] { paused });
			}
		}

		private void Initialize()
		{
			if (!isInitialized)
			{
				isInitialized = true;
				Object.DontDestroyOnLoad(base.gameObject);
				ParseClient.Initialize(new ParseClient.Configuration
				{
					ApplicationId = applicationID,
					WindowsKey = dotnetKey,
					Server = serverURL
				});
				StartCoroutine(PlatformHooks.RunDispatcher());
			}
		}

		internal void OnPushNotificationReceived(string pushPayloadString)
		{
			Initialize();
			ParsePush.parsePushNotificationReceived.Invoke(ParseInstallation.CurrentInstallation, new ParsePushNotificationEventArgs(pushPayloadString));
		}

		internal void OnGcmRegistrationReceived(string registrationId)
		{
			Initialize();
			ParseInstallation currentInstallation = ParseInstallation.CurrentInstallation;
			currentInstallation.DeviceToken = registrationId;
			currentInstallation.Set("pushType", "gcm");
			currentInstallation.SaveAsync();
		}
	}
}
