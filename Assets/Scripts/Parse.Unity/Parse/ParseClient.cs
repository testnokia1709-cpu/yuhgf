using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Parse.Internal;

namespace Parse
{
	public static class ParseClient
	{
		public struct Configuration
		{
			public string ApplicationId { get; set; }

			public string Server { get; set; }

			public string WindowsKey { get; set; }
		}

		internal static readonly string[] DateFormatStrings;

		private static readonly object mutex;

		private static readonly string[] assemblyNames;

		private static readonly IPlatformHooks platformHooks;

		private static readonly IParseCommandRunner commandRunner;

		private static readonly string versionString;

		internal static IPlatformHooks PlatformHooks
		{
			get
			{
				return platformHooks;
			}
		}

		internal static IParseCommandRunner ParseCommandRunner
		{
			get
			{
				return commandRunner;
			}
		}

		public static Configuration CurrentConfiguration { get; internal set; }

		internal static string MasterKey { get; set; }

		internal static Version Version
		{
			get
			{
				return new AssemblyName(typeof(ParseClient).GetTypeInfo().Assembly.FullName).Version;
			}
		}

		internal static string VersionString
		{
			get
			{
				return versionString;
			}
		}

		internal static Guid? InstallationId
		{
			get
			{
				return ParseCorePlugins.Instance.InstallationIdController.Get();
			}
			set
			{
				ParseCorePlugins.Instance.InstallationIdController.Set(value);
			}
		}

		internal static IDictionary<string, object> ApplicationSettings
		{
			get
			{
				return PlatformHooks.ApplicationSettings;
			}
		}

		static ParseClient()
		{
			DateFormatStrings = new string[3] { "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'", "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ff'Z'", "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'f'Z'" };
			mutex = new object();
			assemblyNames = new string[6] { "Parse.Phone", "Parse.WinRT", "Parse.NetFx45", "Parse.iOS", "Parse.Android", "Parse.Unity" };
			Type parseType = GetParseType("PlatformHooks");
			if (parseType == null)
			{
				throw new InvalidOperationException("You must include a reference to a platform-specific Parse library.");
			}
			platformHooks = Activator.CreateInstance(parseType) as IPlatformHooks;
			commandRunner = new ParseCommandRunner(platformHooks.HttpClient);
			versionString = "net-" + platformHooks.SDKName + Version;
		}

		private static Type GetParseType(string name)
		{
			string[] array = assemblyNames;
			foreach (string arg in array)
			{
				Type type = Type.GetType(string.Format("Parse.{0}, {1}", name, arg));
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		public static void Initialize(string applicationId, string dotnetKey)
		{
			Initialize(new Configuration
			{
				ApplicationId = applicationId,
				WindowsKey = dotnetKey
			});
		}

		public static void Initialize(Configuration configuration)
		{
			lock (mutex)
			{
				configuration.Server = (string.IsNullOrEmpty(configuration.Server) ? "https://api.parse.com/1/" : configuration.Server);
				CurrentConfiguration = configuration;
				ParseObject.RegisterSubclass<ParseUser>();
				ParseObject.RegisterSubclass<ParseInstallation>();
				ParseObject.RegisterSubclass<ParseRole>();
				ParseObject.RegisterSubclass<ParseSession>();
				PlatformHooks.Initialize();
			}
		}

		internal static string BuildQueryString(IDictionary<string, object> parameters)
		{
			return string.Join("&", (from pair in parameters
				let valueString = pair.Value as string
				select string.Format("{0}={1}", Uri.EscapeDataString(pair.Key), Uri.EscapeDataString(string.IsNullOrEmpty(valueString) ? Json.Encode(pair.Value) : valueString))).ToArray());
		}

		internal static IDictionary<string, string> DecodeQueryString(string queryString)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] array = queryString.Split('&');
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[1] { '=' }, 2);
				dictionary[array2[0]] = ((array2.Length == 2) ? Uri.UnescapeDataString(array2[1].Replace("+", " ")) : null);
			}
			return dictionary;
		}

		internal static IDictionary<string, object> DeserializeJsonString(string jsonData)
		{
			return Json.Parse(jsonData) as IDictionary<string, object>;
		}

		internal static string SerializeJsonString(IDictionary<string, object> jsonData)
		{
			return Json.Encode(jsonData);
		}
	}
}
