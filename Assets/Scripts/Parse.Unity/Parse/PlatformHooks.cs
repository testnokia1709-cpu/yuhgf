using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;
using UnityEngine;
using UnityEngine.iOS;

namespace Parse
{
	internal class PlatformHooks : IPlatformHooks
	{
		private class SettingsWrapper : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IEnumerable
		{
			private readonly IDictionary<string, object> data;

			private static SettingsWrapper wrapper;

			public static SettingsWrapper Wrapper
			{
				get
				{
					wrapper = wrapper ?? new SettingsWrapper();
					return wrapper;
				}
			}

			public ICollection<string> Keys
			{
				get
				{
					return data.Keys;
				}
			}

			public ICollection<object> Values
			{
				get
				{
					return data.Values;
				}
			}

			public object this[string key]
			{
				get
				{
					return data[key];
				}
				set
				{
					data[key] = value;
					Save();
				}
			}

			public int Count
			{
				get
				{
					return data.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return data.IsReadOnly;
				}
			}

			private SettingsWrapper()
			{
				string text = Load();
				if (string.IsNullOrEmpty(text))
				{
					data = new Dictionary<string, object>();
					Save();
				}
				else
				{
					data = ParseClient.DeserializeJsonString(text);
				}
			}

			private string Load()
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				lock (this)
				{
					try
					{
						if (IsWebPlayer)
						{
							return PlayerPrefs.GetString("Parse.settings", null);
						}
						if (IsTvOS)
						{
							Debug.Log("Running on TvOS, prefs cannot be loaded.");
							return null;
						}
						using (FileStream stream = new FileStream(settingsPath, FileMode.Open, FileAccess.Read))
						{
							return new StreamReader(stream).ReadToEnd();
						}
					}
					catch (Exception)
					{
						return null;
					}
				}
			}

			private void Save()
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				lock (this)
				{
					if (IsWebPlayer)
					{
						PlayerPrefs.SetString("Parse.settings", ParseClient.SerializeJsonString(data));
						PlayerPrefs.Save();
						return;
					}
					if (IsTvOS)
					{
						Debug.Log("Running on TvOS, prefs cannot be saved.");
						return;
					}
					using (FileStream stream = new FileStream(settingsPath, FileMode.Create, FileAccess.Write))
					{
						using (StreamWriter streamWriter = new StreamWriter(stream))
						{
							streamWriter.Write(ParseClient.SerializeJsonString(data));
						}
					}
				}
			}

			public void Add(string key, object value)
			{
				data.Add(key, value);
				Save();
			}

			public bool ContainsKey(string key)
			{
				return data.ContainsKey(key);
			}

			public bool Remove(string key)
			{
				if (data.Remove(key))
				{
					Save();
					return true;
				}
				return false;
			}

			public bool TryGetValue(string key, out object value)
			{
				return data.TryGetValue(key, out value);
			}

			public void Add(KeyValuePair<string, object> item)
			{
				data.Add(item);
				Save();
			}

			public void Clear()
			{
				data.Clear();
				Save();
			}

			public bool Contains(KeyValuePair<string, object> item)
			{
				return data.Contains(item);
			}

			public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
			{
				data.CopyTo(array, arrayIndex);
			}

			public bool Remove(KeyValuePair<string, object> item)
			{
				if (data.Remove(item))
				{
					Save();
					return true;
				}
				return false;
			}

