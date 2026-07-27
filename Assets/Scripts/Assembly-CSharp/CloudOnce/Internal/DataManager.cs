using System;
using System.Collections.Generic;
using System.Globalization;
using CloudOnce.Internal.Providers;
using CloudOnce.Internal.Utils;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using UnityEngine;

namespace CloudOnce.Internal
{
	public static class DataManager
	{
		public const string DevStringKey = "CloudOnceDevString";

		private static Dictionary<string, IPersistent> s_cloudPrefs;

		private static GameData s_localGameData = new GameData();

		private static bool s_isInitialized;

		public static bool IsLocalDataDirty
		{
			get
			{
				return s_localGameData.IsDirty;
			}
			set
			{
				s_localGameData.IsDirty = value;
			}
		}

		public static Dictionary<string, IPersistent> CloudPrefs
		{
			get
			{
				return s_cloudPrefs ?? (s_cloudPrefs = new Dictionary<string, IPersistent>());
			}
		}

		public static void InitDataManager()
		{
			if (!s_isInitialized)
			{
				s_isInitialized = true;
				LoadFromDisk();
			}
		}

		public static void InitializeCurrency(string key)
		{
			if (!s_localGameData.SyncableCurrencies.ContainsKey(key))
			{
				s_localGameData.SyncableCurrencies.Add(key, new SyncableCurrency(key));
				IsLocalDataDirty = true;
			}
		}

		public static void InitializeBool(string key, PersistenceType persistenceType, bool value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Bool, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeInt(string key, PersistenceType persistenceType, int value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Int, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeUInt(string key, PersistenceType persistenceType, uint value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.UInt, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeFloat(string key, PersistenceType persistenceType, float value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Float, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString("R", CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeDouble(string key, PersistenceType persistenceType, double value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Double, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString("R", CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeString(string key, PersistenceType persistenceType, string value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.String, persistenceType);
				SyncableItem item = new SyncableItem(value, metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeLong(string key, PersistenceType persistenceType, long value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Long, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeDateTime(string key, PersistenceType persistenceType, DateTime value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Long, persistenceType);
				SyncableItem item = new SyncableItem(value.ToBinary().ToString(CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void InitializeDecimal(string key, PersistenceType persistenceType, decimal value)
		{
			if (!s_localGameData.SyncableItems.ContainsKey(key))
			{
				SyncableItemMetaData metadata = new SyncableItemMetaData(DataType.Decimal, persistenceType);
				SyncableItem item = new SyncableItem(value.ToString(CultureInfo.InvariantCulture), metadata);
				CreateItem(key, item);
			}
		}

		public static void SetCurrencyValues(string key, Dictionary<string, CurrencyValue> currencyValues)
		{
			SyncableCurrency value;
			if (s_localGameData.SyncableCurrencies.TryGetValue(key, out value))
			{
				value.DeviceCurrencyValues = currencyValues;
				IsLocalDataDirty = true;
				return;
			}
			throw new KeyNotFoundException(key);
		}

		public static void SetBool(string key, bool value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Bool)
			{
				s_localGameData.SyncableItems[key].ValueString = ((!value) ? 0.ToString(CultureInfo.InvariantCulture) : 1.ToString(CultureInfo.InvariantCulture));
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(bool));
		}

		public static void SetInt(string key, int value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Int)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(int));
		}

		public static void SetUInt(string key, uint value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.UInt)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(uint));
		}

		public static void SetFloat(string key, float value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Float)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToString("R", CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(float));
		}

		public static void SetDouble(string key, double value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Double)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToString("R", CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(double));
		}

		public static void SetString(string key, string value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.String)
			{
				s_localGameData.SyncableItems[key].ValueString = value;
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(string));
		}

		public static void SetLong(string key, long value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Long)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(long));
		}

		public static void SetDateTime(string key, DateTime value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Long)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToBinary().ToString(CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(long));
		}

