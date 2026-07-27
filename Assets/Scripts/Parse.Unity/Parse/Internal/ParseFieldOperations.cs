using System;
using System.Collections.Generic;

namespace Parse.Internal
{
	internal static class ParseFieldOperations
	{
		private static ParseObjectIdComparer comparer;

		public static IEqualityComparer<object> ParseObjectComparer
		{
			get
			{
				if (comparer == null)
				{
					comparer = new ParseObjectIdComparer();
				}
				return comparer;
			}
		}

		public static IParseFieldOperation Decode(IDictionary<string, object> json)
		{
			throw new NotImplementedException();
		}
	}
}
