using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;

namespace Amazon.Util
{
	public static class AWSSDKUtils
	{
		internal const string DefaultRegion = "us-east-1";

		internal const string DefaultGovRegion = "us-gov-west-1";

		private const char SlashChar = '/';

		private const string Slash = "/";

		internal const int DefaultMaxRetry = 3;

		private const int DefaultConnectionLimit = 50;

		private const int DefaultMaxIdleTime = 50000;

		public static readonly DateTime EPOCH_START = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public const int DefaultBufferSize = 8192;

		public const long DefaultProgressUpdateInterval = 102400L;

		internal static Dictionary<int, string> RFCEncodingSchemes = new Dictionary<int, string>
		{
			{ 3986, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~" },
			{ 1738, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_." }
		};

		internal const string S3Accelerate = "s3-accelerate";

		public const string UserAgentHeader = "User-Agent";

		public const string ValidUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		public const string ValidUrlCharactersRFC1738 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";

		private static string ValidPathCharacters = DetermineValidPathCharacters();

		public const string UrlEncodedContent = "application/x-www-form-urlencoded; charset=utf-8";

		public const string GMTDateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";

		public const string ISO8601DateFormat = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

		public const string ISO8601DateFormatNoMS = "yyyy-MM-dd\\THH:mm:ss\\Z";

		public const string ISO8601BasicDateTimeFormat = "yyyyMMddTHHmmssZ";

		public const string ISO8601BasicDateFormat = "yyyyMMdd";

		public const string RFC822DateFormat = "ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T";

		private static BackgroundInvoker _dispatcher;

		private static readonly object _preserveStackTraceLookupLock = new object();

		private static bool _preserveStackTraceLookup = false;

		private static MethodInfo _preserveStackTrace;

		private const int _defaultDefaultConnectionLimit = 2;

		private const int _defaultMaxIdleTime = 100000;

		private static BackgroundInvoker Dispatcher
		{
			get
			{
				if (_dispatcher == null)
				{
					_dispatcher = new BackgroundInvoker();
				}
				return _dispatcher;
			}
		}

		public static string FormattedCurrentTimestampGMT
		{
			get
			{
				DateTime correctedUtcNow = CorrectedUtcNow;
				return new DateTime(correctedUtcNow.Year, correctedUtcNow.Month, correctedUtcNow.Day, correctedUtcNow.Hour, correctedUtcNow.Minute, correctedUtcNow.Second, correctedUtcNow.Millisecond, DateTimeKind.Local).ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture);
			}
		}

		public static string FormattedCurrentTimestampISO8601
		{
			get
			{
				return GetFormattedTimestampISO8601(0);
			}
		}

		public static string FormattedCurrentTimestampRFC822
		{
			get
			{
				return GetFormattedTimestampRFC822(0);
			}
		}

		[Obsolete("This property does not account for endpoint specific clock skew.  Use CorrectClockSkew.GetCorrectedUtcNowForEndpoint() instead.")]
		public static DateTime CorrectedUtcNow
		{
			get
			{
				DateTime result = AWSConfigs.utcNowSource();
				if (AWSConfigs.ManualClockCorrection.HasValue)
				{
					result += AWSConfigs.ManualClockCorrection.Value;
				}
				return result;
			}
		}

		internal static bool IsIL2CPP
		{
			get
			{
				Type type = Type.GetType("Mono.Runtime");
				if (type != null)
				{
					MethodInfo method = type.GetMethod("GetDisplayName", BindingFlags.Static | BindingFlags.NonPublic);
					if (method != null)
					{
						string text = null;
						try
						{
							text = method.Invoke(null, null).ToString();
						}
						catch (Exception)
						{
							return false;
						}
						if (text != null && text.ToUpper().Contains("IL2CPP"))
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		private static string DetermineValidPathCharacters()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = "/:'()!*[]$";
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				string text2 = Uri.EscapeUriString(c.ToString());
				if (text2.Length == 1 && text2[0] == c)
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetExtension(string path)
		{
			if (path == null)
			{
				return null;
			}
			int length = path.Length;
			int num = length;
			while (--num >= 0)
			{
				char c = path[num];
				if (c == '.')
				{
					if (num != length - 1)
					{
						return path.Substring(num, length - num);
					}
					return string.Empty;
				}
				if (IsPathSeparator(c))
				{
					break;
				}
			}
			return string.Empty;
		}

		private static bool IsPathSeparator(char ch)
		{
			if (ch != '\\' && ch != '/')
			{
				return ch == ':';
			}
			return true;
		}

		internal static string CalculateStringToSignV2(ParameterCollection parameterCollection, string serviceUrl)
		{
			StringBuilder stringBuilder = new StringBuilder("POST\n", 512);
			List<KeyValuePair<string, string>> sortedParametersList = parameterCollection.GetSortedParametersList();
			Uri uri = new Uri(serviceUrl);
			stringBuilder.Append(uri.Host);
			stringBuilder.Append("\n");
			string text = uri.AbsolutePath;
			if (text == null || text.Length == 0)
			{
				text = "/";
			}
			stringBuilder.Append(UrlEncode(text, true));
			stringBuilder.Append("\n");
			foreach (KeyValuePair<string, string> item in sortedParametersList)
			{
				if (item.Value != null)
				{
					stringBuilder.Append(UrlEncode(item.Key, false));
					stringBuilder.Append("=");
					stringBuilder.Append(UrlEncode(item.Value, false));
					stringBuilder.Append("&");
				}
			}
			string text2 = stringBuilder.ToString();
			return text2.Remove(text2.Length - 1);
		}

		internal static string GetParametersAsString(IRequest request)
		{
			return GetParametersAsString(request.ParameterCollection);
		}

		internal static string GetParametersAsString(ParameterCollection parameterCollection)
		{
			List<KeyValuePair<string, string>> sortedParametersList = parameterCollection.GetSortedParametersList();
			StringBuilder stringBuilder = new StringBuilder(512);
			foreach (KeyValuePair<string, string> item in sortedParametersList)
			{
				string key = item.Key;
				string value = item.Value;
				if (value != null)
				{
					stringBuilder.Append(key);
					stringBuilder.Append('=');
					stringBuilder.Append(UrlEncode(value, false));
					stringBuilder.Append('&');
				}
			}
			string text = stringBuilder.ToString();
			if (text.Length == 0)
			{
				return string.Empty;
			}
			return text.Remove(text.Length - 1);
		}

		public static string CanonicalizeResourcePath(Uri endpoint, string resourcePath)
		{
			return CanonicalizeResourcePath(endpoint, resourcePath, false);
		}

		public static string CanonicalizeResourcePath(Uri endpoint, string resourcePath, bool detectPreEncode)
		{
			if (endpoint != null)
			{
				string text = endpoint.AbsolutePath;
				if (string.IsNullOrEmpty(text) || string.Equals(text, "/", StringComparison.Ordinal))
				{
					text = string.Empty;
				}
				if (!string.IsNullOrEmpty(resourcePath) && resourcePath.StartsWith("/", StringComparison.Ordinal))
				{
					resourcePath = resourcePath.Substring(1);
				}
				if (!string.IsNullOrEmpty(resourcePath))
				{
					text = text + "/" + resourcePath;
				}
				resourcePath = text;
			}
			if (string.IsNullOrEmpty(resourcePath))
			{
				return "/";
			}
			IEnumerable<string> source = resourcePath.Split(new char[1] { '/' }, StringSplitOptions.None);
			bool flag = false;
			if (detectPreEncode)
			{
				if (endpoint == null)
				{
					throw new ArgumentNullException("endpoint", "A non-null endpoint is necessary to decide whether or not to pre URL encode.");
				}
				if (!S3Uri.IsS3Uri(endpoint))
				{
					source = source.Select((string segment) => UrlEncode(segment, true));
					flag = true;
				}
			}
			source = source.Select((string segment) => UrlEncode(segment, false));
			string text2 = string.Join("/", source.ToArray());
			Logger.GetLogger(typeof(AWSSDKUtils)).DebugFormat("{0} encoded {1}{2} for canonicalization: {3}", flag ? "Double" : "Single", resourcePath, (endpoint == null) ? "" : (" with endpoint " + endpoint.AbsoluteUri), text2);
			return text2;
		}

		public static string Join(List<string> strings)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string @string in strings)
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(@string);
				flag = false;
			}
			return stringBuilder.ToString();
		}

		public static string DetermineRegion(string url)
		{
			int num = url.IndexOf("//", StringComparison.Ordinal);
			if (num >= 0)
			{
				url = url.Substring(num + 2);
			}
			if (url.EndsWith("/", StringComparison.Ordinal))
			{
				url = url.Substring(0, url.Length - 1);
			}
			int num2 = url.IndexOf(".amazonaws.com", StringComparison.Ordinal);
			if (num2 < 0)
			{
				return "us-east-1";
			}
			string text = url.Substring(0, num2);
			int num3 = url.IndexOf(".cloudsearch.amazonaws.com", StringComparison.Ordinal);
			if (num3 > 0)
			{
				text = url.Substring(0, num3);
			}
			int num4 = text.IndexOf("queue", StringComparison.Ordinal);
			if (num4 == 0)
			{
				return "us-east-1";
			}
			if (num4 > 0)
			{
				return text.Substring(0, num4 - 1);
			}
			if (text.StartsWith("s3-", StringComparison.Ordinal))
			{
				if (text.Equals("s3-accelerate", StringComparison.Ordinal))
				{
					return null;
				}
				text = "s3." + text.Substring(3);
			}
			int num5 = text.LastIndexOf('.');
			if (num5 == -1)
			{
				return "us-east-1";
			}
			string text2 = text.Substring(num5 + 1);
			if (text2.Equals("external-1"))
			{
				return RegionEndpoint.USEast1.SystemName;
			}
			if (string.Equals(text2, "us-gov", StringComparison.Ordinal))
			{
				return "us-gov-west-1";
			}
			return text2;
		}

		public static string DetermineService(string url)
		{
			int num = url.IndexOf("//", StringComparison.Ordinal);
			if (num >= 0)
			{
				url = url.Substring(num + 2);
			}
			string[] array = url.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			if (array == null || array.Length == 0)
			{
				return string.Empty;
			}
			string text = array[0];
			int num2 = text.IndexOf('-');
			string text2 = ((num2 >= 0) ? text.Substring(0, num2) : text);
			if (text2.Equals("queue"))
			{
				return "sqs";
			}
			return text2;
		}

		public static DateTime ConvertFromUnixEpochSeconds(int seconds)
		{
			return new DateTime((long)seconds * 10000000L + EPOCH_START.Ticks, DateTimeKind.Utc).ToLocalTime();
		}

		public static int ConvertToUnixEpochSeconds(DateTime dateTime)
		{
			TimeSpan timeSpan = new TimeSpan(dateTime.ToUniversalTime().Ticks - EPOCH_START.Ticks);
			return Convert.ToInt32(timeSpan.TotalSeconds);
		}

		public static string ConvertToUnixEpochSecondsString(DateTime dateTime)
		{
			TimeSpan timeSpan = new TimeSpan(dateTime.ToUniversalTime().Ticks - EPOCH_START.Ticks);
			return Convert.ToInt64(timeSpan.TotalSeconds).ToString(CultureInfo.InvariantCulture);
		}

		public static double ConvertToUnixEpochMilliSeconds(DateTime dateTime)
		{
			TimeSpan timeSpan = new TimeSpan(dateTime.ToUniversalTime().Ticks - EPOCH_START.Ticks);
			return Math.Round(timeSpan.TotalMilliseconds, 0) / 1000.0;
		}

		public static string ToHex(byte[] data, bool lowercase)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				stringBuilder.Append(data[i].ToString(lowercase ? "x2" : "X2", CultureInfo.InvariantCulture));
			}
			return stringBuilder.ToString();
		}

