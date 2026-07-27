using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParseCache
{
	public SerializableDictionaryStringInt LevelAttempts = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt LevelSolves = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt CommunityAttempts = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt CommunitySolves = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt CommunityThreeStars = new SerializableDictionaryStringInt();

	public SerializableDictionaryStringInt CommunityLikes = new SerializableDictionaryStringInt();

	[SerializeField]
	private List<StringInt> m_levelAttempts = new List<StringInt>();

	[SerializeField]
	private List<StringInt> m_levelSolves = new List<StringInt>();

	[SerializeField]
	private List<StringInt> m_communityAttempts = new List<StringInt>();

	[SerializeField]
	private List<StringInt> m_communitySolves = new List<StringInt>();

	[SerializeField]
	private List<StringInt> m_communityThreeStars = new List<StringInt>();

	[SerializeField]
	private List<StringInt> m_communityLikes = new List<StringInt>();

	public void BeforeSave()
	{
		m_levelAttempts = new List<StringInt>();
		foreach (KeyValuePair<string, int> levelAttempt in LevelAttempts)
		{
			m_levelAttempts.Add(new StringInt
			{
				Key = levelAttempt.Key,
				Value = levelAttempt.Value
			});
		}
		m_levelSolves = new List<StringInt>();
		foreach (KeyValuePair<string, int> levelSolf in LevelSolves)
		{
			m_levelSolves.Add(new StringInt
			{
				Key = levelSolf.Key,
				Value = levelSolf.Value
			});
		}
		m_communityAttempts = new List<StringInt>();
		foreach (KeyValuePair<string, int> communityAttempt in CommunityAttempts)
		{
			m_communityAttempts.Add(new StringInt
			{
				Key = communityAttempt.Key,
				Value = communityAttempt.Value
			});
		}
		m_communitySolves = new List<StringInt>();
		foreach (KeyValuePair<string, int> communitySolf in CommunitySolves)
		{
			m_communitySolves.Add(new StringInt
			{
				Key = communitySolf.Key,
				Value = communitySolf.Value
			});
		}
		m_communityThreeStars = new List<StringInt>();
		foreach (KeyValuePair<string, int> communityThreeStar in CommunityThreeStars)
		{
			m_communityThreeStars.Add(new StringInt
			{
				Key = communityThreeStar.Key,
				Value = communityThreeStar.Value
			});
		}
		m_communityLikes = new List<StringInt>();
		foreach (KeyValuePair<string, int> communityLike in CommunityLikes)
		{
			m_communityLikes.Add(new StringInt
			{
				Key = communityLike.Key,
				Value = communityLike.Value
			});
		}
	}

	public void AfterLoad()
	{
		LevelAttempts = new SerializableDictionaryStringInt();
		foreach (StringInt levelAttempt in m_levelAttempts)
		{
			LevelAttempts.Add(levelAttempt.Key, levelAttempt.Value);
		}
		LevelSolves = new SerializableDictionaryStringInt();
		foreach (StringInt levelSolf in m_levelSolves)
		{
			LevelSolves.Add(levelSolf.Key, levelSolf.Value);
		}
		CommunityAttempts = new SerializableDictionaryStringInt();
		foreach (StringInt communityAttempt in m_communityAttempts)
		{
			CommunityAttempts.Add(communityAttempt.Key, communityAttempt.Value);
		}
		CommunitySolves = new SerializableDictionaryStringInt();
		foreach (StringInt communitySolf in m_communitySolves)
		{
			CommunitySolves.Add(communitySolf.Key, communitySolf.Value);
		}
		CommunityThreeStars = new SerializableDictionaryStringInt();
		foreach (StringInt communityThreeStar in m_communityThreeStars)
		{
			CommunityThreeStars.Add(communityThreeStar.Key, communityThreeStar.Value);
		}
		CommunityLikes = new SerializableDictionaryStringInt();
		foreach (StringInt communityLike in m_communityLikes)
		{
			CommunityLikes.Add(communityLike.Key, communityLike.Value);
		}
	}
}
