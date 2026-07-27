using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Parse.Utilities;

namespace Parse.Internal
{
	internal class ParseRemoveOperation : IParseFieldOperation
	{
		private ReadOnlyCollection<object> objects;

		public IEnumerable<object> Objects
		{
			get
			{
				return objects;
			}
		}

		public ParseRemoveOperation(IEnumerable<object> objects)
		{
			this.objects = new ReadOnlyCollection<object>(objects.Distinct().ToList());
		}

		public object Encode()
		{
			return new Dictionary<string, object>
			{
				{ "__op", "Remove" },
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
				return previous;
			}
			if (previous is ParseSetOperation)
			{
				IList<object> oldValue = Conversion.As<IList<object>>(((ParseSetOperation)previous).Value);
				return new ParseSetOperation(Apply(oldValue, null));
			}
			if (previous is ParseRemoveOperation)
			{
				return new ParseRemoveOperation(((ParseRemoveOperation)previous).Objects.Concat(objects));
			}
			throw new InvalidOperationException("Operation is invalid after previous operation.");
		}

		public object Apply(object oldValue, string key)
		{
			if (oldValue == null)
			{
				return new List<object>();
			}
			return Conversion.As<IList<object>>(oldValue).Except(objects, ParseFieldOperations.ParseObjectComparer).ToList();
		}
	}
}
