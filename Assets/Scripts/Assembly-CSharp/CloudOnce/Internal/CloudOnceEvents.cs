using CloudOnce.Internal.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace CloudOnce.Internal
{
	public class CloudOnceEvents
	{
		public event UnityAction OnInitializeComplete;

		public event UnityAction<bool> OnSignedInChanged;

		public event UnityAction OnSignInFailed;

		public event UnityAction<Texture2D> OnPlayerImageDownloaded;

		public event UnityAction<bool> OnCloudSaveComplete;

		public event UnityAction<bool> OnCloudLoadComplete;

		public event UnityAction<string[]> OnNewCloudValues;

		public void RaiseOnInitializeComplete()
		{
			CloudOnceUtils.SafeInvoke(this.OnInitializeComplete);
		}

		public void RaiseOnSignedInChanged(bool isSignedIn)
		{
			CloudOnceUtils.SafeInvoke(this.OnSignedInChanged, isSignedIn);
		}

		public void RaiseOnSignInFailed()
		{
			CloudOnceUtils.SafeInvoke(this.OnSignInFailed);
		}

		public void RaiseOnPlayerImageDownloaded(Texture2D playerImage)
		{
			CloudOnceUtils.SafeInvoke(this.OnPlayerImageDownloaded, playerImage);
		}

		public void RaiseOnCloudSaveComplete(bool success)
		{
			CloudOnceUtils.SafeInvoke(this.OnCloudSaveComplete, success);
		}

		public void RaiseOnCloudLoadComplete(bool success)
		{
			CloudOnceUtils.SafeInvoke(this.OnCloudLoadComplete, success);
		}

		public void RaiseOnNewCloudValues(string[] changedKeys)
		{
			CloudOnceUtils.SafeInvoke(this.OnNewCloudValues, changedKeys);
		}
	}
}
