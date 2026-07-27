using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class ParseAddUniqueOperation : IParseFieldOperation
	{
		private ReadOnlyCollection<object> objects;

		public IEnumerable<object> Objects
		{
			get
			{
				return objects;
			}
		}

		public ParseAddUniqueOperation(IEnumerable<object> objects)
		{
			this.objects = new ReadOnlyCollection<object>(objects.Distinct().ToList());
		}

		public object Encode()
		{
			return new Dictionary<string, object>
			{
				{ "__op", "AddUnique" },
				{
					"objects",
					PointerOrLocalIdEncoder.Instance.Encode(objects)
				}
			};
		}

		public IParseFieldOperation MergeWithPrevious(IParseFieldOperation previous)
		{
			if (previous == null)
			{
				return this;
			}
			if (previous is ParseDeleteOperation)
			{
				return new ParseSetOperation(objects.ToList());
			}
			if (previous is ParseSetOperation)
			{
				IList<object> oldValue = (IList<object>)Conversion.ConvertTo<IList<object>>(((ParseSetOperation)previous).Value);
				return new ParseSetOperation(Apply(oldValue, null));
			}
			if (previous is ParseAddUniqueOperation)
			{
				IEnumerable<object> oldValue2 = ((ParseAddUniqueOperation)previous).Objects;
				return new ParseAddUniqueOperation((IList<object>)Apply(oldValue2, null));
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, string key)
		{
			if (oldValue == null)
			{
				return objects.ToList();
			}
			List<object> list = ((IList<object>)Conversion.ConvertTo<IList<object>>(oldValue)).ToList();
			IEqualityComparer<object> comparer = ParseFieldOperations.ParseObjectComparer;
			foreach (object objToAdd in objects)
			{
				if (objToAdd is ParseObject)
				{
					object obj = list.FirstOrDefault((object listObj) => comparer.Equals(objToAdd, listObj));
					if (obj == null)
					{
						list.Add(objToAdd);
						continue;
					}
					int index = list.IndexOf(obj);
					list[index] = objToAdd;
				}
				else if (!list.Contains(objToAdd, comparer))
				{
					list.Add(objToAdd);
				}
			}
			return list;
		}
	}
}
