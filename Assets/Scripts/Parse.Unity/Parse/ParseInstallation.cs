using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;
using UnityEngine.iOS;

namespace Parse
{
	[ParseClassName("_Installation")]
	public class ParseInstallation : ParseObject
	{
		private static readonly HashSet<string> readOnlyKeys = new HashSet<string> { "deviceType", "deviceUris", "installationId", "timeZone", "localeIdentifier", "parseVersion", "appName", "appIdentifier", "appVersion", "pushType" };

		internal static readonly Dictionary<string, string> TimeZoneNameMap = new Dictionary<string, string>
		{
			{ "Dateline Standard Time", "Etc/GMT+12" },
			{ "UTC-11", "Etc/GMT+11" },
			{ "Hawaiian Standard Time", "Pacific/Honolulu" },
			{ "Alaskan Standard Time", "America/Anchorage" },
			{ "Pacific Standard Time (Mexico)", "America/Santa_Isabel" },
			{ "Pacific Standard Time", "America/Los_Angeles" },
			{ "US Mountain Standard Time", "America/Phoenix" },
			{ "Mountain Standard Time (Mexico)", "America/Chihuahua" },
			{ "Mountain Standard Time", "America/Denver" },
			{ "Central America Standard Time", "America/Guatemala" },
			{ "Central Standard Time", "America/Chicago" },
			{ "Central Standard Time (Mexico)", "America/Mexico_City" },
			{ "Canada Central Standard Time", "America/Regina" },
			{ "SA Pacific Standard Time", "America/Bogota" },
			{ "Eastern Standard Time", "America/New_York" },
			{ "US Eastern Standard Time", "America/Indianapolis" },
			{ "Venezuela Standard Time", "America/Caracas" },
			{ "Paraguay Standard Time", "America/Asuncion" },
			{ "Atlantic Standard Time", "America/Halifax" },
			{ "Central Brazilian Standard Time", "America/Cuiaba" },
			{ "SA Western Standard Time", "America/La_Paz" },
			{ "Pacific SA Standard Time", "America/Santiago" },
			{ "Newfoundland Standard Time", "America/St_Johns" },
			{ "E. South America Standard Time", "America/Sao_Paulo" },
			{ "Argentina Standard Time", "America/Buenos_Aires" },
			{ "SA Eastern Standard Time", "America/Cayenne" },
			{ "Greenland Standard Time", "America/Godthab" },
			{ "Montevideo Standard Time", "America/Montevideo" },
			{ "Bahia Standard Time", "America/Bahia" },
			{ "UTC-02", "Etc/GMT+2" },
			{ "Azores Standard Time", "Atlantic/Azores" },
			{ "Cape Verde Standard Time", "Atlantic/Cape_Verde" },
			{ "Morocco Standard Time", "Africa/Casablanca" },
			{ "UTC", "Etc/GMT" },
			{ "GMT Standard Time", "Europe/London" },
			{ "Greenwich Standard Time", "Atlantic/Reykjavik" },
			{ "W. Europe Standard Time", "Europe/Berlin" },
			{ "Central Europe Standard Time", "Europe/Budapest" },
			{ "Romance Standard Time", "Europe/Paris" },
			{ "Central European Standard Time", "Europe/Warsaw" },
			{ "W. Central Africa Standard Time", "Africa/Lagos" },
			{ "Namibia Standard Time", "Africa/Windhoek" },
			{ "GTB Standard Time", "Europe/Bucharest" },
			{ "Middle East Standard Time", "Asia/Beirut" },
			{ "Egypt Standard Time", "Africa/Cairo" },
			{ "Syria Standard Time", "Asia/Damascus" },
			{ "E. Europe Standard Time", "Asia/Nicosia" },
			{ "South Africa Standard Time", "Africa/Johannesburg" },
			{ "FLE Standard Time", "Europe/Kiev" },
			{ "Turkey Standard Time", "Europe/Istanbul" },
			{ "Israel Standard Time", "Asia/Jerusalem" },
			{ "Jordan Standard Time", "Asia/Amman" },
			{ "Arabic Standard Time", "Asia/Baghdad" },
			{ "Kaliningrad Standard Time", "Europe/Kaliningrad" },
			{ "Arab Standard Time", "Asia/Riyadh" },
			{ "E. Africa Standard Time", "Africa/Nairobi" },
			{ "Iran Standard Time", "Asia/Tehran" },
			{ "Arabian Standard Time", "Asia/Dubai" },
			{ "Azerbaijan Standard Time", "Asia/Baku" },
			{ "Russian Standard Time", "Europe/Moscow" },
			{ "Mauritius Standard Time", "Indian/Mauritius" },
			{ "Georgian Standard Time", "Asia/Tbilisi" },
			{ "Caucasus Standard Time", "Asia/Yerevan" },
			{ "Afghanistan Standard Time", "Asia/Kabul" },
			{ "Pakistan Standard Time", "Asia/Karachi" },
			{ "West Asia Standard Time", "Asia/Tashkent" },
			{ "India Standard Time", "Asia/Calcutta" },
			{ "Sri Lanka Standard Time", "Asia/Colombo" },
			{ "Nepal Standard Time", "Asia/Katmandu" },
			{ "Central Asia Standard Time", "Asia/Almaty" },
			{ "Bangladesh Standard Time", "Asia/Dhaka" },
			{ "Ekaterinburg Standard Time", "Asia/Yekaterinburg" },
			{ "Myanmar Standard Time", "Asia/Rangoon" },
			{ "SE Asia Standard Time", "Asia/Bangkok" },
			{ "N. Central Asia Standard Time", "Asia/Novosibirsk" },
			{ "China Standard Time", "Asia/Shanghai" },
			{ "North Asia Standard Time", "Asia/Krasnoyarsk" },
			{ "Singapore Standard Time", "Asia/Singapore" },
			{ "W. Australia Standard Time", "Australia/Perth" },
			{ "Taipei Standard Time", "Asia/Taipei" },
			{ "Ulaanbaatar Standard Time", "Asia/Ulaanbaatar" },
			{ "North Asia East Standard Time", "Asia/Irkutsk" },
			{ "Tokyo Standard Time", "Asia/Tokyo" },
			{ "Korea Standard Time", "Asia/Seoul" },
			{ "Cen. Australia Standard Time", "Australia/Adelaide" },
			{ "AUS Central Standard Time", "Australia/Darwin" },
			{ "E. Australia Standard Time", "Australia/Brisbane" },
			{ "AUS Eastern Standard Time", "Australia/Sydney" },
			{ "West Pacific Standard Time", "Pacific/Port_Moresby" },
			{ "Tasmania Standard Time", "Australia/Hobart" },
			{ "Yakutsk Standard Time", "Asia/Yakutsk" },
			{ "Central Pacific Standard Time", "Pacific/Guadalcanal" },
			{ "Vladivostok Standard Time", "Asia/Vladivostok" },
			{ "New Zealand Standard Time", "Pacific/Auckland" },
			{ "UTC+12", "Etc/GMT-12" },
			{ "Fiji Standard Time", "Pacific/Fiji" },
			{ "Magadan Standard Time", "Asia/Magadan" },
			{ "Tonga Standard Time", "Pacific/Tongatapu" },
			{ "Samoa Standard Time", "Pacific/Apia" }
		};

