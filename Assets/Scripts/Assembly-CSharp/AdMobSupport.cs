using System;

public class AdMobSupport : AdSupport
{
	public bool IsTesting = true;

	public AdMobSupport(string adUnitID)
	{
	}

	public override bool ShowStaticInterstitial(Action<AdResult> adCompleteAction)
	{
		return false;
	}
}
