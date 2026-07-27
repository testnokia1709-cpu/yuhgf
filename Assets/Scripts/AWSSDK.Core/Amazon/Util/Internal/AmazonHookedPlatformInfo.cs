using System;
using Amazon.Runtime.Internal.Util;
using ThirdParty.iOS4Unity;
using UnityEngine;

namespace Amazon.Util.Internal
{
	public class AmazonHookedPlatformInfo
	{
		private static Amazon.Runtime.Internal.Util.Logger _logger = Amazon.Runtime.Internal.Util.Logger.GetLogger(typeof(AmazonHookedPlatformInfo));

		private const string IPHONE_OS = "iPhone OS";

		private const string ANDROID_OS = "Android";

		private static AmazonHookedPlatformInfo instance = null;

		private string device_platform;

		private string device_model;

		private string device_make;

		private string device_platformVersion;

		private string device_locale;

		private string app_version_name;

		private string app_version_code;

		private string app_package_name;

		private string app_title;

		public string Platform
		{
			get
			{
				return device_platform;
			}
			internal set
			{
				if (value.Equals(RuntimePlatform.IPhonePlayer.ToString(), StringComparison.OrdinalIgnoreCase) || value.Contains("iPhoneOS") || value.Contains("iPhone"))
				{
					device_platform = "iPhone OS";
				}
				else if (value.Equals(RuntimePlatform.Android.ToString(), StringComparison.OrdinalIgnoreCase) || value.Contains("Android") || value.Contains("android"))
				{
					device_platform = "Android";
				}
				else
				{
					device_platform = value;
				}
			}
		}

		public string Model
		{
			get
			{
				return device_model;
			}
			internal set
			{
				device_model = value;
			}
		}

		public string Make
		{
			get
			{
				return device_make;
			}
			internal set
			{
				device_make = value;
			}
		}

		public string PlatformVersion
		{
			get
			{
				return device_platformVersion;
			}
			internal set
			{
				device_platformVersion = value;
			}
		}

		public string PersistentDataPath { get; set; }

		public string UnityVersion { get; private set; }

		public string Locale
		{
			get
			{
				return device_locale;
			}
			set
			{
				device_locale = value;
			}
		}

		public static AmazonHookedPlatformInfo Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AmazonHookedPlatformInfo();
					instance.Init();
				}
				return instance;
			}
		}

		public string PackageName
		{
			get
			{
				return app_package_name;
			}
			internal set
			{
				app_package_name = value;
			}
		}

		public string VersionName
		{
			get
			{
				return app_version_name;
			}
			internal set
			{
				app_version_name = value;
			}
		}

		public string VersionCode
		{
			get
			{
				return app_version_code;
			}
			internal set
			{
				app_version_code = value;
			}
		}

		public string Title
		{
			get
			{
				return app_title;
			}
			internal set
			{
				app_title = value;
			}
		}

		private AmazonHookedPlatformInfo()
		{
		}

		public void Init()
		{
			PersistentDataPath = Application.persistentDataPath;
			UnityVersion = Application.unityVersion;
			if (InternalSDKUtils.IsAndroid)
			{
				PlatformVersion = AndroidInterop.GetStaticJavaField<string>("android.os.Build$VERSION", "RELEASE");
				Platform = "Android";
				Model = AndroidInterop.GetStaticJavaField<string>("android.os.Build", "MODEL");
				Make = AndroidInterop.GetStaticJavaField<string>("android.os.Build", "MANUFACTURER");
				object javaObjectStatically = AndroidInterop.GetJavaObjectStatically("java.util.Locale", "getDefault");
				Locale = AndroidInterop.CallMethod<string>(javaObjectStatically, "toString", new object[0]);
				object androidContext = AndroidInterop.GetAndroidContext();
				PackageName = AndroidInterop.CallMethod<string>(androidContext, "getPackageName", new object[0]);
				object androidJavaObject = AndroidInterop.CallMethod(androidContext, "getPackageManager");
				object androidJavaObject2 = AndroidInterop.CallMethod(androidJavaObject, "getPackageInfo", PackageName, 0);
				object obj = AndroidInterop.CallMethod(androidJavaObject, "getApplicationInfo", PackageName, 0);
				VersionCode = Convert.ToString(AndroidInterop.GetJavaField<int>(androidJavaObject2, "versionCode"));
				VersionName = AndroidInterop.GetJavaField<string>(androidJavaObject2, "versionName");
				Title = AndroidInterop.CallMethod<string>(androidJavaObject, "getApplicationLabel", new object[1] { obj });
			}
			else
			{
				if (!InternalSDKUtils.IsiOS)
				{
					return;
				}
				if (!string.IsNullOrEmpty(NSLocale.AutoUpdatingCurrentLocale.Identifier))
				{
					Locale = NSLocale.AutoUpdatingCurrentLocale.Identifier;
				}
				else
				{
					Locale = NSLocale.AutoUpdatingCurrentLocale.LocaleIdentifier;
				}
				using (UIDevice uIDevice = UIDevice.CurrentDevice)
				{
					Platform = uIDevice.SystemName;
					PlatformVersion = uIDevice.SystemVersion;
					Model = uIDevice.Model;
				}
				Make = "apple";
				using (NSBundle nSBundle = NSBundle.MainBundle)
				{
					using (NSDictionary nSDictionary = ThirdParty.iOS4Unity.Runtime.GetNSObject<NSDictionary>(ObjC.MessageSendIntPtr(nSBundle.Handle, Selector.GetHandle("infoDictionary"))))
					{
						Title = nSDictionary.ObjectForKey("CFBundleDisplayName").ToString();
						VersionCode = nSDictionary.ObjectForKey("CFBundleVersion").ToString();
						VersionName = nSDictionary.ObjectForKey("CFBundleShortVersionString").ToString();
						PackageName = nSDictionary.ObjectForKey("CFBundleIdentifier").ToString();
					}
				}
			}
		}
	}
}
