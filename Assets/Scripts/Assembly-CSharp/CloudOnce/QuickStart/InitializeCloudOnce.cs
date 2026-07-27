using UnityEngine;

namespace CloudOnce.QuickStart
{
	[AddComponentMenu("CloudOnce/Initialize CloudOnce", 0)]
	public class InitializeCloudOnce : MonoBehaviour
	{
		[SerializeField]
		private bool cloudSaveEnabled = true;

		[SerializeField]
		private bool autoSignIn = true;

		[SerializeField]
		private bool autoCloudLoad = true;

		private void Start()
		{
			Cloud.Initialize(cloudSaveEnabled, autoSignIn, autoCloudLoad);
		}
	}
}
