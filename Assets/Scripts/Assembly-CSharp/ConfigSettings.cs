using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigSettings
{
	public int AdsInterval = 3;

	public bool AdsEnabled = true;

	public int NoticeID;

	public int NoticeReadID;

	public bool NoticeEnabled = true;

	public bool NoticeTimeLimited;

	public DateTime NoticeStart;

	public DateTime NoticeEnd;

	public string NoticeContents = string.Empty;

	public List<int> PackUnlockStars = new List<int>
	{
		30, 60, 105, 150, 195, 255, 315, 375, 465, 555,
		650
	};

	public bool ShowShareInsteadOfReplay;

	public bool ShowSolveCount = true;

	public bool HintsEnabled = true;

	public int NoticeIDAndroid;

	public string NoticeContentsAndroid = string.Empty;

	public bool CommunityLocked = true;

	public int Sale;

	public bool ShowNextGameAndroid;

	public bool ShowNextGameiOS;

	public int GetPackUnlockStars(int packIndex)
	{
		int result;
		if (packIndex > -1 && packIndex < PackUnlockStars.Count && int.TryParse(PackUnlockStars[packIndex].ToString(), out result))
		{
			return result;
		}
		return -1;
	}

	public bool IsNoticeAvailable()
	{
		return NoticeEnabled && (!NoticeTimeLimited || (NoticeTimeLimited && DateTime.UtcNow >= NoticeStart && DateTime.UtcNow <= NoticeEnd));
	}

	public int GetNoticeID()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			return NoticeIDAndroid;
		}
		return NoticeID;
	}
}