		public static void SetDecimal(string key, decimal value)
		{
			if (s_localGameData.SyncableItems[key].Metadata.DataType == DataType.Decimal)
			{
				s_localGameData.SyncableItems[key].ValueString = value.ToString(CultureInfo.InvariantCulture);
				IsLocalDataDirty = true;
				return;
			}
			throw new UnexpectedCollectionElementTypeException(key, typeof(decimal));
		}

		public static Dictionary<string, CurrencyValue> GetCurrencyValues(string key)
		{
			SyncableCurrency value;
			return (!s_localGameData.SyncableCurrencies.TryGetValue(key, out value)) ? null : value.DeviceCurrencyValues;
		}

		public static bool GetBool(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Bool)
				{
					int result;
					if (int.TryParse(value.ValueString, out result))
					{
						return result == 1;
					}
					return Convert.ToBoolean(value.ValueString, CultureInfo.InvariantCulture);
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(bool));
			}
			throw new KeyNotFoundException(key);
		}

		public static int GetInt(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Int)
				{
					return Convert.ToInt32(value.ValueString);
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(int));
			}
			throw new KeyNotFoundException(key);
		}

		public static uint GetUInt(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.UInt)
				{
					return Convert.ToUInt32(value.ValueString, CultureInfo.InvariantCulture);
				}
				return 0u;
			}
			throw new KeyNotFoundException(key);
		}

		public static float GetFloat(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Float)
				{
					return Convert.ToSingle(value.ValueString, CultureInfo.InvariantCulture);
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(float));
			}
			throw new KeyNotFoundException(key);
		}

		public static double GetDouble(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Double)
				{
					return Convert.ToDouble(value.ValueString, CultureInfo.InvariantCulture);
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(double));
			}
			throw new KeyNotFoundException(key);
		}

		public static string GetString(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.String)
				{
					return value.ValueString;
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(string));
			}
			throw new KeyNotFoundException(key);
		}

		public static long GetLong(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Long)
				{
					return Convert.ToInt64(value.ValueString, CultureInfo.InvariantCulture);
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(long));
			}
			throw new KeyNotFoundException(key);
		}

		public static DateTime GetDateTime(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Long)
				{
					return DateTime.FromBinary(Convert.ToInt64(value.ValueString, CultureInfo.InvariantCulture));
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(long));
			}
			throw new KeyNotFoundException(key);
		}

		public static decimal GetDecimal(string key)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.DataType == DataType.Decimal)
				{
					return Convert.ToDecimal(value.ValueString, CultureInfo.InvariantCulture);
				}
				throw new UnexpectedCollectionElementTypeException(key, typeof(decimal));
			}
			throw new KeyNotFoundException(key);
		}

		public static void RefreshCloudValues()
		{
			foreach (KeyValuePair<string, IPersistent> cloudPref in CloudPrefs)
			{
				cloudPref.Value.Load();
			}
		}

		public static void ResetSyncableCurrency(string key)
		{
			SyncableCurrency value;
			if (s_localGameData.SyncableCurrencies.TryGetValue(key, out value))
			{
				value.ResetCurrency();
				IsLocalDataDirty = true;
				return;
			}
			throw new KeyNotFoundException(key);
		}

		public static bool DeleteCloudPref(string key)
		{
			if (s_localGameData.SyncableItems.ContainsKey(key))
			{
				s_localGameData.SyncableItems.Remove(key);
				return true;
			}
			if (s_localGameData.SyncableCurrencies.ContainsKey(key))
			{
				s_localGameData.SyncableCurrencies.Remove(key);
				return true;
			}
			return false;
		}

		public static string[] ResetAllData()
		{
			foreach (KeyValuePair<string, IPersistent> cloudPref in CloudPrefs)
			{
				cloudPref.Value.Reset();
			}
			return s_localGameData.GetAllKeys();
		}

		public static void DeleteAllCloudVariables()
		{
			DeleteCloudData();
			ClearStowawayVariablesFromGameData();
			foreach (KeyValuePair<string, IPersistent> cloudPref in CloudPrefs)
			{
				cloudPref.Value.Reset();
			}
		}

		public static string[] ClearStowawayVariablesFromGameData()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, SyncableItem> syncableItem in s_localGameData.SyncableItems)
			{
				if (!s_cloudPrefs.ContainsKey(syncableItem.Key))
				{
					list.Add(syncableItem.Key);
				}
			}
			List<string> list2 = new List<string>();
			foreach (KeyValuePair<string, SyncableCurrency> syncableCurrency in s_localGameData.SyncableCurrencies)
			{
				if (!s_cloudPrefs.ContainsKey(syncableCurrency.Key))
				{
					list2.Add(syncableCurrency.Key);
				}
			}
			foreach (string item in list)
			{
				s_localGameData.SyncableItems.Remove(item);
			}
			foreach (string item2 in list2)
			{
				s_localGameData.SyncableCurrencies.Remove(item2);
				list.Add(item2);
			}
			return list.ToArray();
		}

		public static void SaveToDisk()
		{
			foreach (KeyValuePair<string, IPersistent> cloudPref in CloudPrefs)
			{
				cloudPref.Value.Flush();
			}
			if (IsLocalDataDirty)
			{
				PlayerPrefs.SetString("CloudOnceDevString", SerializeLocalData().ToBase64String());
				PlayerPrefs.Save();
			}
		}

		public static string[] LoadFromDisk()
		{
			string text = PlayerPrefs.GetString("CloudOnceDevString");
			if (!string.IsNullOrEmpty(text) && !text.IsJson())
			{
				try
				{
					text = text.FromBase64StringToString();
				}
				catch (FormatException)
				{
					Debug.LogWarning("Unable to deserialize local data! Resetting it.");
					text = string.Empty;
				}
			}
			string[] array;
			if (s_localGameData == null)
			{
				s_localGameData = new GameData(text);
				RefreshCloudValues();
				array = new string[0];
			}
			else
			{
				array = MergeLocalDataWith(text);
				if (array.Length > 0)
				{
					RefreshCloudValues();
				}
			}
			return array;
		}

		public static string SerializeLocalData()
		{
			return s_localGameData.Serialize();
		}

		public static string[] MergeLocalDataWith(string otherData)
		{
			string[] array = s_localGameData.MergeWith(new GameData(otherData));
			if (array.Length > 0)
			{
				RefreshCloudValues();
				SaveToDisk();
			}
			return array;
		}

		public static string[] ReplaceLocalDataWith(string otherData)
		{
			s_localGameData = new GameData(otherData);
			foreach (KeyValuePair<string, IPersistent> cloudPref in CloudPrefs)
			{
				cloudPref.Value.Reset();
			}
			RefreshCloudValues();
			SaveToDisk();
			return s_localGameData.GetAllKeys();
		}

		private static void CreateItem(string key, SyncableItem item)
		{
			SyncableItem value;
			if (s_localGameData.SyncableItems.TryGetValue(key, out value))
			{
				if (value.Metadata.PersistenceType == item.Metadata.PersistenceType && !value.Equals(item))
				{
					s_localGameData.SyncableItems[key] = ConflictResolver.ResolveConflict(value, item);
					IsLocalDataDirty = true;
				}
			}
			else
			{
				s_localGameData.SyncableItems.Add(key, item);
				IsLocalDataDirty = true;
			}
		}

		private static void DeleteCloudData()
		{
			if (CloudProviderBase<GooglePlayGamesCloudProvider>.Instance.IsGpgsInitialized && PlayGamesPlatform.Instance.IsAuthenticated())
			{
				PlayGamesPlatform.Instance.SavedGame.OpenWithAutomaticConflictResolution("GameData", DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, delegate(SavedGameRequestStatus status, ISavedGameMetadata metadata)
				{
					if (status == SavedGameRequestStatus.Success)
					{
						PlayGamesPlatform.Instance.SavedGame.Delete(metadata);
					}
				});
			}
			PlayerPrefs.DeleteKey("CloudOnceDevString");
			PlayerPrefs.Save();
		}
	}
}
