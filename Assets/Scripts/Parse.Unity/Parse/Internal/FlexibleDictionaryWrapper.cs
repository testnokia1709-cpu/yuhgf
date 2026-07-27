using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class FlexibleDictionaryWrapper<TOut, TIn> : IDictionary<string, TOut>, ICollection<KeyValuePair<string, TOut>>, IEnumerable<KeyValuePair<string, TOut>>, IEnumerable
	{
		private readonly IDictionary<string, TIn> toWrap;

		public ICollection<string> Keys
		{
			get
			{
				return toWrap.Keys;
			}
		}

		public ICollection<TOut> Values
		{
			get
			{
				return toWrap.Values.Select((TIn item) => (TOut)Conversion.ConvertTo<TOut>(item)).ToList();
			}
		}

		public TOut this[string key]
		{
			get
			{
				return (TOut)Conversion.ConvertTo<TOut>(toWrap[key]);
			}
			set
			{
				toWrap[key] = (TIn)Conversion.ConvertTo<TIn>(value);
			}
		}

		public int Count
		{
			get
			{
				return toWrap.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return toWrap.IsReadOnly;
			}
		}

		public FlexibleDictionaryWrapper(IDictionary<string, TIn> toWrap)
		{
			this.toWrap = toWrap;
		}

		public void Add(string key, TOut value)
		{
			toWrap.Add(key, (TIn)Conversion.ConvertTo<TIn>(value));
		}

		public bool ContainsKey(string key)
		{
			return toWrap.ContainsKey(key);
		}

		public bool Remove(string key)
		{
			return toWrap.Remove(key);
		}

		public bool TryGetValue(string key, out TOut value)
		{
			TIn value2;
			bool result = toWrap.TryGetValue(key, out value2);
			value = (TOut)Conversion.ConvertTo<TOut>(value2);
			return result;
		}

		public void Add(KeyValuePair<string, TOut> item)
		{
			toWrap.Add(new KeyValuePair<string, TIn>(item.Key, (TIn)Conversion.ConvertTo<TIn>(item.Value)));
		}

		public void Clear()
		{
			toWrap.Clear();
		}

		public bool Contains(KeyValuePair<string, TOut> item)
		{
			return toWrap.Contains(new KeyValuePair<string, TIn>(item.Key, (TIn)Conversion.ConvertTo<TIn>(item.Value)));
		}

		public void CopyTo(KeyValuePair<string, TOut>[] array, int arrayIndex)
		{
			toWrap.Select((KeyValuePair<string, TIn> pair) => new KeyValuePair<string, TOut>(pair.Key, (TOut)Conversion.ConvertTo<TOut>(pair.Value))).ToList().CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<string, TOut> item)
		{
			return toWrap.Remove(new KeyValuePair<string, TIn>(item.Key, (TIn)Conversion.ConvertTo<TIn>(item.Value)));
		}

		public IEnumerator<KeyValuePair<string, TOut>> GetEnumerator()
		{
			foreach (KeyValuePair<string, TIn> item in toWrap)
			{
				yield return new KeyValuePair<string, TOut>(item.Key, (TOut)Conversion.ConvertTo<TOut>(item.Value));
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
