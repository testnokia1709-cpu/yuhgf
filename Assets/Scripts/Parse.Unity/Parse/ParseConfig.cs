using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Parse.Internal;
using Parse.Utilities;

namespace Parse
{
	public class ParseConfig : IJsonConvertible
	{
		private IDictionary<string, object> properties = new Dictionary<string, object>();

		public static ParseConfig CurrentConfig
		{
			get
			{
				Task<ParseConfig> currentConfigAsync = ConfigController.CurrentConfigController.GetCurrentConfigAsync();
				currentConfigAsync.Wait();
				return currentConfigAsync.Result;
			}
		}

		private static IParseConfigController ConfigController
		{
			get
			{
				return ParseCorePlugins.Instance.ConfigController;
			}
		}

		public virtual object this[string key]
		{
			get
			{
				return properties[key];
			}
		}

		internal static void ClearCurrentConfig()
		{
			ConfigController.CurrentConfigController.ClearCurrentConfigAsync().Wait();
		}

		internal static void ClearCurrentConfigInMemory()
		{
			ConfigController.CurrentConfigController.ClearCurrentConfigInMemoryAsync().Wait();
		}

		internal ParseConfig()
		{
		}

		internal ParseConfig(IDictionary<string, object> fetchedConfig)
		{
			IDictionary<string, object> dictionary = ParseDecoder.Instance.Decode(fetchedConfig["params"]) as IDictionary<string, object>;
			properties = dictionary;
		}

		public static Task<ParseConfig> GetAsync()
		{
			return GetAsync(CancellationToken.None);
		}

		public static Task<ParseConfig> GetAsync(CancellationToken cancellationToken)
		{
			return ConfigController.FetchConfigAsync(ParseUser.CurrentSessionToken, cancellationToken);
		}

		public T Get<T>(string key)
		{
			return (T)Conversion.ConvertTo<T>(properties[key]);
		}

		public bool TryGetValue<T>(string key, out T result)
		{
			if (properties.ContainsKey(key))
			{
				object obj = Conversion.ConvertTo<T>(properties[key]);
				if (obj is T || (obj == null && (!typeof(T).GetTypeInfo().IsValueType || ReflectionHelpers.IsNullable(typeof(T)))))
				{
					result = (T)obj;
					return true;
				}
			}
			result = default(T);
			return false;
		}

		IDictionary<string, object> IJsonConvertible.ToJSON()
		{
			return new Dictionary<string, object> { 
			{
				"params",
				NoObjectsEncoder.Instance.Encode(properties)
			} };
		}
	}
}
