using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParseHistory
{
	public SerializableDictionaryStringInt CommunityLikes = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt CommunityLevelCompletion = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringBool CommunityThreeStars = new SerializableDictionaryStringBool();

	[SerializeField]
	private List<StringInt> m_communityLikes = new List<StringInt>();

	[SerializeField]
	private List<StringInt> m_communityLevelCompletion = new List<StringInt>();

	[SerializeField]
	private List<StringBool> m_communityThreeStars = new List<StringBool>();

	public void BeforeSave()
	{
		m_communityLikes = new List<StringInt>();
		foreach (KeyValuePair<string, int> communityLike in CommunityLikes)
		{
			m_communityLikes.Add(new StringInt
			{
				Key = communityLike.Key,
				Value = communityLike.Value
			});
		}
		m_communityLevelCompletion = new List<StringInt>();
		foreach (KeyValuePair<string, int> item in CommunityLevelCompletion)
		{
			m_communityLevelCompletion.Add(new StringInt
			{
				Key = item.Key,
				Value = item.Value
			});
		}
		m_communityThreeStars = new List<StringBool>();
		foreach (KeyValuePair<string, bool> communityThreeStar in CommunityThreeStars)
		{
			m_communityThreeStars.Add(new StringBool
			{
				Key = communityThreeStar.Key,
				Value = communityThreeStar.Value
			});
		}
	}

	public void AfterLoad()
	{
		CommunityLikes = new SerializableDictionaryStringInt();
		foreach (StringInt communityLike in m_communityLikes)
		{
			CommunityLikes.Add(communityLike.Key, communityLike.Value);
		}
		CommunityLevelCompletion = new SerializableDictionaryStringInt();
		foreach (StringInt item in m_communityLevelCompletion)
		{
			CommunityLevelCompletion.Add(item.Key, item.Value);
		}
		CommunityThreeStars = new SerializableDictionaryStringBool();
		foreach (StringBool communityThreeStar in m_communityThreeStars)
		{
			CommunityThreeStars.Add(communityThreeStar.Key, communityThreeStar.Value);
		}
	}
}
