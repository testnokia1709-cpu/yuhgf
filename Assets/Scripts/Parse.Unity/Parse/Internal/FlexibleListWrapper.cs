using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class FlexibleListWrapper<TOut, TIn> : IList<TOut>, ICollection<TOut>, IEnumerable<TOut>, IEnumerable
	{
		private IList<TIn> toWrap;

		public TOut this[int index]
		{
			get
			{
				return (TOut)Conversion.ConvertTo<TOut>(toWrap[index]);
			}
			set
			{
				toWrap[index] = (TIn)Conversion.ConvertTo<TIn>(value);
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

		public FlexibleListWrapper(IList<TIn> toWrap)
		{
			this.toWrap = toWrap;
		}

		public int IndexOf(TOut item)
		{
			return toWrap.IndexOf((TIn)Conversion.ConvertTo<TIn>(item));
		}

		public void Insert(int index, TOut item)
		{
			toWrap.Insert(index, (TIn)Conversion.ConvertTo<TIn>(item));
		}

		public void RemoveAt(int index)
		{
			toWrap.RemoveAt(index);
		}

		public void Add(TOut item)
		{
			toWrap.Add((TIn)Conversion.ConvertTo<TIn>(item));
		}

		public void Clear()
		{
			toWrap.Clear();
		}

		public bool Contains(TOut item)
		{
			return toWrap.Contains((TIn)Conversion.ConvertTo<TIn>(item));
		}

		public void CopyTo(TOut[] array, int arrayIndex)
		{
			toWrap.Select((TIn item) => (TOut)Conversion.ConvertTo<TOut>(item)).ToList().CopyTo(array, arrayIndex);
		}

		public bool Remove(TOut item)
		{
			return toWrap.Remove((TIn)Conversion.ConvertTo<TIn>(item));
		}

		public IEnumerator<TOut> GetEnumerator()
		{
			foreach (object item in (IEnumerable)toWrap)
			{
				yield return (TOut)Conversion.ConvertTo<TOut>(item);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
