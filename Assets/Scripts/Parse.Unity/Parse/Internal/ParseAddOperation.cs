using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class ParseAddOperation : IParseFieldOperation
	{
		private ReadOnlyCollection<object> objects;

		public IEnumerable<object> Objects
		{
			get
			{
				return objects;
			}
		}

		public ParseAddOperation(IEnumerable<object> objects)
		{
			this.objects = new ReadOnlyCollection<object>(objects.ToList());
		}

		public object Encode()
		{
			return new Dictionary<string, object>
			{
				{ "__op", "Add" },
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
				return new ParseSetOperation(((IList<object>)Conversion.ConvertTo<IList<object>>(((ParseSetOperation)previous).Value)).Concat(objects).ToList());
			}
			if (previous is ParseAddOperation)
			{
				return new ParseAddOperation(((ParseAddOperation)previous).Objects.Concat(objects));
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, string key)
		{
			if (oldValue == null)
			{
				return objects.ToList();
			}
			return ((IList<object>)Conversion.ConvertTo<IList<object>>(oldValue)).Concat(objects).ToList();
		}
	}
}
