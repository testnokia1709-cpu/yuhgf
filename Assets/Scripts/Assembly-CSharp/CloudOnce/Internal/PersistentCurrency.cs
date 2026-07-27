using System;
using System.Collections.Generic;
using UnityEngine;

namespace CloudOnce.Internal
{
	public class PersistentCurrency : IPersistent
	{
		private const string c_deviceIdKey = "CloudOnceDeviceID";

		private static string s_deviceIdCache;

		private Dictionary<string, CurrencyValue> deviceCurrencyValues;

		private CurrencyValue thisDeviceCurrencyValue;

		private float otherDevicesValueCache;

		public string Key { get; private set; }

		public float Additions
		{
			get
			{
				float num = thisDeviceCurrencyValue.Additions;
				if (deviceCurrencyValues != null)
				{
					foreach (KeyValuePair<string, CurrencyValue> deviceCurrencyValue in deviceCurrencyValues)
					{
						if (!(deviceCurrencyValue.Key == DeviceID))
						{
							num += deviceCurrencyValue.Value.Additions;
						}
					}
				}
				return num;
			}
		}

		public float Subtractions
		{
			get
			{
				float num = thisDeviceCurrencyValue.Subtractions;
				if (deviceCurrencyValues != null)
				{
					foreach (KeyValuePair<string, CurrencyValue> deviceCurrencyValue in deviceCurrencyValues)
					{
						if (!(deviceCurrencyValue.Key == DeviceID))
						{
							num += deviceCurrencyValue.Value.Subtractions;
						}
					}
				}
				return num;
			}
		}

		public float Value
		{
			get
			{
				float num = thisDeviceCurrencyValue.Value + DefaultValue;
				if (deviceCurrencyValues != null)
				{
					foreach (KeyValuePair<string, CurrencyValue> deviceCurrencyValue in deviceCurrencyValues)
					{
						if (!(deviceCurrencyValue.Key == DeviceID))
						{
							num += deviceCurrencyValue.Value.Value;
						}
					}
				}
				if (!AllowNegative && num < 0f)
				{
					Value = 0f;
					return 0f;
				}
				return num;
			}
			set
			{
				if (AllowNegative || value >= 0f)
				{
					thisDeviceCurrencyValue.Value = value - otherDevicesValueCache - DefaultValue;
				}
				else
				{
					thisDeviceCurrencyValue.Value = 0f - otherDevicesValueCache - DefaultValue;
				}
			}
		}

		public float DefaultValue { get; private set; }

		public bool AllowNegative { get; private set; }

		private static string DeviceID
		{
			get
			{
				if (!string.IsNullOrEmpty(s_deviceIdCache))
				{
					return s_deviceIdCache;
				}
				if (PlayerPrefs.HasKey("CloudOnceDeviceID"))
				{
					s_deviceIdCache = PlayerPrefs.GetString("CloudOnceDeviceID");
					return s_deviceIdCache;
				}
				s_deviceIdCache = Guid.NewGuid().ToString();
				PlayerPrefs.SetString("CloudOnceDeviceID", s_deviceIdCache);
				PlayerPrefs.Save();
				return s_deviceIdCache;
			}
		}

		protected PersistentCurrency(string key, float defaultValue, bool allowNegative)
		{
			Key = key;
			DefaultValue = defaultValue;
			AllowNegative = allowNegative;
			DataManager.CloudPrefs[key] = this;
			DataManager.InitDataManager();
		}

		public void Flush()
		{
			if (deviceCurrencyValues == null)
			{
				deviceCurrencyValues = new Dictionary<string, CurrencyValue>();
			}
			deviceCurrencyValues[DeviceID] = thisDeviceCurrencyValue;
			DataManager.SetCurrencyValues(Key, deviceCurrencyValues);
		}

		public void Load()
		{
			deviceCurrencyValues = DataManager.GetCurrencyValues(Key);
			if (deviceCurrencyValues != null)
			{
				thisDeviceCurrencyValue = ((!deviceCurrencyValues.ContainsKey(DeviceID)) ? new CurrencyValue() : deviceCurrencyValues[DeviceID]);
				CacheValueFromOtherDevices();
			}
			else
			{
				thisDeviceCurrencyValue = new CurrencyValue();
			}
		}

		public void Reset()
		{
			DataManager.ResetSyncableCurrency(Key);
			Load();
		}

		private void CacheValueFromOtherDevices()
		{
			otherDevicesValueCache = 0f;
			foreach (KeyValuePair<string, CurrencyValue> deviceCurrencyValue in deviceCurrencyValues)
			{
				if (!(deviceCurrencyValue.Key == DeviceID))
				{
					otherDevicesValueCache += deviceCurrencyValue.Value.Value;
				}
			}
		}
	}
}
