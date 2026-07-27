using System;
using UnityEngine;

public class AdContoller : MonoBehaviour
{
	public static AdContoller Instance;

	public bool IsTesting = true;

	public string IOSAppId;

	private AdSupport m_adSupport;

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		try
		{
			m_adSupport = new IronSourceSupport(StoreManager.Instance.HasPurchaseBeenMade(), StoreManager.Instance.IsGameOwned(), IsTesting);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error: " + ex);
			AnalyticsManager.LogDebugEvent("AdSupportFailedToInit");
		}
	}

	public void OnApplicationPause(bool isPaused)
	{
		if (m_adSupport != null)
		{
			m_adSupport.OnApplicationPause(isPaused);
		}
	}

	public bool IsTimeToShow()
	{
		return Marketing.IsItTimeToShowAnAd();
	}

	public bool ShowRewardedVideo(Action<AdResult> adCompleteAction)
	{
		if (m_adSupport == null)
		{
			return false;
		}
		if (StoreManager.Instance.IsGameOwned())
		{
			adCompleteAction(AdResult.PremiumUser);
			return true;
		}
		return m_adSupport.ShowRewardedVideo(adCompleteAction);
	}

	public bool ShowStaticInterstitial(Action<AdResult> adCompleteAction)
	{
		if (m_adSupport == null)
		{
			return false;
		}
		if (!DataStore.Instance.ConfigSettings.AdsEnabled)
		{
			Debug.Log("Ads are currently disabled.");
			return false;
		}
		if (StoreManager.Instance.IsGameOwned() || StoreManager.Instance.HasPurchaseBeenMade())
		{
			adCompleteAction(AdResult.PremiumUser);
			return true;
		}
		return m_adSupport.ShowStaticInterstitial(adCompleteAction);
	}

	public void EnsureAdsAreLoaded()
	{
		if (m_adSupport != null && !StoreManager.Instance.IsGameOwned())
		{
			m_adSupport.EnsureAdsAreLoaded();
		}
	}
}
