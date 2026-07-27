using System;
using System.Collections.Generic;
using System.Linq;

namespace Amazon.Runtime.Internal
{
	public class ParameterCollection : SortedDictionary<string, ParameterValue>
	{
		public ParameterCollection()
			: base((IComparer<string>)StringComparer.Ordinal)
		{
		}

		public void Add(string key, string value)
		{
			Add(key, new StringParameterValue(value));
		}

		public void Add(string key, List<string> values)
		{
			Add(key, new StringListParameterValue(values));
		}

		public List<KeyValuePair<string, string>> GetSortedParametersList()
		{
			return GetParametersEnumerable().ToList();
		}

		private IEnumerable<KeyValuePair<string, string>> GetParametersEnumerable()
		{
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, ParameterValue> current = enumerator.Current;
					string name = current.Key;
					ParameterValue value = current.Value;
					StringParameterValue stringParameterValue = value as StringParameterValue;
					StringListParameterValue slpv = value as StringListParameterValue;
					if (stringParameterValue != null)
					{
						yield return new KeyValuePair<string, string>(name, stringParameterValue.Value);
						continue;
					}
					if (slpv != null)
					{
						List<string> value2 = slpv.Value;
						value2.Sort(StringComparer.Ordinal);
						foreach (string item in value2)
						{
							yield return new KeyValuePair<string, string>(name, item);
						}
						continue;
					}
					throw new AmazonClientException("Unsupported parameter value type '" + value.GetType().FullName + "'");
				}
			}
		}
	}
}
