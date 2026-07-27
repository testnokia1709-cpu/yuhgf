using System;
using System.Collections.Generic;
using UnityEngine;

public class VungleSupport : AdSupport
{
	private Action<AdResult> m_onCompleteAction;

	private static Dictionary<string, object> s_adOptions = new Dictionary<string, object> { { "large", "true" } };

	public override bool ShowRewardedVideo(Action<AdResult> adCompleteAction)
	{
		return false;
	}

	private void Vungle_onAdViewedEvent(double arg1, double arg2)
	{
		Debug.Log("Vungle_onAdViewedEvent: " + arg1 + ", " + arg2);
		if (m_onCompleteAction != null)
		{
			m_onCompleteAction(AdResult.Completed);
		}
	}

	private void Vungle_onLogEvent(string obj)
	{
		Debug.Log("Vungle_onLogEvent: " + obj);
	}

	private void Vungle_onCachedAdAvailableEvent()
	{
		Debug.Log("Vungle_onCachedAdAvailableEvent");
	}

	private void Vungle_onAdEndedEvent()
	{
		Debug.Log("Vungle_onAdEndedEvent");
	}

	private void Vungle_onAdStartedEvent()
	{
		Debug.Log("Vungle_onAdStartedEvent");
	}
}