		internal static readonly Dictionary<TimeSpan, string> TimeZoneOffsetMap = new Dictionary<TimeSpan, string>
		{
			{
				new TimeSpan(12, 45, 0),
				"Pacific/Chatham"
			},
			{
				new TimeSpan(10, 30, 0),
				"Australia/Lord_Howe"
			},
			{
				new TimeSpan(9, 30, 0),
				"Australia/Adelaide"
			},
			{
				new TimeSpan(8, 45, 0),
				"Australia/Eucla"
			},
			{
				new TimeSpan(8, 30, 0),
				"Asia/Pyongyang"
			},
			{
				new TimeSpan(6, 30, 0),
				"Asia/Rangoon"
			},
			{
				new TimeSpan(5, 45, 0),
				"Asia/Kathmandu"
			},
			{
				new TimeSpan(5, 30, 0),
				"Asia/Colombo"
			},
			{
				new TimeSpan(4, 30, 0),
				"Asia/Kabul"
			},
			{
				new TimeSpan(3, 30, 0),
				"Asia/Tehran"
			},
			{
				new TimeSpan(-3, 30, 0),
				"America/St_Johns"
			},
			{
				new TimeSpan(-4, 30, 0),
				"America/Caracas"
			},
			{
				new TimeSpan(-9, 30, 0),
				"Pacific/Marquesas"
			}
		};

		internal static IParseCurrentInstallationController CurrentInstallationController
		{
			get
			{
				return ParseCorePlugins.Instance.CurrentInstallationController;
			}
		}

		public static ParseInstallation CurrentInstallation
		{
			get
			{
				Task<ParseInstallation> async = CurrentInstallationController.GetAsync(CancellationToken.None);
				async.Wait();
				return async.Result;
			}
		}

		public static ParseQuery<ParseInstallation> Query
		{
			get
			{
				return new ParseQuery<ParseInstallation>();
			}
		}

		[ParseFieldName("installationId")]
		public Guid InstallationId
		{
			get
			{
				string property = GetProperty<string>("InstallationId");
				Guid? guid = null;
				try
				{
					guid = new Guid(property);
				}
				catch (Exception)
				{
				}
				return guid.Value;
			}
			internal set
			{
				Guid guid = value;
				SetProperty(guid.ToString(), "InstallationId");
			}
		}

		[ParseFieldName("deviceType")]
		public string DeviceType
		{
			get
			{
				return GetProperty<string>("DeviceType");
			}
			internal set
			{
				SetProperty(value, "DeviceType");
			}
		}

		[ParseFieldName("appName")]
		public string AppName
		{
			get
			{
				return GetProperty<string>("AppName");
			}
			internal set
			{
				SetProperty(value, "AppName");
			}
		}

