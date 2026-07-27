using System;
using UnityEngine;

public abstract class AdSupport
{
	public bool IsActive { get; private set; }

	public void SetActive(bool active)
	{
		IsActive = active;
	}

	public virtual void ShowBanner()
	{
	}

	public virtual void HideBanner()
	{
	}

	public virtual bool IsRewardedVideoAvailable()
	{
		return false;
	}

	public virtual bool ShowRewardedVideo(Action<AdResult> adCompleteAction)
	{
		return false;
	}

	public virtual bool ShowStaticInterstitial(Action<AdResult> adCompleteAction)
	{
		return false;
	}

	public virtual void OnApplicationPause(bool isPaused)
	{
	}

	public virtual void EnsureAdsAreLoaded()
	{
	}

	protected string getAdvertisingID()
	{
		string empty = string.Empty;
		bool flag = false;
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.google.android.gms.ads.identifier.AdvertisingIdClient");
		AndroidJavaObject androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("getAdvertisingIdInfo", new object[1] { androidJavaObject });
		empty = androidJavaObject2.Call<string>("getId", new object[0]).ToString();
		flag = androidJavaObject2.Call<bool>("isLimitAdTrackingEnabled", new object[0]);
		Debug.Log("Advertising Id: " + empty);
		Debug.Log("Limit Tracking: " + flag);
		return empty;
	}
}
