using System.Threading;
using Amazon.Runtime.Internal;
using UnityEngine;

namespace Amazon.Util.Storage.Internal
{
	public class PlayerPreferenceKVStore : KVStore
	{
		public override void Clear(string key)
		{
			if (UnityInitializer.IsMainThread())
			{
				ClearHelper(key);
				return;
			}
			UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
			{
				ClearHelper(key);
			});
		}

		public override void Put(string key, string value)
		{
			if (UnityInitializer.IsMainThread())
			{
				PutHelper(key, value);
				return;
			}
			AutoResetEvent asyncEvent = new AutoResetEvent(false);
			UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
			{
				PutHelper(key, value);
				asyncEvent.Set();
			});
			asyncEvent.WaitOne();
		}

		public override string Get(string key)
		{
			if (UnityInitializer.IsMainThread())
			{
				return GetHelper(key);
			}
			string value = string.Empty;
			AutoResetEvent asyncEvent = new AutoResetEvent(false);
			UnityRequestQueue.Instance.ExecuteOnMainThread(delegate
			{
				value = GetHelper(key);
				asyncEvent.Set();
			});
			asyncEvent.WaitOne();
			return value;
		}

		private void PutHelper(string key, string value)
		{
			PlayerPrefs.SetString(key, value);
			PlayerPrefs.Save();
		}

		private void ClearHelper(string key)
		{
			PlayerPrefs.DeleteKey(key);
			PlayerPrefs.Save();
		}

		private string GetHelper(string key)
		{
			string result = string.Empty;
			if (PlayerPrefs.HasKey(key))
			{
				result = PlayerPrefs.GetString(key);
			}
			return result;
		}
	}
}