		[ParseFieldName("appVersion")]
		public string AppVersion
		{
			get
			{
				return GetProperty<string>("AppVersion");
			}
			internal set
			{
				SetProperty(value, "AppVersion");
			}
		}

		[ParseFieldName("appIdentifier")]
		public string AppIdentifier
		{
			get
			{
				return GetProperty<string>("AppIdentifier");
			}
			internal set
			{
				SetProperty(value, "AppIdentifier");
			}
		}

		[ParseFieldName("timeZone")]
		public string TimeZone
		{
			get
			{
				return GetProperty<string>("TimeZone");
			}
			private set
			{
				SetProperty(value, "TimeZone");
			}
		}

		[ParseFieldName("localeIdentifier")]
		public string LocaleIdentifier
		{
			get
			{
				return GetProperty<string>("LocaleIdentifier");
			}
			private set
			{
				SetProperty(value, "LocaleIdentifier");
			}
		}

		[ParseFieldName("parseVersion")]
		public Version ParseVersion
		{
			get
			{
				string property = GetProperty<string>("ParseVersion");
				Version result = null;
				try
				{
					result = new Version(property);
				}
				catch (Exception)
				{
				}
				return result;
			}
			private set
			{
				SetProperty(value.ToString(), "ParseVersion");
			}
		}

		[ParseFieldName("channels")]
		public IList<string> Channels
		{
			get
			{
				return GetProperty<IList<string>>("Channels");
			}
			set
			{
				SetProperty(value, "Channels");
			}
		}

		[ParseFieldName("deviceToken")]
		public string DeviceToken
		{
			get
			{
				return GetProperty<string>("DeviceToken");
			}
			internal set
			{
				SetProperty(value, "DeviceToken");
			}
		}

		[ParseFieldName("badge")]
		public int Badge
		{
			get
			{
				if (PlatformHooks.IsIOS)
				{
					PlatformHooks.RunOnMainThread(delegate
					{
						if (NotificationServices.localNotificationCount > 0)
						{
							SetProperty(NotificationServices.localNotifications[0].applicationIconBadgeNumber, "Badge");
						}
					});
				}
				return GetProperty<int>("Badge");
			}
			set
			{
				SetProperty(value, "Badge");
				if (PlatformHooks.IsIOS)
				{
					PlatformHooks.RunOnMainThread(delegate
					{
						//IL_0000: Unknown result type (might be due to invalid IL or missing references)
						//IL_0005: Unknown result type (might be due to invalid IL or missing references)
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_001d: Expected O, but got Unknown
						NotificationServices.PresentLocalNotificationNow(new LocalNotification
						{
							applicationIconBadgeNumber = value,
							hasAction = false
						});
					});
				}
			}
		}

		internal static void ClearInMemoryInstallation()
		{
			CurrentInstallationController.ClearFromMemory();
		}

		private string GetLocaleIdentifier()
		{
			string text = null;
			string text2 = null;
			if (CultureInfo.CurrentCulture != null)
			{
				text = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
			}
			if (RegionInfo.CurrentRegion != null)
			{
				text2 = RegionInfo.CurrentRegion.TwoLetterISORegionName;
			}
			if (string.IsNullOrEmpty(text2))
			{
				return text;
			}
			return string.Format("{0}-{1}", text, text2);
		}

		private Version GetParseVersion()
		{
			return ParseClient.Version;
		}

		internal override bool IsKeyMutable(string key)
		{
			return !readOnlyKeys.Contains(key);
		}

		internal override Task SaveAsync(Task toAwait, CancellationToken cancellationToken)
		{
			Task task = null;
			if (CurrentInstallationController.IsCurrent(this))
			{
				SetIfDifferent("deviceType", ParseClient.PlatformHooks.DeviceType);
				SetIfDifferent("timeZone", ParseClient.PlatformHooks.DeviceTimeZone);
				SetIfDifferent("localeIdentifier", GetLocaleIdentifier());
				SetIfDifferent("parseVersion", GetParseVersion().ToString());
				SetIfDifferent("appVersion", ParseClient.PlatformHooks.AppBuildVersion);
				SetIfDifferent("appIdentifier", ParseClient.PlatformHooks.AppIdentifier);
				SetIfDifferent("appName", ParseClient.PlatformHooks.AppName);
				task = ParseClient.PlatformHooks.ExecuteParseInstallationSaveHookAsync(this);
			}
			return task.Safe().OnSuccess((Task _) => _003C_003En__0(toAwait, cancellationToken)).Unwrap()
				.OnSuccess((Task _) => CurrentInstallationController.IsCurrent(this) ? Task.FromResult(0) : CurrentInstallationController.SetAsync(this, cancellationToken))
				.Unwrap();
		}

		public void SetDeviceTokenFromData(byte[] deviceToken)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (byte b in deviceToken)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			DeviceToken = stringBuilder.ToString();
		}

		[CompilerGenerated]
		[DebuggerHidden]
		private Task _003C_003En__0(Task toAwait, CancellationToken cancellationToken)
		{
			return base.SaveAsync(toAwait, cancellationToken);
		}
	}
}
