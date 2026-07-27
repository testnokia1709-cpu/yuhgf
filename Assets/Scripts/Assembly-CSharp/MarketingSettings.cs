using System;

[Serializable]
public struct MarketingSettings
{
	public bool HasReviewed;

	public int LevelsCompleted;

	public bool LastLevelSolved;

	public bool LastLevelHadAd;

	public int ViewedReviewReminder;
}
