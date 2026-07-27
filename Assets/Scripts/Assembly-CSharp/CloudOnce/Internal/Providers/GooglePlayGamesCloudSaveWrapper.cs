using System;
using System.Text;
using CloudOnce.Internal.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.OurUtils;
using UnityEngine;

namespace CloudOnce.Internal.Providers
{
	public class GooglePlayGamesCloudSaveWrapper : ICloudStorageProvider
	{
		private const string c_saveGameFileName = "GameData";

		private static float s_timeWhenCloudSaveWasLoaded;

		private static bool s_saveInitialized;

		private static bool s_loadInitialized;

		private static bool s_isSynchronising;

		private readonly CloudOnceEvents cloudOnceEvents;

		public GooglePlayGamesCloudSaveWrapper(CloudOnceEvents events)
		{
			cloudOnceEvents = events;
		}

		public void Save()
		{
			if (s_saveInitialized)
			{
				return;
			}
			s_saveInitialized = true;
			DataManager.SaveToDisk();
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
			{
				s_saveInitialized = false;
				cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
			else if (DataManager.IsLocalDataDirty)
			{
				if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
				{
					PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("GameData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForSave);
					return;
				}
				s_saveInitialized = false;
				cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
			else
			{
				s_saveInitialized = false;
				cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
		}

		public void Load()
		{
			if (!CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.CloudSaveInitialized || !Cloud.CloudSaveEnabled)
			{
				cloudOnceEvents.RaiseOnCloudLoadComplete(false);
			}
			else if (!s_loadInitialized)
			{
				s_loadInitialized = true;
				if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
				{
					GooglePlayGames.OurUtils.Logger.d("Loading default save game.");
					PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("GameData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpenedForLoad);
				}
				else
				{
					GooglePlayGames.OurUtils.Logger.w("Load can only be called after authentication.");
					OnSavedGameDataRead(SavedGameRequestStatus.AuthenticationError, null);
				}
			}
		}

		public void Synchronize()
		{
			if (!s_isSynchronising)
			{
				s_isSynchronising = true;
				Cloud.OnCloudLoadComplete += OnCloudLoadComplete;
				Load();
			}
		}

		public bool DeleteVariable(string key)
		{
			return DataManager.DeleteCloudPref(key);
		}

		public string[] ClearUnusedVariables()
		{
			return DataManager.ClearStowawayVariablesFromGameData();
		}

		public void DeleteAll()
		{
			DataManager.DeleteAllCloudVariables();
		}

		public void SubscribeToAuthenticationEvent()
		{
			PlayGamesPlatform.Instance.OnAuthenticated -= Load;
			PlayGamesPlatform.Instance.OnAuthenticated += Load;
		}

		private static byte[] StringToBytes(string s)
		{
			if (s == null)
			{
				s = string.Empty;
			}
			return Encoding.Default.GetBytes(s);
		}

		private static string BytesToString(byte[] bytes)
		{
			return Encoding.Default.GetString(bytes);
		}

		private void OnSavedGameOpenedForLoad(SavedGameRequestStatus status, ISavedGameMetadata game)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
				return;
			}
			s_loadInitialized = false;
			GooglePlayGames.OurUtils.Logger.w("Failed to open saved game. Request status: " + status);
			cloudOnceEvents.RaiseOnCloudLoadComplete(false);
		}

		private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				s_timeWhenCloudSaveWasLoaded = Time.realtimeSinceStartup;
				ProcessCloudData(data);
			}
			else
			{
				s_loadInitialized = false;
				GooglePlayGames.OurUtils.Logger.w("Failed to load saved game. Request status: " + status);
				cloudOnceEvents.RaiseOnCloudLoadComplete(false);
			}
		}

		private void OnSavedGameOpenedForSave(SavedGameRequestStatus status, ISavedGameMetadata game)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				TimeSpan totalTimePlayed = game.TotalTimePlayed;
				totalTimePlayed += TimeSpan.FromSeconds(Time.realtimeSinceStartup - s_timeWhenCloudSaveWasLoaded);
				SaveGame(game, StringToBytes(DataManager.SerializeLocalData().ToBase64String()), totalTimePlayed);
			}
			else
			{
				s_saveInitialized = false;
				GooglePlayGames.OurUtils.Logger.w("Failed to open saved game. Request status: " + status);
				cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
		}

		private void SaveGame(ISavedGameMetadata game, byte[] savedData, TimeSpan totalPlaytime)
		{
			ISavedGameClient savedGame = PlayGamesPlatform.Instance.SavedGame;
			savedGame.CommitUpdate(game, default(SavedGameMetadataUpdate.Builder).WithUpdatedPlayedTime(totalPlaytime).WithUpdatedDescription("Saved game at " + DateTime.Now).Build(), savedData, OnSavedGameWritten);
		}

		private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
		{
			if (status == SavedGameRequestStatus.Success)
			{
				GooglePlayGames.OurUtils.Logger.d("Save successful!");
				DataManager.IsLocalDataDirty = false;
				cloudOnceEvents.RaiseOnCloudSaveComplete(true);
			}
			else
			{
				GooglePlayGames.OurUtils.Logger.w("Failed to write saved game. Request status: " + status);
				cloudOnceEvents.RaiseOnCloudSaveComplete(false);
			}
			s_saveInitialized = false;
		}

		private void ProcessCloudData(byte[] cloudData)
		{
			if (cloudData == null)
			{
				s_loadInitialized = false;
				cloudOnceEvents.RaiseOnCloudLoadComplete(true);
				return;
			}
			string text = BytesToString(cloudData);
			if (!string.IsNullOrEmpty(text))
			{
				if (!text.IsJson())
				{
					try
					{
						text = text.FromBase64StringToString();
					}
					catch (FormatException)
					{
						Debug.LogWarning("Unable to deserialize cloud data!");
						cloudOnceEvents.RaiseOnCloudLoadComplete(false);
						return;
					}
				}
				string[] array = DataManager.MergeLocalDataWith(text);
				if (array.Length > 0)
				{
					cloudOnceEvents.RaiseOnNewCloudValues(array);
				}
				s_loadInitialized = false;
				cloudOnceEvents.RaiseOnCloudLoadComplete(true);
			}
			else
			{
				s_loadInitialized = false;
				cloudOnceEvents.RaiseOnCloudLoadComplete(true);
			}
		}

		private void OnCloudLoadComplete(bool arg0)
		{
			Cloud.OnCloudLoadComplete -= OnCloudLoadComplete;
			Save();
			s_isSynchronising = false;
		}
	}
}
