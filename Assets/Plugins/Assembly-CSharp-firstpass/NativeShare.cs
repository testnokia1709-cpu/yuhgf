using UnityEngine;

public static class NativeShare
{
	public static void Share(string body, string filePath = null, string url = null, string subject = "", string mimeType = "text/html", bool chooser = false, string chooserText = "Select sharing app")
	{
		ShareMultiple(body, new string[1] { filePath }, url, subject, mimeType, chooser);
	}

	public static void ShareMultiple(string body, string[] filePaths = null, string url = null, string subject = "", string mimeType = "text/html", bool chooser = false, string chooserText = "Select sharing app")
	{
		ShareAndroid(body, subject, url, filePaths, mimeType, chooser, chooserText);
	}

	public static void ShareAndroid(string body, string subject, string url, string[] filePaths, string mimeType, bool chooser, string chooserText)
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.content.Intent"))
				{
					using (AndroidJavaObject androidJavaObject2 = new AndroidJavaObject("android.content.Intent"))
					{
						using (androidJavaObject2.Call<AndroidJavaObject>("setAction", new object[1] { androidJavaClass2.GetStatic<string>("ACTION_SEND") }))
						{
						}
						using (androidJavaObject2.Call<AndroidJavaObject>("setType", new object[1] { mimeType }))
						{
						}
						using (androidJavaObject2.Call<AndroidJavaObject>("putExtra", new object[2]
						{
							androidJavaClass2.GetStatic<string>("EXTRA_SUBJECT"),
							subject
						}))
						{
						}
						using (androidJavaObject2.Call<AndroidJavaObject>("putExtra", new object[2]
						{
							androidJavaClass2.GetStatic<string>("EXTRA_TEXT"),
							body
						}))
						{
						}
						if (!string.IsNullOrEmpty(url))
						{
							using (AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("android.net.Uri"))
							{
								using (AndroidJavaObject androidJavaObject3 = androidJavaClass3.CallStatic<AndroidJavaObject>("parse", new object[1] { url }))
								{
									using (androidJavaObject2.Call<AndroidJavaObject>("putExtra", new object[2]
									{
										androidJavaClass2.GetStatic<string>("EXTRA_STREAM"),
										androidJavaObject3
									}))
									{
									}
								}
							}
						}
						else if (filePaths != null)
						{
							using (AndroidJavaClass androidJavaClass4 = new AndroidJavaClass("android.support.v4.content.FileProvider"))
							{
								using (AndroidJavaObject androidJavaObject4 = androidJavaObject.Call<AndroidJavaObject>("getApplicationContext", new object[0]))
								{
									using (new AndroidJavaClass("android.net.Uri"))
									{
										using (new AndroidJavaObject("java.util.ArrayList"))
										{
											string text = androidJavaObject4.Call<string>("getPackageName", new object[0]);
											string text2 = text + ".provider";
											AndroidJavaObject androidJavaObject5 = new AndroidJavaObject("java.io.File", filePaths[0]);
											AndroidJavaObject androidJavaObject6 = androidJavaClass4.CallStatic<AndroidJavaObject>("getUriForFile", new object[3] { androidJavaObject4, text2, androidJavaObject5 });
											int num = androidJavaObject2.GetStatic<int>("FLAG_GRANT_READ_URI_PERMISSION");
											androidJavaObject2.Call<AndroidJavaObject>("addFlags", new object[1] { num });
											using (androidJavaObject2.Call<AndroidJavaObject>("putExtra", new object[2]
											{
												androidJavaClass2.GetStatic<string>("EXTRA_STREAM"),
												androidJavaObject6
											}))
											{
											}
										}
									}
								}
							}
						}
						if (chooser)
						{
							AndroidJavaObject androidJavaObject7 = androidJavaClass2.CallStatic<AndroidJavaObject>("createChooser", new object[2] { androidJavaObject2, chooserText });
							androidJavaObject.Call("startActivity", androidJavaObject7);
						}
						else
						{
							androidJavaObject.Call("startActivity", androidJavaObject2);
						}
					}
				}
			}
		}
	}
}
