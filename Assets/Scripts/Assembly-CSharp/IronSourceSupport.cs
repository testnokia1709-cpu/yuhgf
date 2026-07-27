using System;
using UnityEngine;

public class IronSourceSupport : AdSupport
{
	private Action<AdResult> m_onCompleteAction;

	private string m_userId = string.Empty;

	private string m_appKey = "3f3ad821";

	public IronSourceSupport(bool purchaseMade, bool gameOwned, bool isTesting)
	{
		if (!gameOwned)
		{
			m_userId = getAdvertisingID();
			IronSource.Agent.setUserId(m_userId);
			IronSource.Agent.setAdaptersDebug(isTesting);
			Debug.Log("IronSource.Agent.init");
			if (!purchaseMade)
			{
				IronSource.Agent.init(m_appKey, IronSourceAdUnits.REWARDED_VIDEO, IronSourceAdUnits.INTERSTITIAL);
			}
			else
			{
				IronSource.Agent.init(m_appKey, IronSourceAdUnits.REWARDED_VIDEO);
			}
			IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
			IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
			IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
			IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
			IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
			IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
			IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialLoadFailedEvent;
			IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
			IronSourceEvents.onInterstitialAdReadyEvent += InterstitialReadyEvent;
			IronSourceEvents.onInterstitialAdRewardedEvent += InterstitialAdRewardedEvent;
			IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialShowFailEvent;
			IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialShowSuccessEvent;
			Debug.Log("Validating IronSource integration...");
			IronSource.Agent.validateIntegration();
			IronSource.Agent.loadInterstitial();
		}
	}

	private void RewardedVideoAvailabilityChangedEvent(bool value)
	{
		Debug.Log("IronSourceSupport.RewardedVideoAvailabilityChangedEvent, value = " + value);
	}

	private void RewardedVideoAdStartedEvent()
	{
		Debug.Log("IronSourceSupport.RewardedVideoAdStartedEvent");
	}

	private void RewardedVideoAdOpenedEvent()
	{
		Debug.Log("IronSourceSupport.RewardedVideoAdOpenedEvent");
	}

	private void RewardedVideoAdEndedEvent()
	{
		Debug.Log("IronSourceSupport.RewardedVideoAdEndedEvent");
	}

	private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
	{
		Debug.Log("IronSourceSupport.RewardedVideoAdRewardedEvent, amount = " + obj.getRewardAmount() + " name = " + obj.getRewardName());
		int rewardAmount = obj.getRewardAmount();
		AdResult obj2 = AdResult.Skipped;
		if (rewardAmount > 0)
		{
			AnalyticsManager.LogEvent("Advertising", "RewardedVideo", "AdCompleted", 1L);
			obj2 = AdResult.Completed;
		}
		else
		{
			AnalyticsManager.LogEvent("Advertising", "RewardedVideo", "AdSkipped", 1L);
		}
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(obj2);
		}
	}

	private void RewardedVideoAdClosedEvent()
	{
		Debug.Log("IronSourceSupport.RewardedVideoAdClosedEvent");
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(AdResult.Close);
		}
		UIManager.Instance.PerformAction(delegate
		{
			AudioManager.Instance.ResumeMusic();
		});
	}

	private void RewardedVideoAdShowFailedEvent(IronSourceError obj)
	{
		Debug.Log("IronSourceSupport.RewardedVideoInitFailEvent, code :  " + obj.getCode() + ", description : " + obj.getDescription());
		AnalyticsManager.LogEvent("Advertising", "RewardedVideo", "AdFailed", 1L);
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(AdResult.Failed);
		}
		UIManager.Instance.PerformAction(delegate
		{
			AudioManager.Instance.ResumeMusic();
		});
	}

	public override bool ShowStaticInterstitial(Action<AdResult> adCompleteAction)
	{
		bool flag = IronSource.Agent.isInterstitialReady();
		Debug.Log("IronSourceSupport.ShowStaticInterstitial(): Ready: " + flag);
		if (flag)
		{
			m_onCompleteAction = adCompleteAction;
			IronSource.Agent.showInterstitial();
			return true;
		}
		IronSource.Agent.loadInterstitial();
		return false;
	}

	public override void ShowBanner()
	{
	}

	public override void HideBanner()
	{
	}

	public override bool IsRewardedVideoAvailable()
	{
		return IronSource.Agent.isRewardedVideoAvailable();
	}

	public override bool ShowRewardedVideo(Action<AdResult> adCompleteAction)
	{
		bool flag = IsRewardedVideoAvailable();
		Debug.Log("IronSourceSupport.ShowRewardedVideo(): Ready: " + flag);
		if (flag)
		{
			UnityMainThreadDispatcher.Instance().Enqueue(delegate
			{
				AudioManager.Instance.PauseMusic();
				m_onCompleteAction = adCompleteAction;
				IronSource.Agent.showRewardedVideo();
			});
			return true;
		}
		return false;
	}

	public override void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}

	private void InterstitialLoadFailedEvent(IronSourceError error)
	{
		Debug.Log("IronSourceSupport.InterstitialLoadFailedEvent, code: " + error.getCode() + ", description : " + error.getDescription());
		AnalyticsManager.LogEvent("Advertising", "Interstitial", "LoadFailed", 1L);
	}

	private void InterstitialShowSuccessEvent()
	{
		Debug.Log("IronSourceSupport.InterstitialShowSuccessEvent");
		AnalyticsManager.LogEvent("Advertising", "Interstitial", "AdCompleted", 1L);
		IronSource.Agent.loadInterstitial();
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(AdResult.Completed);
		}
	}

	private void InterstitialShowFailEvent(IronSourceError error)
	{
		Debug.Log("IronSourceSupport.InterstitialShowFailEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
		AnalyticsManager.LogEvent("Advertising", "Interstitial", "AdFailed", 1L);
		IronSource.Agent.loadInterstitial();
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(AdResult.Failed);
		}
	}

	private void ISAvailability(bool available)
	{
	}

	private void InterstitialAdClickedEvent()
	{
		Debug.Log("IronSourceSupport.InterstitialAdClickedEvent");
	}

	private void InterstitialAdClosedEvent()
	{
		Debug.Log("IronSourceSupport.InterstitialAdClosedEvent");
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(AdResult.Close);
		}
	}

	private void InterstitialReadyEvent()
	{
		Debug.Log("IronSourceSupport.InterstitialReadyEvent");
	}

	private void InterstitialAdOpenedEvent()
	{
		Debug.Log("IronSourceSupport.InterstitialAdOpenedEvent");
	}

	private void InterstitialAdRewardedEvent()
	{
		Debug.Log("IronSourceSupport.InterstitialAdRewardedEvent");
	}
}