		public static void InvokeInBackground<T>(EventHandler<T> handler, T args, object sender) where T : EventArgs
		{
			if (handler == null)
			{
				return;
			}
			Delegate[] invocationList = handler.GetInvocationList();
			foreach (Delegate obj in invocationList)
			{
				EventHandler<T> eventHandler = (EventHandler<T>)obj;
				if (eventHandler != null)
				{
					Dispatcher.Dispatch(delegate
					{
						eventHandler(sender, args);
					});
				}
			}
		}

		public static Dictionary<string, string> ParseQueryParameters(string url)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(url))
			{
				int num = url.IndexOf('?');
				if (num >= 0)
				{
					string[] array = url.Substring(num + 1).Split(new char[1] { '&' }, StringSplitOptions.None);
					foreach (string text in array)
					{
						if (!string.IsNullOrEmpty(text))
						{
							string[] array2 = text.Split(new char[1] { '=' }, 2);
							string key = array2[0];
							string value = ((array2.Length == 1) ? null : array2[1]);
							dictionary[key] = value;
						}
					}
				}
			}
			return dictionary;
		}

		internal static bool AreEqual(object[] itemsA, object[] itemsB)
		{
			if (itemsA == null || itemsB == null)
			{
				return itemsA == itemsB;
			}
			if (itemsA.Length != itemsB.Length)
			{
				return false;
			}
			int num = itemsA.Length;
			for (int i = 0; i < num; i++)
			{
				object a = itemsA[i];
				object b = itemsB[i];
				if (!AreEqual(a, b))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool AreEqual(object a, object b)
		{
			if (a == null || b == null)
			{
				return a == b;
			}
			if (a == b)
			{
				return true;
			}
			return a.Equals(b);
		}

		internal static bool DictionariesAreEqual<K, V>(Dictionary<K, V> a, Dictionary<K, V> b)
		{
			if (a == null || b == null)
			{
				return a == b;
			}
			if (a == b)
			{
				return true;
			}
			if (a.Count == b.Count)
			{
				return !a.Except(b).Any();
			}
			return false;
		}

		public static MemoryStream GenerateMemoryStreamFromString(string s)
		{
			MemoryStream memoryStream = new MemoryStream();
			StreamWriter streamWriter = new StreamWriter(memoryStream);
			streamWriter.Write(s);
			streamWriter.Flush();
			memoryStream.Position = 0L;
			return memoryStream;
		}

		public static void CopyStream(Stream source, Stream destination)
		{
			CopyStream(source, destination, 8192);
		}

		public static void CopyStream(Stream source, Stream destination, int bufferSize)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			byte[] array = new byte[bufferSize];
			int count;
			while ((count = source.Read(array, 0, array.Length)) != 0)
			{
				destination.Write(array, 0, count);
			}
		}

		public static string GetFormattedTimestampISO8601(int minutesFromNow)
		{
			return GetFormattedTimestampISO8601(CorrectedUtcNow.AddMinutes(minutesFromNow));
		}

		internal static string GetFormattedTimestampISO8601(IClientConfig config)
		{
			return GetFormattedTimestampISO8601(config.CorrectedUtcNow);
		}

		private static string GetFormattedTimestampISO8601(DateTime dateTime)
		{
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Local).ToString("yyyy-MM-dd\\THH:mm:ss.fff\\Z", CultureInfo.InvariantCulture);
		}

		public static string GetFormattedTimestampRFC822(int minutesFromNow)
		{
			DateTime dateTime = CorrectedUtcNow.AddMinutes(minutesFromNow);
			return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Local).ToString("ddd, dd MMM yyyy HH:mm:ss \\G\\M\\T", CultureInfo.InvariantCulture);
		}

		public static string UrlEncode(string data, bool path)
		{
			return UrlEncode(3986, data, path);
		}

		public static string UrlEncode(int rfcNumber, string data, bool path)
		{
			StringBuilder stringBuilder = new StringBuilder(data.Length * 2);
			string value;
			if (!RFCEncodingSchemes.TryGetValue(rfcNumber, out value))
			{
				value = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
			}
			string text = value + (path ? ValidPathCharacters : "");
			byte[] bytes = Encoding.UTF8.GetBytes(data);
			for (int i = 0; i < bytes.Length; i++)
			{
				char c = (char)bytes[i];
				if (text.IndexOf(c) != -1)
				{
					stringBuilder.Append(c);
					continue;
				}
				stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
			}
			return stringBuilder.ToString();
		}

		public static void Sleep(TimeSpan ts)
		{
			Sleep((int)ts.TotalMilliseconds);
		}

		public static string BytesToHexString(byte[] value)
		{
			return BitConverter.ToString(value).Replace("-", string.Empty);
		}

		public static byte[] HexStringToBytes(string hex)
		{
			if (string.IsNullOrEmpty(hex) || hex.Length % 2 == 1)
			{
				throw new ArgumentOutOfRangeException("hex");
			}
			int num = 0;
			byte[] array = new byte[hex.Length / 2];
			for (int i = 0; i < hex.Length; i += 2)
			{
				byte b = Convert.ToByte(hex.Substring(i, 2), 16);
				array[num] = b;
				num++;
			}
			return array;
		}

		public static bool HasBidiControlCharacters(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return false;
			}
			for (int i = 0; i < input.Length; i++)
			{
				if (IsBidiControlChar(input[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsBidiControlChar(char c)
		{
			switch (c)
			{
			default:
				return false;
			case '‐':
			case '‑':
			case '‒':
			case '–':
			case '—':
			case '―':
			case '‖':
			case '‗':
			case '‘':
			case '’':
			case '‚':
			case '‛':
			case '“':
			case '”':
			case '„':
			case '‟':
			case '†':
			case '‡':
			case '•':
			case '‣':
			case '․':
			case '‥':
			case '…':
			case '‧':
			case '\u2028':
			case '\u2029':
			case '\u202e':
				return c == '\u202e';
			case '\u200e':
			case '\u200f':
			case '\u202a':
			case '\u202b':
			case '\u202c':
			case '\u202d':
				return true;
			}
		}

		public static string DownloadStringContent(Uri uri)
		{
			return DownloadStringContent(uri, TimeSpan.Zero);
		}

		public static string DownloadStringContent(Uri uri, TimeSpan timeout)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
			if (timeout > TimeSpan.Zero)
			{
				httpWebRequest.Timeout = (int)timeout.TotalMilliseconds;
			}
			using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
			{
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		public static Stream OpenStream(Uri uri)
		{
			HttpWebRequest obj = WebRequest.Create(uri) as HttpWebRequest;
			IAsyncResult asyncResult = obj.BeginGetResponse(null, null);
			return (obj.EndGetResponse(asyncResult) as HttpWebResponse).GetResponseStream();
		}

		public static void ForceCanonicalPathAndQuery(Uri uri)
		{
			try
			{
				FieldInfo field = typeof(Uri).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
				ulong num = (ulong)field.GetValue(uri);
				num &= 0xFFFFFFFFFFFFFFCFuL;
				field.SetValue(uri, num);
			}
			catch
			{
			}
		}

		public static void PreserveStackTrace(Exception exception)
		{
			if (!_preserveStackTraceLookup)
			{
				lock (_preserveStackTraceLookupLock)
				{
					_preserveStackTraceLookup = true;
					try
					{
						_preserveStackTrace = typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);
					}
					catch
					{
					}
				}
			}
			if (_preserveStackTrace != null)
			{
				_preserveStackTrace.Invoke(exception, null);
			}
		}

		internal static int GetConnectionLimit(int? clientConfigValue)
		{
			if (clientConfigValue.HasValue)
			{
				return clientConfigValue.Value;
			}
			return 50;
		}

		internal static int GetMaxIdleTime(int? clientConfigValue)
		{
			if (clientConfigValue.HasValue)
			{
				return clientConfigValue.Value;
			}
			return 50000;
		}

		public static void Sleep(int ms)
		{
			Thread.Sleep(ms);
		}
	}
}
