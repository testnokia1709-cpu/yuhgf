using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Amazon.Runtime;
using Amazon.Util.Internal;
using ThirdParty.Json.LitJson;

namespace Amazon.Internal
{
	public class RegionEndpointProviderV3 : IRegionEndpointProvider
	{
		private const string ENDPOINT_JSON_RESOURCE = "Amazon.endpoints.json";

		private const string ENDPOINT_JSON = "endpoints.json";

		private JsonData _root;

		private Dictionary<string, IRegionEndpoint> _regionEndpointMap = new Dictionary<string, IRegionEndpoint>();

		private object _regionEndpointMapLock = new object();

		private object _allRegionEndpointsLock = new object();

		private IEnumerable<IRegionEndpoint> _allRegionEndpoints;

		private static JsonData _emptyDictionaryJsonData = JsonMapper.ToObject("{}");

		public IEnumerable<IRegionEndpoint> AllRegionEndpoints
		{
			get
			{
				lock (_allRegionEndpointsLock)
				{
					lock (_regionEndpointMapLock)
					{
						if (_allRegionEndpoints == null)
						{
							JsonData jsonData = _root["partitions"];
							List<IRegionEndpoint> list = new List<IRegionEndpoint>();
							foreach (JsonData item in (IEnumerable)jsonData)
							{
								JsonData jsonData3 = item["regions"];
								foreach (string propertyName in jsonData3.PropertyNames)
								{
									IRegionEndpoint value;
									if (!_regionEndpointMap.TryGetValue(propertyName, out value))
									{
										value = new RegionEndpointV3(propertyName, (string)jsonData3[propertyName]["description"], item, item["services"]);
										_regionEndpointMap.Add(propertyName, value);
									}
									list.Add(value);
								}
							}
							_allRegionEndpoints = list;
						}
					}
				}
				return _allRegionEndpoints;
			}
		}

		public RegionEndpointProviderV3()
		{
			using (Stream stream = GetEndpointJsonSourceStream())
			{
				using (StreamReader reader = new StreamReader(stream))
				{
					_root = JsonMapper.ToObject(reader);
				}
			}
		}

		public RegionEndpointProviderV3(JsonData root)
		{
			_root = root;
		}

		private static Stream GetEndpointJsonSourceStream()
		{
			return TypeFactory.GetTypeInfo(typeof(RegionEndpointProviderV3)).Assembly.GetManifestResourceStream("Amazon.endpoints.json");
		}

		private static string GetUnknownRegionDescription(string regionName)
		{
			if (regionName.StartsWith("cn-", StringComparison.OrdinalIgnoreCase) || regionName.EndsWith("cn-global", StringComparison.OrdinalIgnoreCase))
			{
				return "China (Unknown)";
			}
			return "Unknown";
		}

		private static bool IsRegionInPartition(string regionName, JsonData partition, out string description)
		{
			JsonData jsonData = partition["regions"];
			string pattern = (string)partition["regionRegex"];
			if (jsonData[regionName] != null)
			{
				description = (string)jsonData[regionName]["description"];
				return true;
			}
			if (regionName.Equals((string)partition["partition"] + "-global", StringComparison.OrdinalIgnoreCase))
			{
				description = "Global";
				return true;
			}
			if (new Regex(pattern).Match(regionName).Success)
			{
				description = GetUnknownRegionDescription(regionName);
				return true;
			}
			description = GetUnknownRegionDescription(regionName);
			return false;
		}

		public IRegionEndpoint GetRegionEndpoint(string regionName)
		{
			try
			{
				lock (_regionEndpointMapLock)
				{
					IRegionEndpoint value;
					if (_regionEndpointMap.TryGetValue(regionName, out value))
					{
						return value;
					}
					foreach (JsonData item in (IEnumerable)_root["partitions"])
					{
						string description;
						if (IsRegionInPartition(regionName, item, out description))
						{
							value = new RegionEndpointV3(regionName, description, item, item["services"]);
							_regionEndpointMap.Add(regionName, value);
							return value;
						}
					}
				}
			}
			catch (Exception)
			{
				throw new AmazonClientException("Invalid endpoint.json format.");
			}
			return GetNonstandardRegionEndpoint(regionName);
		}

		private IRegionEndpoint GetNonstandardRegionEndpoint(string regionName)
		{
			JsonData partition = _root["partitions"][0];
			string unknownRegionDescription = GetUnknownRegionDescription(regionName);
			JsonData services = _emptyDictionaryJsonData;
			foreach (JsonData item in (IEnumerable)_root["partitions"])
			{
				JsonData jsonData2 = item["services"];
				foreach (string propertyName in jsonData2.PropertyNames)
				{
					if (jsonData2[propertyName] != null && jsonData2[propertyName].Count > 0)
					{
						JsonData jsonData3 = jsonData2[propertyName];
						if (jsonData3 != null && jsonData3["endpoints"][regionName] != null)
						{
							partition = item;
							services = jsonData2;
							break;
						}
					}
				}
			}
			return new RegionEndpointV3(regionName, unknownRegionDescription, partition, services);
		}
	}
}
