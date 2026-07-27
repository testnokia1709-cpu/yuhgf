using System;
using UnityEngine;

public class StoragePath
{
	public static string GetInternalStoragePath()
	{
		string result = Application.persistentDataPath;
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.orbital.androidplugin.StoragePath"))
					{
						result = androidJavaClass2.CallStatic<string>("GetInternalStoragePath", new object[1] { androidJavaObject });
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return result;
	}

	public static string GetExternalStoragePath()
	{
		string result = Application.persistentDataPath;
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.oribtal.AndroidPlugin.StoragePath"))
					{
						result = androidJavaClass2.CallStatic<string>("GetExternalStoragePath", new object[1] { androidJavaObject });
					}
				}
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return result;
	}
}