			public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
			{
				return data.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return data.GetEnumerator();
			}
		}

		private static IDictionary<string, object> settings;

		private static string settingsPath;

		private IHttpClient httpClient;

		private string appName;

		private string appBuildVersion;

		private string appDisplayVersion;

		private string osVersion;

		private static bool isCompiledByIL2CPP = AppDomain.CurrentDomain.FriendlyName.Equals("IL2CPP Root Domain");

		private static bool isWebPlayer;

		private static readonly ReaderWriterLockSlim dispatchQueueLock = new ReaderWriterLockSlim();

		private static readonly Queue<Action> dispatchQueue = new Queue<Action>();

		public IHttpClient HttpClient
		{
			get
			{
				httpClient = httpClient ?? new HttpClient();
				return httpClient;
			}
		}

		public string SDKName
		{
			get
			{
				return "unity";
			}
		}

		public string AppName
		{
			get
			{
				return appName;
			}
		}

		public string AppBuildVersion
		{
			get
			{
				return appBuildVersion;
			}
		}

		public string AppDisplayVersion
		{
			get
			{
				return appDisplayVersion;
			}
		}

		public string AppIdentifier
		{
			get
			{
				ApplicationIdentity applicationIdentity = AppDomain.CurrentDomain.ApplicationIdentity;
				if (applicationIdentity == null)
				{
					return null;
				}
				return applicationIdentity.FullName;
			}
		}

		public string OSVersion
		{
			get
			{
				return osVersion;
			}
		}

		public string DeviceType
		{
			get
			{
				if (IsAndroid)
				{
					return "android";
				}
				if (IsIOS)
				{
					return "ios";
				}
				if (IsWindowsPhone8)
				{
					return "winphone";
				}
				return "unknown";
			}
		}

		public string DeviceTimeZone
		{
			get
			{
				try
				{
					TimeZoneInfo local = TimeZoneInfo.Local;
					string value = null;
					if (ParseInstallation.TimeZoneNameMap.TryGetValue(local.StandardName, out value))
					{
						return value;
					}
					TimeSpan baseUtcOffset = local.BaseUtcOffset;
					if (ParseInstallation.TimeZoneOffsetMap.TryGetValue(baseUtcOffset, out value))
					{
						return value;
					}
					bool flag = baseUtcOffset.Ticks < 0;
					return string.Format("Etc/GMT{0}{1}", flag ? "+" : "-", Math.Abs(baseUtcOffset.Hours));
				}
				catch (TimeZoneNotFoundException)
				{
					return null;
				}
			}
		}

		internal static bool IsCompiledByIL2CPP
		{
			get
			{
				return isCompiledByIL2CPP;
			}
		}

		internal static bool IsWebPlayer
		{
			get
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				return isWebPlayer;
			}
		}

		internal static bool IsAndroid
		{
			get
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				return Application.platform == RuntimePlatform.Android;
			}
		}

		internal static bool IsIOS
		{
			get
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				return Application.platform == RuntimePlatform.IPhonePlayer;
			}
		}

		internal static bool IsTvOS
		{
			get
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				return Application.platform == RuntimePlatform.tvOS;
			}
		}

		internal static bool IsWindowsPhone8
		{
			get
			{
				if (settingsPath == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				return Application.platform == RuntimePlatform.WP8Player;
			}
		}

		public IDictionary<string, object> ApplicationSettings
		{
			get
			{
				if (settings == null)
				{
					throw new InvalidOperationException("Parse must be initialized before making any calls.");
				}
				return settings;
			}
		}

		internal static Type GetTypeFromUnityEngine(string typeName)
		{
			return Type.GetType(string.Format("UnityEngine.{0}, UnityEngine", typeName));
		}

		internal static void CallStaticJavaUnityMethod(string className, string methodName, object[] parameters)
		{
			Type typeFromUnityEngine = GetTypeFromUnityEngine("AndroidJavaClass");
			if (typeFromUnityEngine != null)
			{
				object obj = Activator.CreateInstance(typeFromUnityEngine, className);
				MethodInfo methodInfo = (from x in typeFromUnityEngine.GetMethods()
					where x.Name == "CallStatic"
					select x).First((MethodInfo x) => !x.ContainsGenericParameters);
				if (methodInfo != null)
				{
					methodInfo.Invoke(obj, new object[2] { methodName, parameters });
				}
			}
		}

		private static List<object> CreateWrapperTypes()
		{
			return new List<object>
			{
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<object>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<bool>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<byte>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<sbyte>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<short>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<ushort>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<int>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<uint>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<long>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<ulong>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<char>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<double>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<float>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<IDictionary<string, object>>(null, null, CancellationToken.None);
				},
				(Action)delegate
				{
					ParseCloud.CallFunctionAsync<IList<object>>(null, null, CancellationToken.None);
				},
				typeof(FlexibleListWrapper<object, object>),
				typeof(FlexibleListWrapper<object, bool>),
				typeof(FlexibleListWrapper<object, byte>),
				typeof(FlexibleListWrapper<object, sbyte>),
				typeof(FlexibleListWrapper<object, short>),
				typeof(FlexibleListWrapper<object, ushort>),
				typeof(FlexibleListWrapper<object, int>),
				typeof(FlexibleListWrapper<object, uint>),
				typeof(FlexibleListWrapper<object, long>),
				typeof(FlexibleListWrapper<object, ulong>),
				typeof(FlexibleListWrapper<object, char>),
				typeof(FlexibleListWrapper<object, double>),
				typeof(FlexibleListWrapper<object, float>),
				typeof(FlexibleListWrapper<bool, object>),
				typeof(FlexibleListWrapper<bool, bool>),
				typeof(FlexibleListWrapper<bool, byte>),
				typeof(FlexibleListWrapper<bool, sbyte>),
				typeof(FlexibleListWrapper<bool, short>),
				typeof(FlexibleListWrapper<bool, ushort>),
				typeof(FlexibleListWrapper<bool, int>),
				typeof(FlexibleListWrapper<bool, uint>),
				typeof(FlexibleListWrapper<bool, long>),
				typeof(FlexibleListWrapper<bool, ulong>),
				typeof(FlexibleListWrapper<bool, char>),
				typeof(FlexibleListWrapper<bool, double>),
				typeof(FlexibleListWrapper<bool, float>),
				typeof(FlexibleListWrapper<byte, object>),
				typeof(FlexibleListWrapper<byte, bool>),
				typeof(FlexibleListWrapper<byte, byte>),
				typeof(FlexibleListWrapper<byte, sbyte>),
				typeof(FlexibleListWrapper<byte, short>),
				typeof(FlexibleListWrapper<byte, ushort>),
				typeof(FlexibleListWrapper<byte, int>),
				typeof(FlexibleListWrapper<byte, uint>),
				typeof(FlexibleListWrapper<byte, long>),
				typeof(FlexibleListWrapper<byte, ulong>),
				typeof(FlexibleListWrapper<byte, char>),
				typeof(FlexibleListWrapper<byte, double>),
				typeof(FlexibleListWrapper<byte, float>),
				typeof(FlexibleListWrapper<sbyte, object>),
				typeof(FlexibleListWrapper<sbyte, bool>),
				typeof(FlexibleListWrapper<sbyte, byte>),
				typeof(FlexibleListWrapper<sbyte, sbyte>),
				typeof(FlexibleListWrapper<sbyte, short>),
				typeof(FlexibleListWrapper<sbyte, ushort>),
				typeof(FlexibleListWrapper<sbyte, int>),
				typeof(FlexibleListWrapper<sbyte, uint>),
				typeof(FlexibleListWrapper<sbyte, long>),
				typeof(FlexibleListWrapper<sbyte, ulong>),
				typeof(FlexibleListWrapper<sbyte, char>),
				typeof(FlexibleListWrapper<sbyte, double>),
				typeof(FlexibleListWrapper<sbyte, float>),
				typeof(FlexibleListWrapper<short, object>),
				typeof(FlexibleListWrapper<short, bool>),
				typeof(FlexibleListWrapper<short, byte>),
				typeof(FlexibleListWrapper<short, sbyte>),
				typeof(FlexibleListWrapper<short, short>),
				typeof(FlexibleListWrapper<short, ushort>),
				typeof(FlexibleListWrapper<short, int>),
				typeof(FlexibleListWrapper<short, uint>),
				typeof(FlexibleListWrapper<short, long>),
				typeof(FlexibleListWrapper<short, ulong>),
				typeof(FlexibleListWrapper<short, char>),
				typeof(FlexibleListWrapper<short, double>),
				typeof(FlexibleListWrapper<short, float>),
				typeof(FlexibleListWrapper<ushort, object>),
				typeof(FlexibleListWrapper<ushort, bool>),
				typeof(FlexibleListWrapper<ushort, byte>),
				typeof(FlexibleListWrapper<ushort, sbyte>),
				typeof(FlexibleListWrapper<ushort, short>),
				typeof(FlexibleListWrapper<ushort, ushort>),
				typeof(FlexibleListWrapper<ushort, int>),
				typeof(FlexibleListWrapper<ushort, uint>),
				typeof(FlexibleListWrapper<ushort, long>),
				typeof(FlexibleListWrapper<ushort, ulong>),
				typeof(FlexibleListWrapper<ushort, char>),
				typeof(FlexibleListWrapper<ushort, double>),
				typeof(FlexibleListWrapper<ushort, float>),
				typeof(FlexibleListWrapper<int, object>),
				typeof(FlexibleListWrapper<int, bool>),
				typeof(FlexibleListWrapper<int, byte>),
				typeof(FlexibleListWrapper<int, sbyte>),
				typeof(FlexibleListWrapper<int, short>),
				typeof(FlexibleListWrapper<int, ushort>),
				typeof(FlexibleListWrapper<int, int>),
				typeof(FlexibleListWrapper<int, uint>),
				typeof(FlexibleListWrapper<int, long>),
				typeof(FlexibleListWrapper<int, ulong>),
				typeof(FlexibleListWrapper<int, char>),
				typeof(FlexibleListWrapper<int, double>),
				typeof(FlexibleListWrapper<int, float>),
				typeof(FlexibleListWrapper<uint, object>),
				typeof(FlexibleListWrapper<uint, bool>),
				typeof(FlexibleListWrapper<uint, byte>),
				typeof(FlexibleListWrapper<uint, sbyte>),
				typeof(FlexibleListWrapper<uint, short>),
				typeof(FlexibleListWrapper<uint, ushort>),
				typeof(FlexibleListWrapper<uint, int>),
				typeof(FlexibleListWrapper<uint, uint>),
				typeof(FlexibleListWrapper<uint, long>),
				typeof(FlexibleListWrapper<uint, ulong>),
				typeof(FlexibleListWrapper<uint, char>),
				typeof(FlexibleListWrapper<uint, double>),
				typeof(FlexibleListWrapper<uint, float>),
				typeof(FlexibleListWrapper<long, object>),
				typeof(FlexibleListWrapper<long, bool>),
				typeof(FlexibleListWrapper<long, byte>),
				typeof(FlexibleListWrapper<long, sbyte>),
				typeof(FlexibleListWrapper<long, short>),
				typeof(FlexibleListWrapper<long, ushort>),
				typeof(FlexibleListWrapper<long, int>),
				typeof(FlexibleListWrapper<long, uint>),
				typeof(FlexibleListWrapper<long, long>),
				typeof(FlexibleListWrapper<long, ulong>),
				typeof(FlexibleListWrapper<long, char>),
				typeof(FlexibleListWrapper<long, double>),
				typeof(FlexibleListWrapper<long, float>),
				typeof(FlexibleListWrapper<ulong, object>),
				typeof(FlexibleListWrapper<ulong, bool>),
				typeof(FlexibleListWrapper<ulong, byte>),
				typeof(FlexibleListWrapper<ulong, sbyte>),
				typeof(FlexibleListWrapper<ulong, short>),
				typeof(FlexibleListWrapper<ulong, ushort>),
				typeof(FlexibleListWrapper<ulong, int>),
				typeof(FlexibleListWrapper<ulong, uint>),
				typeof(FlexibleListWrapper<ulong, long>),
				typeof(FlexibleListWrapper<ulong, ulong>),
				typeof(FlexibleListWrapper<ulong, char>),
				typeof(FlexibleListWrapper<ulong, double>),
				typeof(FlexibleListWrapper<ulong, float>),
				typeof(FlexibleListWrapper<char, object>),
				typeof(FlexibleListWrapper<char, bool>),
				typeof(FlexibleListWrapper<char, byte>),
				typeof(FlexibleListWrapper<char, sbyte>),
				typeof(FlexibleListWrapper<char, short>),
				typeof(FlexibleListWrapper<char, ushort>),
				typeof(FlexibleListWrapper<char, int>),
				typeof(FlexibleListWrapper<char, uint>),
				typeof(FlexibleListWrapper<char, long>),
				typeof(FlexibleListWrapper<char, ulong>),
				typeof(FlexibleListWrapper<char, char>),
				typeof(FlexibleListWrapper<char, double>),
				typeof(FlexibleListWrapper<char, float>),
				typeof(FlexibleListWrapper<double, object>),
				typeof(FlexibleListWrapper<double, bool>),
				typeof(FlexibleListWrapper<double, byte>),
				typeof(FlexibleListWrapper<double, sbyte>),
				typeof(FlexibleListWrapper<double, short>),
				typeof(FlexibleListWrapper<double, ushort>),
				typeof(FlexibleListWrapper<double, int>),
				typeof(FlexibleListWrapper<double, uint>),
				typeof(FlexibleListWrapper<double, long>),
				typeof(FlexibleListWrapper<double, ulong>),
				typeof(FlexibleListWrapper<double, char>),
				typeof(FlexibleListWrapper<double, double>),
				typeof(FlexibleListWrapper<double, float>),
				typeof(FlexibleListWrapper<float, object>),
				typeof(FlexibleListWrapper<float, bool>),
				typeof(FlexibleListWrapper<float, byte>),
				typeof(FlexibleListWrapper<float, sbyte>),
				typeof(FlexibleListWrapper<float, short>),
				typeof(FlexibleListWrapper<float, ushort>),
				typeof(FlexibleListWrapper<float, int>),
				typeof(FlexibleListWrapper<float, uint>),
				typeof(FlexibleListWrapper<float, long>),
				typeof(FlexibleListWrapper<float, ulong>),
				typeof(FlexibleListWrapper<float, char>),
				typeof(FlexibleListWrapper<float, double>),
				typeof(FlexibleListWrapper<float, float>),
				typeof(FlexibleListWrapper<object, string>),
				typeof(FlexibleListWrapper<string, object>),
				typeof(FlexibleListWrapper<object, DateTime>),
				typeof(FlexibleListWrapper<DateTime, object>),
				typeof(FlexibleListWrapper<object, ParseObject>),
				typeof(FlexibleListWrapper<ParseObject, object>),
				typeof(FlexibleListWrapper<object, ParseGeoPoint>),
				typeof(FlexibleListWrapper<ParseGeoPoint, object>),
				typeof(FlexibleListWrapper<object, ParseFile>),
				typeof(FlexibleListWrapper<ParseFile, object>),
				typeof(FlexibleListWrapper<object, ParseACL>),
				typeof(FlexibleListWrapper<ParseACL, object>),
				typeof(FlexibleListWrapper<object, ParseUser>),
				typeof(FlexibleListWrapper<ParseUser, object>),
				typeof(FlexibleListWrapper<object, ParseRole>),
				typeof(FlexibleListWrapper<ParseRole, object>),
				typeof(FlexibleListWrapper<object, IList<bool>>),
				typeof(FlexibleListWrapper<IList<bool>, object>),
				typeof(FlexibleListWrapper<object, IList<int>>),
				typeof(FlexibleListWrapper<IList<int>, object>),
				typeof(FlexibleListWrapper<object, IList<float>>),
				typeof(FlexibleListWrapper<IList<float>, object>),
				typeof(FlexibleListWrapper<object, IList<double>>),
				typeof(FlexibleListWrapper<IList<double>, object>),
				typeof(FlexibleListWrapper<object, IList<string>>),
				typeof(FlexibleListWrapper<IList<string>, object>),
				typeof(FlexibleListWrapper<object, IList<object>>),
				typeof(FlexibleListWrapper<IList<object>, object>),
				typeof(FlexibleListWrapper<object, IList<DateTime>>),
				typeof(FlexibleListWrapper<IList<DateTime>, object>),
				typeof(FlexibleListWrapper<object, IList<ParseObject>>),
				typeof(FlexibleListWrapper<IList<ParseObject>, object>),
				typeof(FlexibleListWrapper<object, IList<ParseGeoPoint>>),
				typeof(FlexibleListWrapper<IList<ParseGeoPoint>, object>),
				typeof(FlexibleListWrapper<object, IList<ParseFile>>),
				typeof(FlexibleListWrapper<IList<ParseFile>, object>),
				typeof(FlexibleListWrapper<object, IList<ParseACL>>),
				typeof(FlexibleListWrapper<IList<ParseACL>, object>),
				typeof(FlexibleListWrapper<object, IList<ParseUser>>),
				typeof(FlexibleListWrapper<IList<ParseUser>, object>),
				typeof(FlexibleListWrapper<object, IList<ParseRole>>),
				typeof(FlexibleListWrapper<IList<ParseRole>, object>),
				typeof(FlexibleListWrapper<object, List<bool>>),
				typeof(FlexibleListWrapper<List<bool>, object>),
				typeof(FlexibleListWrapper<object, List<int>>),
				typeof(FlexibleListWrapper<List<int>, object>),
				typeof(FlexibleListWrapper<object, List<float>>),
				typeof(FlexibleListWrapper<List<float>, object>),
				typeof(FlexibleListWrapper<object, List<double>>),
				typeof(FlexibleListWrapper<List<double>, object>),
				typeof(FlexibleListWrapper<object, List<string>>),
				typeof(FlexibleListWrapper<List<string>, object>),
				typeof(FlexibleListWrapper<object, List<object>>),
				typeof(FlexibleListWrapper<List<object>, object>),
				typeof(FlexibleListWrapper<object, List<DateTime>>),
				typeof(FlexibleListWrapper<List<DateTime>, object>),
				typeof(FlexibleListWrapper<object, List<ParseObject>>),
				typeof(FlexibleListWrapper<List<ParseObject>, object>),
				typeof(FlexibleListWrapper<object, List<ParseGeoPoint>>),
				typeof(FlexibleListWrapper<List<ParseGeoPoint>, object>),
				typeof(FlexibleListWrapper<object, List<ParseFile>>),
				typeof(FlexibleListWrapper<List<ParseFile>, object>),
				typeof(FlexibleListWrapper<object, List<ParseACL>>),
				typeof(FlexibleListWrapper<List<ParseACL>, object>),
				typeof(FlexibleListWrapper<object, List<ParseUser>>),
				typeof(FlexibleListWrapper<List<ParseUser>, object>),
				typeof(FlexibleListWrapper<object, List<ParseRole>>),
				typeof(FlexibleListWrapper<List<ParseRole>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, bool>>),
				typeof(FlexibleListWrapper<IDictionary<string, bool>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, int>>),
				typeof(FlexibleListWrapper<IDictionary<string, int>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, float>>),
				typeof(FlexibleListWrapper<IDictionary<string, float>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, double>>),
				typeof(FlexibleListWrapper<IDictionary<string, double>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, string>>),
				typeof(FlexibleListWrapper<IDictionary<string, string>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, object>>),
				typeof(FlexibleListWrapper<IDictionary<string, object>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, DateTime>>),
				typeof(FlexibleListWrapper<IDictionary<string, DateTime>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, ParseObject>>),
				typeof(FlexibleListWrapper<IDictionary<string, ParseObject>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, ParseGeoPoint>>),
				typeof(FlexibleListWrapper<IDictionary<string, ParseGeoPoint>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, ParseFile>>),
				typeof(FlexibleListWrapper<IDictionary<string, ParseFile>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, ParseACL>>),
				typeof(FlexibleListWrapper<IDictionary<string, ParseACL>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, ParseUser>>),
				typeof(FlexibleListWrapper<IDictionary<string, ParseUser>, object>),
				typeof(FlexibleListWrapper<object, IDictionary<string, ParseRole>>),
				typeof(FlexibleListWrapper<IDictionary<string, ParseRole>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, bool>>),
				typeof(FlexibleListWrapper<Dictionary<string, bool>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, int>>),
				typeof(FlexibleListWrapper<Dictionary<string, int>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, float>>),
				typeof(FlexibleListWrapper<Dictionary<string, float>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, double>>),
				typeof(FlexibleListWrapper<Dictionary<string, double>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, string>>),
				typeof(FlexibleListWrapper<Dictionary<string, string>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, object>>),
				typeof(FlexibleListWrapper<Dictionary<string, object>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, DateTime>>),
				typeof(FlexibleListWrapper<Dictionary<string, DateTime>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, ParseObject>>),
				typeof(FlexibleListWrapper<Dictionary<string, ParseObject>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, ParseGeoPoint>>),
				typeof(FlexibleListWrapper<Dictionary<string, ParseGeoPoint>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, ParseFile>>),
				typeof(FlexibleListWrapper<Dictionary<string, ParseFile>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, ParseACL>>),
				typeof(FlexibleListWrapper<Dictionary<string, ParseACL>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, ParseUser>>),
				typeof(FlexibleListWrapper<Dictionary<string, ParseUser>, object>),
				typeof(FlexibleListWrapper<object, Dictionary<string, ParseRole>>),
				typeof(FlexibleListWrapper<Dictionary<string, ParseRole>, object>),
				typeof(FlexibleDictionaryWrapper<object, object>),
				typeof(FlexibleDictionaryWrapper<object, bool>),
				typeof(FlexibleDictionaryWrapper<object, byte>),
				typeof(FlexibleDictionaryWrapper<object, sbyte>),
				typeof(FlexibleDictionaryWrapper<object, short>),
				typeof(FlexibleDictionaryWrapper<object, ushort>),
				typeof(FlexibleDictionaryWrapper<object, int>),
				typeof(FlexibleDictionaryWrapper<object, uint>),
				typeof(FlexibleDictionaryWrapper<object, long>),
				typeof(FlexibleDictionaryWrapper<object, ulong>),
				typeof(FlexibleDictionaryWrapper<object, char>),
				typeof(FlexibleDictionaryWrapper<object, double>),
				typeof(FlexibleDictionaryWrapper<object, float>),
				typeof(FlexibleDictionaryWrapper<bool, object>),
				typeof(FlexibleDictionaryWrapper<bool, bool>),
				typeof(FlexibleDictionaryWrapper<bool, byte>),
				typeof(FlexibleDictionaryWrapper<bool, sbyte>),
				typeof(FlexibleDictionaryWrapper<bool, short>),
				typeof(FlexibleDictionaryWrapper<bool, ushort>),
				typeof(FlexibleDictionaryWrapper<bool, int>),
				typeof(FlexibleDictionaryWrapper<bool, uint>),
				typeof(FlexibleDictionaryWrapper<bool, long>),
				typeof(FlexibleDictionaryWrapper<bool, ulong>),
				typeof(FlexibleDictionaryWrapper<bool, char>),
				typeof(FlexibleDictionaryWrapper<bool, double>),
				typeof(FlexibleDictionaryWrapper<bool, float>),
				typeof(FlexibleDictionaryWrapper<byte, object>),
				typeof(FlexibleDictionaryWrapper<byte, bool>),
				typeof(FlexibleDictionaryWrapper<byte, byte>),
				typeof(FlexibleDictionaryWrapper<byte, sbyte>),
				typeof(FlexibleDictionaryWrapper<byte, short>),
				typeof(FlexibleDictionaryWrapper<byte, ushort>),
				typeof(FlexibleDictionaryWrapper<byte, int>),
				typeof(FlexibleDictionaryWrapper<byte, uint>),
				typeof(FlexibleDictionaryWrapper<byte, long>),
				typeof(FlexibleDictionaryWrapper<byte, ulong>),
				typeof(FlexibleDictionaryWrapper<byte, char>),
				typeof(FlexibleDictionaryWrapper<byte, double>),
				typeof(FlexibleDictionaryWrapper<byte, float>),
				typeof(FlexibleDictionaryWrapper<sbyte, object>),
				typeof(FlexibleDictionaryWrapper<sbyte, bool>),
				typeof(FlexibleDictionaryWrapper<sbyte, byte>),
				typeof(FlexibleDictionaryWrapper<sbyte, sbyte>),
				typeof(FlexibleDictionaryWrapper<sbyte, short>),
				typeof(FlexibleDictionaryWrapper<sbyte, ushort>),
				typeof(FlexibleDictionaryWrapper<sbyte, int>),
				typeof(FlexibleDictionaryWrapper<sbyte, uint>),
				typeof(FlexibleDictionaryWrapper<sbyte, long>),
				typeof(FlexibleDictionaryWrapper<sbyte, ulong>),
				typeof(FlexibleDictionaryWrapper<sbyte, char>),
				typeof(FlexibleDictionaryWrapper<sbyte, double>),
				typeof(FlexibleDictionaryWrapper<sbyte, float>),
				typeof(FlexibleDictionaryWrapper<short, object>),
				typeof(FlexibleDictionaryWrapper<short, bool>),
				typeof(FlexibleDictionaryWrapper<short, byte>),
				typeof(FlexibleDictionaryWrapper<short, sbyte>),
				typeof(FlexibleDictionaryWrapper<short, short>),
				typeof(FlexibleDictionaryWrapper<short, ushort>),
				typeof(FlexibleDictionaryWrapper<short, int>),
				typeof(FlexibleDictionaryWrapper<short, uint>),
				typeof(FlexibleDictionaryWrapper<short, long>),
				typeof(FlexibleDictionaryWrapper<short, ulong>),
				typeof(FlexibleDictionaryWrapper<short, char>),
				typeof(FlexibleDictionaryWrapper<short, double>),
				typeof(FlexibleDictionaryWrapper<short, float>),
				typeof(FlexibleDictionaryWrapper<ushort, object>),
				typeof(FlexibleDictionaryWrapper<ushort, bool>),
				typeof(FlexibleDictionaryWrapper<ushort, byte>),
				typeof(FlexibleDictionaryWrapper<ushort, sbyte>),
				typeof(FlexibleDictionaryWrapper<ushort, short>),
				typeof(FlexibleDictionaryWrapper<ushort, ushort>),
				typeof(FlexibleDictionaryWrapper<ushort, int>),
				typeof(FlexibleDictionaryWrapper<ushort, uint>),
				typeof(FlexibleDictionaryWrapper<ushort, long>),
				typeof(FlexibleDictionaryWrapper<ushort, ulong>),
				typeof(FlexibleDictionaryWrapper<ushort, char>),
				typeof(FlexibleDictionaryWrapper<ushort, double>),
				typeof(FlexibleDictionaryWrapper<ushort, float>),
				typeof(FlexibleDictionaryWrapper<int, object>),
				typeof(FlexibleDictionaryWrapper<int, bool>),
				typeof(FlexibleDictionaryWrapper<int, byte>),
				typeof(FlexibleDictionaryWrapper<int, sbyte>),
				typeof(FlexibleDictionaryWrapper<int, short>),
				typeof(FlexibleDictionaryWrapper<int, ushort>),
				typeof(FlexibleDictionaryWrapper<int, int>),
				typeof(FlexibleDictionaryWrapper<int, uint>),
				typeof(FlexibleDictionaryWrapper<int, long>),
				typeof(FlexibleDictionaryWrapper<int, ulong>),
				typeof(FlexibleDictionaryWrapper<int, char>),
				typeof(FlexibleDictionaryWrapper<int, double>),
				typeof(FlexibleDictionaryWrapper<int, float>),
				typeof(FlexibleDictionaryWrapper<uint, object>),
				typeof(FlexibleDictionaryWrapper<uint, bool>),
				typeof(FlexibleDictionaryWrapper<uint, byte>),
				typeof(FlexibleDictionaryWrapper<uint, sbyte>),
				typeof(FlexibleDictionaryWrapper<uint, short>),
				typeof(FlexibleDictionaryWrapper<uint, ushort>),
				typeof(FlexibleDictionaryWrapper<uint, int>),
				typeof(FlexibleDictionaryWrapper<uint, uint>),
				typeof(FlexibleDictionaryWrapper<uint, long>),
				typeof(FlexibleDictionaryWrapper<uint, ulong>),
				typeof(FlexibleDictionaryWrapper<uint, char>),
				typeof(FlexibleDictionaryWrapper<uint, double>),
				typeof(FlexibleDictionaryWrapper<uint, float>),
				typeof(FlexibleDictionaryWrapper<long, object>),
				typeof(FlexibleDictionaryWrapper<long, bool>),
				typeof(FlexibleDictionaryWrapper<long, byte>),
				typeof(FlexibleDictionaryWrapper<long, sbyte>),
				typeof(FlexibleDictionaryWrapper<long, short>),
				typeof(FlexibleDictionaryWrapper<long, ushort>),
				typeof(FlexibleDictionaryWrapper<long, int>),
				typeof(FlexibleDictionaryWrapper<long, uint>),
				typeof(FlexibleDictionaryWrapper<long, long>),
				typeof(FlexibleDictionaryWrapper<long, ulong>),
				typeof(FlexibleDictionaryWrapper<long, char>),
				typeof(FlexibleDictionaryWrapper<long, double>),
				typeof(FlexibleDictionaryWrapper<long, float>),
				typeof(FlexibleDictionaryWrapper<ulong, object>),
				typeof(FlexibleDictionaryWrapper<ulong, bool>),
				typeof(FlexibleDictionaryWrapper<ulong, byte>),
				typeof(FlexibleDictionaryWrapper<ulong, sbyte>),
				typeof(FlexibleDictionaryWrapper<ulong, short>),
				typeof(FlexibleDictionaryWrapper<ulong, ushort>),
				typeof(FlexibleDictionaryWrapper<ulong, int>),
				typeof(FlexibleDictionaryWrapper<ulong, uint>),
				typeof(FlexibleDictionaryWrapper<ulong, long>),
				typeof(FlexibleDictionaryWrapper<ulong, ulong>),
				typeof(FlexibleDictionaryWrapper<ulong, char>),
				typeof(FlexibleDictionaryWrapper<ulong, double>),
				typeof(FlexibleDictionaryWrapper<ulong, float>),
				typeof(FlexibleDictionaryWrapper<char, object>),
				typeof(FlexibleDictionaryWrapper<char, bool>),
				typeof(FlexibleDictionaryWrapper<char, byte>),
				typeof(FlexibleDictionaryWrapper<char, sbyte>),
				typeof(FlexibleDictionaryWrapper<char, short>),
				typeof(FlexibleDictionaryWrapper<char, ushort>),
				typeof(FlexibleDictionaryWrapper<char, int>),
				typeof(FlexibleDictionaryWrapper<char, uint>),
				typeof(FlexibleDictionaryWrapper<char, long>),
				typeof(FlexibleDictionaryWrapper<char, ulong>),
				typeof(FlexibleDictionaryWrapper<char, char>),
				typeof(FlexibleDictionaryWrapper<char, double>),
				typeof(FlexibleDictionaryWrapper<char, float>),
				typeof(FlexibleDictionaryWrapper<double, object>),
				typeof(FlexibleDictionaryWrapper<double, bool>),
				typeof(FlexibleDictionaryWrapper<double, byte>),
				typeof(FlexibleDictionaryWrapper<double, sbyte>),
				typeof(FlexibleDictionaryWrapper<double, short>),
				typeof(FlexibleDictionaryWrapper<double, ushort>),
				typeof(FlexibleDictionaryWrapper<double, int>),
				typeof(FlexibleDictionaryWrapper<double, uint>),
				typeof(FlexibleDictionaryWrapper<double, long>),
				typeof(FlexibleDictionaryWrapper<double, ulong>),
				typeof(FlexibleDictionaryWrapper<double, char>),
				typeof(FlexibleDictionaryWrapper<double, double>),
				typeof(FlexibleDictionaryWrapper<double, float>),
				typeof(FlexibleDictionaryWrapper<float, object>),
				typeof(FlexibleDictionaryWrapper<float, bool>),
				typeof(FlexibleDictionaryWrapper<float, byte>),
				typeof(FlexibleDictionaryWrapper<float, sbyte>),
				typeof(FlexibleDictionaryWrapper<float, short>),
				typeof(FlexibleDictionaryWrapper<float, ushort>),
				typeof(FlexibleDictionaryWrapper<float, int>),
				typeof(FlexibleDictionaryWrapper<float, uint>),
				typeof(FlexibleDictionaryWrapper<float, long>),
				typeof(FlexibleDictionaryWrapper<float, ulong>),
				typeof(FlexibleDictionaryWrapper<float, char>),
				typeof(FlexibleDictionaryWrapper<float, double>),
				typeof(FlexibleDictionaryWrapper<float, float>),
				typeof(FlexibleDictionaryWrapper<object, string>),
				typeof(FlexibleDictionaryWrapper<string, object>),
				typeof(FlexibleDictionaryWrapper<object, DateTime>),
				typeof(FlexibleDictionaryWrapper<DateTime, object>),
				typeof(FlexibleDictionaryWrapper<object, ParseObject>),
				typeof(FlexibleDictionaryWrapper<ParseObject, object>),
				typeof(FlexibleDictionaryWrapper<object, ParseGeoPoint>),
				typeof(FlexibleDictionaryWrapper<ParseGeoPoint, object>),
				typeof(FlexibleDictionaryWrapper<object, ParseFile>),
				typeof(FlexibleDictionaryWrapper<ParseFile, object>),
				typeof(FlexibleDictionaryWrapper<object, ParseACL>),
				typeof(FlexibleDictionaryWrapper<ParseACL, object>),
				typeof(FlexibleDictionaryWrapper<object, ParseUser>),
				typeof(FlexibleDictionaryWrapper<ParseUser, object>),
				typeof(FlexibleDictionaryWrapper<object, ParseRole>),
				typeof(FlexibleDictionaryWrapper<ParseRole, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<bool>>),
				typeof(FlexibleDictionaryWrapper<IList<bool>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<int>>),
				typeof(FlexibleDictionaryWrapper<IList<int>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<float>>),
				typeof(FlexibleDictionaryWrapper<IList<float>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<double>>),
				typeof(FlexibleDictionaryWrapper<IList<double>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<string>>),
				typeof(FlexibleDictionaryWrapper<IList<string>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<object>>),
				typeof(FlexibleDictionaryWrapper<IList<object>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<DateTime>>),
				typeof(FlexibleDictionaryWrapper<IList<DateTime>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<ParseObject>>),
				typeof(FlexibleDictionaryWrapper<IList<ParseObject>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<ParseGeoPoint>>),
				typeof(FlexibleDictionaryWrapper<IList<ParseGeoPoint>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<ParseFile>>),
				typeof(FlexibleDictionaryWrapper<IList<ParseFile>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<ParseACL>>),
				typeof(FlexibleDictionaryWrapper<IList<ParseACL>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<ParseUser>>),
				typeof(FlexibleDictionaryWrapper<IList<ParseUser>, object>),
				typeof(FlexibleDictionaryWrapper<object, IList<ParseRole>>),
				typeof(FlexibleDictionaryWrapper<IList<ParseRole>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<bool>>),
				typeof(FlexibleDictionaryWrapper<List<bool>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<int>>),
				typeof(FlexibleDictionaryWrapper<List<int>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<float>>),
				typeof(FlexibleDictionaryWrapper<List<float>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<double>>),
				typeof(FlexibleDictionaryWrapper<List<double>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<string>>),
				typeof(FlexibleDictionaryWrapper<List<string>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<string>>),
				typeof(FlexibleDictionaryWrapper<List<string>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<object>>),
				typeof(FlexibleDictionaryWrapper<List<object>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<DateTime>>),
				typeof(FlexibleDictionaryWrapper<List<DateTime>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<ParseObject>>),
				typeof(FlexibleDictionaryWrapper<List<ParseObject>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<ParseGeoPoint>>),
				typeof(FlexibleDictionaryWrapper<List<ParseGeoPoint>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<ParseFile>>),
				typeof(FlexibleDictionaryWrapper<List<ParseFile>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<ParseACL>>),
				typeof(FlexibleDictionaryWrapper<List<ParseACL>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<ParseUser>>),
				typeof(FlexibleDictionaryWrapper<List<ParseUser>, object>),
				typeof(FlexibleDictionaryWrapper<object, List<ParseRole>>),
				typeof(FlexibleDictionaryWrapper<List<ParseRole>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, bool>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, bool>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, int>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, int>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, float>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, float>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, double>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, double>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, string>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, string>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, object>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, object>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, DateTime>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, DateTime>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, ParseObject>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, ParseObject>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, ParseGeoPoint>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, ParseGeoPoint>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, ParseFile>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, ParseFile>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, ParseACL>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, ParseACL>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, ParseUser>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, ParseUser>, object>),
				typeof(FlexibleDictionaryWrapper<object, IDictionary<string, ParseRole>>),
				typeof(FlexibleDictionaryWrapper<IDictionary<string, ParseRole>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, bool>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, bool>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, int>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, int>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, float>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, float>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, double>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, double>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, string>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, string>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, object>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, object>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, DateTime>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, DateTime>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, ParseObject>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, ParseObject>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, ParseGeoPoint>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, ParseGeoPoint>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, ParseFile>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, ParseFile>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, ParseACL>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, ParseACL>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, ParseUser>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, ParseUser>, object>),
				typeof(FlexibleDictionaryWrapper<object, Dictionary<string, ParseRole>>),
				typeof(FlexibleDictionaryWrapper<Dictionary<string, ParseRole>, object>)
			};
		}

		internal static void RegisterNetworkRequest(WWW www, Action<WWW> action)
		{
			RunOnMainThread(delegate
			{
				bool isDone = www.isDone;
				action(www);
				if (!isDone)
				{
					RegisterNetworkRequest(www, action);
				}
			});
		}

		internal static void RegisterDeviceTokenRequest(Action<byte[]> action)
		{
			RunOnMainThread(delegate
			{
				byte[] deviceToken = NotificationServices.deviceToken;
				if (deviceToken != null)
				{
					action(deviceToken);
					RegisteriOSPushNotificationListener(delegate(IDictionary<string, object> payload)
					{
						ParsePush.parsePushNotificationReceived.Invoke(ParseInstallation.CurrentInstallation, new ParsePushNotificationEventArgs(payload));
					});
				}
				else
				{
					RegisterDeviceTokenRequest(action);
				}
			});
		}

		internal static void RegisteriOSPushNotificationListener(Action<IDictionary<string, object>> action)
		{
			RunOnMainThread(delegate
			{
				if (NotificationServices.remoteNotificationCount > 0)
				{
					RemoteNotification[] remoteNotifications = NotificationServices.remoteNotifications;
					for (int i = 0; i < remoteNotifications.Length; i++)
					{
						IDictionary userInfo = remoteNotifications[i].userInfo;
						Dictionary<string, object> dictionary = new Dictionary<string, object>();
						foreach (object key in userInfo.Keys)
						{
							dictionary[key.ToString()] = userInfo[key];
						}
						action(dictionary);
					}
					NotificationServices.ClearRemoteNotifications();
				}
				RegisteriOSPushNotificationListener(action);
			});
		}

		internal static void RunOnMainThread(Action action)
		{
			if (dispatchQueueLock.IsWriteLockHeld)
			{
				dispatchQueue.Enqueue(action);
				return;
			}
			dispatchQueueLock.EnterWriteLock();
			try
			{
				dispatchQueue.Enqueue(action);
			}
			finally
			{
				dispatchQueueLock.ExitWriteLock();
			}
		}

		internal static IEnumerator RunDispatcher()
		{
			while (true)
			{
				dispatchQueueLock.EnterUpgradeableReadLock();
				try
				{
					int num = dispatchQueue.Count;
					if (num > 0)
					{
						dispatchQueueLock.EnterWriteLock();
						try
						{
							while (num > 0)
							{
								try
								{
									dispatchQueue.Dequeue()();
								}
								catch (Exception exception)
								{
									Debug.LogException(exception);
								}
								num--;
							}
						}
						finally
						{
							dispatchQueueLock.ExitWriteLock();
						}
					}
				}
				finally
				{
					dispatchQueueLock.ExitUpgradeableReadLock();
				}
				yield return null;
			}
		}

		public void Initialize()
		{
			if (settingsPath != null)
			{
				return;
			}
			settingsPath = Path.Combine(Application.persistentDataPath, "Parse.settings");
			isWebPlayer = Application.isWebPlayer;
			osVersion = SystemInfo.deviceModel;
			appBuildVersion = Application.version;
			appDisplayVersion = Application.identifier;
			appName = Application.productName;
			settings = SettingsWrapper.Wrapper;
			ParseFacebookUtils.Initialize();
			if (!IsAndroid)
			{
				return;
			}
			try
			{
				CallStaticJavaUnityMethod("com.parse.ParsePushUnityHelper", "registerGcm", null);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public Task ExecuteParseInstallationSaveHookAsync(ParseInstallation installation)
		{
			return Task.Run(delegate
			{
				installation.SetIfDifferent("badge", installation.Badge);
			});
		}
	}
}
