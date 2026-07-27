using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace CloudOnce.Internal.Utils
{
	public static class CloudOnceUtils
	{
		public static IAchievementUtils AchievementUtils { get; private set; }

		public static ILeaderboardUtils LeaderboardUtils { get; private set; }

		static CloudOnceUtils()
		{
			AchievementUtils = new GoogleAchievementUtils();
			LeaderboardUtils = new GoogleLeaderboardUtils();
		}

		public static string ToBase64String(this string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			byte[] bytes = Encoding.Default.GetBytes(str);
			return Convert.ToBase64String(bytes);
		}

		public static string FromBase64StringToString(this string base64String)
		{
			if (base64String == null)
			{
				base64String = string.Empty;
			}
			byte[] bytes = Convert.FromBase64String(base64String);
			return Encoding.Default.GetString(bytes);
		}

		public static IEnumerator InvokeUnscaledTime(UnityAction callback, float time)
		{
			if (callback != null)
			{
				float startTime = Time.unscaledTime;
				while (Time.unscaledTime - startTime < time)
				{
					yield return null;
				}
				callback();
			}
		}

		public static IEnumerator InvokeUnscaledTime<T>(UnityAction<T> callback, T parameter, float time)
		{
			if (callback != null)
			{
				float startTime = Time.unscaledTime;
				while (Time.unscaledTime - startTime < time)
				{
					yield return null;
				}
				callback(parameter);
			}
		}

		public static void SafeInvoke(Action action)
		{
			if (action != null)
			{
				action();
			}
		}

		public static void SafeInvoke(UnityAction unityAction)
		{
			if (unityAction != null)
			{
				unityAction();
			}
		}

		public static void SafeInvoke<T>(Action<T> action, T param)
		{
			if (action != null)
			{
				action(param);
			}
		}

		public static void SafeInvoke<T>(UnityAction<T> unityAction, T param)
		{
			if (unityAction != null)
			{
				unityAction(param);
			}
		}

		public static bool IsJson(this string input)
		{
			input = input.TrimStart();
			return input.StartsWith("{") || input.StartsWith("[");
		}

		public static string GetAlias(string className, JSONObject jsonObject, params string[] aliases)
		{
			foreach (string text in aliases)
			{
				if (jsonObject.HasFields(text))
				{
					return text;
				}
			}
			throw new SerializationException("JSONObject missing fields, cannot deserialize to " + className);
		}
	}
}
